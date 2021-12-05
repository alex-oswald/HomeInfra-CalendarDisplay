using CalendarDisplay.ViewModels;

namespace CalendarDisplay.Models
{
    public record CalendarDay
    {
        public CalendarDay(DateTime day)
            : this(day, new())
        {
            Day = day;
        }

        public CalendarDay(DateTime day, List<EventViewModel> events)
        {
            Day = day;
            Events = events;
        }

        public DateTime Day { get; }

        public List<EventViewModel> Events { get; }
    }

    public class CalendarWeek
    {
        public List<CalendarDay> Days { get; set; } = new();
    }

    public class CalendarGrid
    {
        public List<CalendarWeek> CalendarWeeks { get; set; } = new();
    }
}