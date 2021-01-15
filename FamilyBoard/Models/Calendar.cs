using FamilyBoard.ViewModels;
using System;
using System.Collections.Generic;

namespace FamilyBoard.Models
{
    public class CalendarDay
    {
        public CalendarDay(DateTime day, List<EventViewModel> events)
        {
            Day = day;
            Events = events;
        }

        public DateTime Day { get; set; }

        public List<EventViewModel> Events { get; set; } = new();
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