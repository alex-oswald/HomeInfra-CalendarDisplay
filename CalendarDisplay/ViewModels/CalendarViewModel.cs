using CalendarDisplay.Data;
using CalendarDisplay.Models;
using CalendarDisplay.Options;
using Microsoft.Extensions.Options;

namespace CalendarDisplay.ViewModels;

public interface ICalendarViewModel
{
    EventViewModelList Events { get; }

    DateTimeZone CurrentDateTime { get; }

    CalendarGrid Grid { get; }

    Task InitAsync(Action stateChanged);
}

public class CalendarViewModel : ICalendarViewModel, IDisposable
{
    private readonly CalendarOptions _options;
    private readonly TimeZoneOptions _timezoneOptions;
    private readonly ICalendarManager _calendarManager;
    private CancellationTokenSource _cancellationTokenSource;
    private Action _stateChanged;

    public CalendarViewModel(
        IOptions<CalendarOptions> options,
        IOptions<TimeZoneOptions> timezoneOptions,
        ICalendarManager calendarManager)
    {
        _options = options.Value;
        _timezoneOptions = timezoneOptions.Value;
        _calendarManager = calendarManager;
    }

    public EventViewModelList Events { get; private set; } = new();

    public DateTimeZone CurrentDateTime { get; private set; }

    public CalendarGrid Grid { get; private set; }

    public async Task InitAsync(Action stateChanged)
    {
        _stateChanged = stateChanged;

        var timezone = _timezoneOptions.FromTimeZoneOptions();
        CurrentDateTime = DateTimeZone.UtcNow(timezone);
        await UpdateGrid();

        _cancellationTokenSource = new CancellationTokenSource();
        _ = Ticker(_cancellationTokenSource.Token);
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

    public async Task Ticker(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var timezone = _timezoneOptions.FromTimeZoneOptions();
            CurrentDateTime = DateTimeZone.UtcNow(timezone);
            _stateChanged?.Invoke();
            await Task.Delay(1000, cancellationToken);
        }
    }

    public async Task UpdateGrid(CancellationToken cancellationToken = default)
    {
        var timezone = _timezoneOptions.FromTimeZoneOptions();
        var start = DateTimeZone.FromTimeZone(
            new DateTime(CurrentDateTime.LocalTime.Year, CurrentDateTime.LocalTime.Month, 1)
            .AddDays(-8), timezone);
        var end = DateTimeZone.FromTimeZone(
            new DateTime(CurrentDateTime.LocalTime.Year, CurrentDateTime.LocalTime.Month, 
                DateTime.DaysInMonth(CurrentDateTime.LocalTime.Year, CurrentDateTime.LocalTime.Month))
            .AddDays(8), timezone);

        Events = new();
        foreach (var calendar in _options.Calendars)
        {
            var events = (await _calendarManager.GetEventsBetweenDatesAsync(calendar.Name, start, end, cancellationToken))
                .Select(e => new EventViewModel(e, calendar.BackgroundColor, calendar.TextColor, _timezoneOptions))
                .ToEventViewModelList()
                .ExpandMultiDayEvent();
            Events.AddRange(events);
        }

        Grid = CreateCalendar(CurrentDateTime, Events);
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        GC.SuppressFinalize(this);
    }

    public CalendarGrid CreateCalendar(DateTimeZone date, EventViewModelList eventViewModels)
    {
        // Build the calendar. We have 7 columns in the calendar and between 4-6 rows (2015-2)
        // depending on how many days there are in the month and what day of the week
        // the month starts on.
        var startingDayOfMonth = new DateTime(date.LocalTime.Year, date.LocalTime.Month, 1);
        var startingDayOfMonthDayOfWeek = (int)startingDayOfMonth.DayOfWeek;
        var daysInMonth = DateTime.DaysInMonth(date.LocalTime.Year, date.LocalTime.Month);

        // We start with some empty days to represent the days at the end of the previous month
        var calendarDays = Enumerable.Range(0, startingDayOfMonthDayOfWeek)
            .Select(day => new CalendarDay(startingDayOfMonth.AddDays(-(1 + day))))
            .Reverse()
            .ToList();

        // Add each day of the month
        for (int day = 1; day <= daysInMonth; day++)
        {
            var thisDay = new DateTime(date.LocalTime.Year, date.LocalTime.Month, day);
            calendarDays.Add(new CalendarDay(thisDay));
        }

        // Get the days left to add to fill up a full week at the end of the month
        var daysLeft = 7 - calendarDays.Count % 7;
        var nextMonthsDays = Enumerable.Range(1, daysLeft)
            .Select(day => new CalendarDay(new DateTime(
                date.LocalTime.Month == 12 ? date.LocalTime.Year + 1 : date.LocalTime.Year,
                date.LocalTime.Month == 12 ? 1 : date.LocalTime.Month + 1,
                day)))
            .ToList();
        calendarDays.AddRange(nextMonthsDays);

        // Contruct the calendar grid
        List<CalendarWeek> weeks = new();
        for (int dayIndex = 0, weekIndex = -1; dayIndex < calendarDays.Count; dayIndex++)
        {
            if (dayIndex % 7 == 0)
            {
                weekIndex++;
                weeks.Add(new CalendarWeek());
            }
            weeks[weekIndex].Days.Add(calendarDays[dayIndex]);
        }

        // Loop through each day in the calendar and add any events
        foreach (var week in weeks)
        {
            foreach (var day in week.Days)
            {
                var thisDay = new DateTime(day.Day.Year, day.Day.Month, day.Day.Day);
                var daysEvents = eventViewModels.Where(e => e.Start.Date == thisDay).ToList();
                day.Events.AddRange(daysEvents);
            }
        }

        return new CalendarGrid() { CalendarWeeks = weeks };
    }
}