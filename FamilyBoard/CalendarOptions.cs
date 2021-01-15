using System.Collections.Generic;

namespace FamilyBoard
{
    public class CalendarOptions
    {
        public List<Calendar> Calendars { get; set; }

        public class Calendar
        {
            public string Name { get; set; }

            public string Color { get; set; }
        }
    }
}