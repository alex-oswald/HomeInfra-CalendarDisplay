using System.Collections.Generic;

namespace FamilyBoard
{
    public class CalendarOptions
    {
        public List<Calendar> Calendars { get; set; }

        public int UpdateFrequency { get; set; } = 3_600;

        public class Calendar
        {
            public string Name { get; set; }

            public string BackgroundColor { get; set; }

            public string TextColor { get; set; }
        }
    }
}