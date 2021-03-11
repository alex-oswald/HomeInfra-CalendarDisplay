using Microsoft.Graph;
using System;

namespace FamilyBoard.ViewModels
{
    public class CountdownEventViewModel
    {
        public CountdownEventViewModel(Event e)
        {
            Subject = e.Subject;
            Start = (e.IsAllDay ?? false)
                ? DateTime.Parse(e.Start.DateTime)
                : DateTime.Parse(e.Start.DateTime).FromUtcToPacificStandardTime();
        }

        public string Subject { get; }

        public DateTime Start { get; }

        public override string ToString()
        {
            var now = DateTime.Now;
            var left = Start.Subtract(now);
            
            if (left.Days > 1)
            {
                return $"{left.Days} days until {Subject}";
            }
            else if (left.Days == 1)
            {
                return $"1 more day until {Subject}";
            }
            else
            {
                return Subject + " passed";
            }
        }
    }
}
