using FamilyBoard.Data;
using FamilyBoard.Options;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FamilyBoard.ViewModels
{
    public interface ICountdownViewModel
    {
        CountdownEventViewModel Event { get; }

        Task InitAsync(Action stateChanged);
    }

    public class CountdownViewModel : ICountdownViewModel, IDisposable
    {
        private readonly CountdownOptions _options;
        private readonly ICalendarManager _calendarManager;
        private CancellationTokenSource _cancellationTokenSource;
        private Action _stateChanged;

        public CountdownViewModel(
            IOptions<CountdownOptions> options,
            ICalendarManager calendarManager)
        {
            _options = options.Value;
            _calendarManager = calendarManager;
        }

        public CountdownEventViewModel Event { get; private set; }

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
            var start = DateTime.Now;
            var events = await _calendarManager.GetEventsBetweenDatesAsync(
                _options.CalendarName, start, start.AddMonths(_options.LookupMonths), cancellationToken);
            Event = events.Select(e => new CountdownEventViewModel(e))
                .OrderBy(e => e.Start)
                .FirstOrDefault();
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }
}
