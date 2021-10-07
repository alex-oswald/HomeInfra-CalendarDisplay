namespace CalendarDisplay.Options
{
    public class TimeZoneOptions
    {
        public const string Section = "TimeZoneOptions";

        public string WindowsTimeZoneId { get; set; }

        public string UnixTimeZoneId { get; set; }
    }
}