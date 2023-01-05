using Microsoft.Graph;
using Microsoft.Identity.Web;

namespace CalendarDisplay.Data;

public interface ICalendarManager
{
    Task<List<Event>> GetEventsBetweenDatesAsync(string calendarName, DateTimeZone start, DateTimeZone end, CancellationToken cancellationToken = default);
}

public class CalendarManager : ICalendarManager
{
    private List<Calendar> _cachedCalendars = null;
    private readonly ILogger<CalendarManager> _logger;
    private readonly GraphServiceClient _graphServiceClient;

    public CalendarManager(
        ILogger<CalendarManager> logger,
        GraphServiceClient graphServiceClient)
    {
        _logger = logger;
        _graphServiceClient = graphServiceClient;
    }

    [AuthorizeForScopes(ScopeKeySection = "DownstreamApi:Scopes")]
    public async Task<string> GetCalendarIdAsync(string calendarName, CancellationToken cancellationToken = default)
    {
        // Fetch a list of calendars
        if (_cachedCalendars is null)
        {
            _cachedCalendars = (await _graphServiceClient.Me.Calendars
                .Request()
                .GetAsync(cancellationToken)).ToList();
            _logger.LogInformation("Calendar cache was empty, fetched {count} calendars", _cachedCalendars.Count);
        }

        // Return the calendar id
        return _cachedCalendars.Where(o => o.Name == calendarName).Single().Id;
    }

    [AuthorizeForScopes(ScopeKeySection = "DownstreamApi:Scopes")]
    public async Task<List<Event>> GetEventsBetweenDatesAsync(
        string calendarName, DateTimeZone start, DateTimeZone end, CancellationToken cancellationToken = default)
    {
        try
        {
            var calendarId = await GetCalendarIdAsync(calendarName, cancellationToken);

            var startDate = start.UniversalTime.Date.ToString("o");
            var endDate = end.UniversalTime.Date.ToString("o");

            var queryOptions = new List<QueryOption>()
            {
                new QueryOption("startDateTime", startDate),
                new QueryOption("endDateTime", endDate)
            };

            ICalendarCalendarViewCollectionPage page = await _graphServiceClient.Me.Calendars[calendarId].CalendarView
                .Request(queryOptions)
                .GetAsync(cancellationToken);
            List<Event> events = page.ToList();

            // Query the rest of the pages
            while (page.NextPageRequest is not null)
            {
                page = await page.NextPageRequest.GetAsync(cancellationToken);
                events.AddRange(page.ToList());
            }

            _logger.LogInformation("{this} success, {eventCount} events found.",
                nameof(GetEventsBetweenDatesAsync),
                events.Count);

            return events;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "{this} failed.", nameof(GetEventsBetweenDatesAsync));
            return new();
        }
    }
}