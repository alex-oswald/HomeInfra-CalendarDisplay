namespace FamilyBoard.Options
{
    public class CountdownOptions
    {
        public string CalendarName { get; set; }

        public int UpdateFrequency { get; set; } = 86_400;

        public int LookupMonths { get; set; } = 24;
    }
}
