using CalendarDisplay.Data;
using CalendarDisplay.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CalendarDisplay.ViewModels
{
    public interface ICountdownViewModel
    {
        List<CountdownEventViewModel> Events { get; }

        Task InitAsync(Action stateChanged);
    }

    public class CountdownViewModel : ICountdownViewModel, IDisposable
    {
        private readonly CountdownOptions _options;
        private readonly TimeZoneOptions _timezoneOptions;
        private readonly ICalendarManager _calendarManager;
        private CancellationTokenSource _cancellationTokenSource;
        private Action _stateChanged;

        public CountdownViewModel(
            IOptions<CountdownOptions> options,
            IOptions<TimeZoneOptions> timezoneOptions,
            ICalendarManager calendarManager)
        {
            _options = options.Value;
            _timezoneOptions = timezoneOptions.Value;
            _calendarManager = calendarManager;
        }

        public List<CountdownEventViewModel> Events { get; private set; } = new();

        public async Task InitAsync(Action stateChanged)
        {
            _stateChanged = stateChanged;

            await UpdateGrid();

            _cancellationTokenSource = new CancellationTokenSource();
            _ = PollData(_cancellationTokenSource.Token);
        }

        private async Task PollData(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(_options.UpdateFrequency), cancellationToken);
                await UpdateGrid(cancellationToken);

                _stateChanged?.Invoke();
            }
        }

        public async Task UpdateGrid(CancellationToken cancellationToken = default)
        {
            var timezone = _timezoneOptions.FromTimeZoneOptions();
            var start = DateTimeZone.UtcNow(timezone);
            var end = DateTimeZone.FromTimeZone(start.LocalTime.AddMonths(_options.LookupMonths), timezone);

            var events = await _calendarManager.GetEventsBetweenDatesAsync(_options.CalendarName, start, end, cancellationToken);

            Events = events.Select(e => new CountdownEventViewModel(e, _timezoneOptions))
                .OrderBy(e => e.Start)
                .Take(_options.CountdownsCount)
                .ToList();
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }
}
