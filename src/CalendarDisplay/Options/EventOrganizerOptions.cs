namespace CalendarDisplay.Options;

public record EventOrganizerOptions
{
    public const string Section = "EventOrganizerOptions";

    public List<EventOrganizer> Organizers { get; set; }

    public class EventOrganizer
    {
        public string OrganizerEmail { get; set; }
        public string BackgroundColor { get; set; }
        public string TextColor { get; set; }
    }
}
