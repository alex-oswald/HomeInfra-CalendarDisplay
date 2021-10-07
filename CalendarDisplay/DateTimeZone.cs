using CalendarDisplay.Options;
using System;

namespace CalendarDisplay
{
    public record DateTimeZone
    {
        internal DateTimeZone(DateTime utcDateTime, TimeZoneInfo timeZone)
        {
            UniversalTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
            TimeZone = timeZone;
            LocalTime = TimeZoneInfo.ConvertTime(UniversalTime, TimeZone);
        }

        public DateTime UniversalTime { get; init; }

        public TimeZoneInfo TimeZone { get; init; }

        public DateTime LocalTime { get; init; }

        public static DateTimeZone UtcNow() => new(DateTime.UtcNow, TimeZoneInfo.Utc);

        public static DateTimeZone UtcNow(TimeZoneInfo timeZone) => new(DateTime.UtcNow, timeZone);

        public static DateTimeZone FromTimeZone(DateTime dateTime, TimeZoneInfo timeZone)
        {
            var dateTimeUnspec = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
            var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(dateTimeUnspec, timeZone);
            return new DateTimeZone(utcDateTime, timeZone);
        }
    }

    public static class TimeZoneOptionsExtensions
    {
        public static TimeZoneInfo FromTimeZoneOptions(this TimeZoneOptions options)
        {
            // Windows
            if (TryFindSystemTimeZoneById(options.WindowsTimeZoneId, out TimeZoneInfo winTimeZoneInfo))
            {
                return winTimeZoneInfo;
            }
            // Linux
            else if (TryFindSystemTimeZoneById(options.UnixTimeZoneId, out TimeZoneInfo unixTimeZoneInfo))
            {
                return unixTimeZoneInfo;
            }
            else
            {
                throw new TimeZoneNotFoundException();
            }
        }

        /// <summary>
        /// Tries to find a <see cref="TimeZoneInfo"/> for the specified id.
        /// An id for one timezone can be different on Windows and Linux.
        /// </summary>
        internal static bool TryFindSystemTimeZoneById(string id, out TimeZoneInfo timeZoneInfo)
        {
            try
            {
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(id);
            }
            catch (TimeZoneNotFoundException)
            {
                timeZoneInfo = null;
                return false;
            }
            return true;
        }
    }
}