namespace CalendarDisplay.Options
{
    public class TimeZoneOptions
    {
        public const string Section = "TimeZoneOptions";

        public string WindowsTimeZoneId { get; set; }

        public string UnixTimeZoneId { get; set; }
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