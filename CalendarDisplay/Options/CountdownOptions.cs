namespace CalendarDisplay.Options;

public class CountdownOptions
{
    public const string Section = "CountdownOptions";

    public string CalendarName { get; set; }

    public int UpdateFrequency { get; set; } = 86_400;

    public int LookupMonths { get; set; } = 24;

    public int CountdownsCount { get; set; } = 3;
}
