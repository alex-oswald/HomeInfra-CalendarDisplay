using CalendarDisplay.Options;
using System;
using System.Diagnostics;
using System.Globalization;

namespace CalendarDisplay
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Tries to find a <see cref="TimeZoneInfo"/> for the specified id.
        /// An id for one timezone can be different on Windows and Linux.
        /// </summary>
        private static bool TryFindSystemTimeZoneById(string id, out TimeZoneInfo timeZoneInfo)
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

        public static DateTime FromUtcTo(this DateTime dateTime, TimeZoneOptions options)
        {
            // Windows
            if (TryFindSystemTimeZoneById(options.WindowsTimeZoneId, out TimeZoneInfo winTimeZoneInfo))
            {
                Debug.WriteLine($"Found time zone '{options.WindowsTimeZoneId}'");
                return TimeZoneInfo.ConvertTimeFromUtc(dateTime, winTimeZoneInfo);
            }
            // Linux
            else if (TryFindSystemTimeZoneById(options.UnixTimeZoneId, out TimeZoneInfo unixTimeZoneInfo))
            {
                Debug.WriteLine($"Found time zone '{options.UnixTimeZoneId}'");
                return TimeZoneInfo.ConvertTimeFromUtc(dateTime, unixTimeZoneInfo);
            }
            else
            {
                throw new TimeZoneNotFoundException();
            }
        }

        public static string ToTimeString(this DateTime dateTime)
        {
            if (dateTime.Hour == 0 && dateTime.Minute == 0)
            {
                return "12" + dateTime.ToString("tt").ToLower();
            }
            else if (dateTime.Hour == 0)
            {
                return "12:" + dateTime.ToString("mmtt").ToLower();
            }
            else if (dateTime.Minute == 0)
            {
                return dateTime.ToString("htt").ToLower();
            }
            else
            {
                return dateTime.ToString("h:mmtt").ToLower();
            }
        }

        public static string ToMonthName(this DateTime dateTime) =>
            CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dateTime.Month);
    }
}