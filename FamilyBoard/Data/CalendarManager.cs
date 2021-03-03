using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FamilyBoard.Data
{
    public interface ICalendarManager
    {
        Task<List<Event>> GetMonthsEventsAsync(string calendarName, DateTime date, CancellationToken cancellationToken = default);
    }

    public class CalendarManager : ICalendarManager
    {
        private List<Calendar> _cachedCalendars = null;
        private string _cachedCalendarId;
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
        public async Task<List<Event>> GetMonthsEventsAsync(string calendarName, DateTime date, CancellationToken cancellationToken = default)
        {
            try
            {
                // Fetch a list of calendars
                if (_cachedCalendars is null)
                {
                    _cachedCalendars = (await _graphServiceClient.Me.Calendars
                        .Request()
                        .GetAsync(cancellationToken)).ToList();
                    _logger.LogInformation("Calendar cache was empty, fetched {count} calendars", _cachedCalendars.Count());
                }

                // Get the calendar id
                _cachedCalendarId = _cachedCalendars.Where(o => o.Name == calendarName).Single().Id;

                // Create the DateTime using local time, or system time (from the Raspberry Pi, or your dev machine)
                // This means the date string will include the offset and the search query will be correct for the local timezone
                var start = new DateTime(date.Year, date.Month, 1, 0, 0, 0, DateTimeKind.Local).ToString("O");
                var end = new DateTime(date.Year, date.Month + 1, 1, 0, 0, 0, DateTimeKind.Local).AddTicks(-1).ToString("O");

                var queryOptions = new List<QueryOption>()
                {
                    new QueryOption("startDateTime", start),
                    new QueryOption("endDateTime", end)
                };

                ICalendarCalendarViewCollectionPage page = await _graphServiceClient.Me.Calendars[_cachedCalendarId].CalendarView
                    .Request(queryOptions)
                    .GetAsync(cancellationToken);
                List<Event> events = page.ToList();

                // Query the rest of the pages
                while (page.NextPageRequest is not null)
                {
                    page = await page.NextPageRequest.GetAsync(cancellationToken);
                    events.AddRange(page.ToList());
                }

                _logger.LogInformation("{this} success, {eventCount} events for {month}.",
                    nameof(GetMonthsEventsAsync),
                    events.Count,
                    date.ToMonthName());

                return events;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "{this} failed.", nameof(GetMonthsEventsAsync));
                return new();
            }
        }
    }
}