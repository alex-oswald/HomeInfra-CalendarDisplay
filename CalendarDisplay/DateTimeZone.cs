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
}