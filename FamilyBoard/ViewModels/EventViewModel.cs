using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FamilyBoard.Tests")]

namespace FamilyBoard.ViewModels
{
    public class EventViewModel
    {
        public EventViewModel(Event e, string backgroundColor, string textColor)
        {
            // All day events dont need the timezone adjustment
            Subject = e.Subject;
            Start = (e.IsAllDay ?? false)
                ? DateTime.Parse(e.Start.DateTime)
                : DateTime.Parse(e.Start.DateTime).FromUtcToPacificStandardTime();
            End = (e.IsAllDay ?? false)
                ? DateTime.Parse(e.End.DateTime)
                : DateTime.Parse(e.End.DateTime).FromUtcToPacificStandardTime();
            AllDay = e.IsAllDay ?? false;
            Recurring = e.Recurrence is not null ? true : false;
            BackgroundColor = backgroundColor;
            TextColor = textColor;
        }

        public EventViewModel(EventViewModel vm)
        {
            Subject = vm.Subject;
            Start = vm.Start;
            End = vm.End;
            AllDay = vm.AllDay;
            Recurring = vm.Recurring;
            BackgroundColor = vm.BackgroundColor;
            TextColor = vm.TextColor;
        }

        internal EventViewModel(
            string subject,
            DateTime start,
            DateTime end,
            bool allDay,
            bool recurring,
            string backgroundColor,
            string textColor)
        {
            Subject = subject;
            Start = start;
            End = end;
            AllDay = allDay;
            Recurring = recurring;
            BackgroundColor = backgroundColor;
            TextColor = textColor;
        }

        public string Subject { get; }

        public DateTime Start { get; }

        public DateTime End { get; }

        public bool AllDay { get; }

        public bool Recurring { get; }

        public string BackgroundColor { get; }

        public string TextColor { get; }

        public EventViewModel Clone() => new EventViewModel(this);

        public override string ToString()
        {
            var eventString = (AllDay ? string.Empty : Start.ToTimeString()) + " " + Subject;
            return eventString;
        }
    }

    public class EventViewModelList<TModel>
        where TModel : EventViewModel
    {
        public List<TModel> Items { get; set; } = new();
    }

    public static class EventViewModelExtensions
    {
        public static EventViewModelList<EventViewModel> ExpandMultiDayEvent(this EventViewModel vm)
        {
            EventViewModelList<EventViewModel> list = new();

            var days = vm.End.Subtract(vm.Start).Days;
            list.Items.Add(vm);

            for (int i = 1; i < days; i++)
            {
                EventViewModel duplicate = new EventViewModel(vm.Subject, vm.Start.AddDays(i), vm.End,
                    vm.AllDay, vm.Recurring, vm.BackgroundColor, vm.TextColor);
                list.Items.Add(duplicate);
            }

            return list;
        }
    }
}