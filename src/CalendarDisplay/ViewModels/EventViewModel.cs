﻿using CalendarDisplay.Options;
using Microsoft.Graph;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("CalendarDisplay.Tests")]

namespace CalendarDisplay.ViewModels;

public class EventViewModel
{
    public EventViewModel(Event @event, string backgroundColor, string textColor, TimeZoneOptions timezoneOptions)
    {
        // All day events dont need the timezone adjustment
        OrganizerEmail = @event.Organizer.EmailAddress.Address;
        Subject = @event.Subject;
        Start = (@event.IsAllDay ?? false)
            ? DateTime.Parse(@event.Start.DateTime)
            : DateTime.Parse(@event.Start.DateTime).FromUtcTo(timezoneOptions);
        End = (@event.IsAllDay ?? false)
            ? DateTime.Parse(@event.End.DateTime)
            : DateTime.Parse(@event.End.DateTime).FromUtcTo(timezoneOptions);
        AllDay = @event.IsAllDay ?? false;
        Recurring = @event.Recurrence is not null ? true : false;
        BackgroundColor = backgroundColor;
        TextColor = textColor;
    }

    public EventViewModel(EventViewModel vm)
    {
        OrganizerEmail = vm.OrganizerEmail;
        Subject = vm.Subject;
        Start = vm.Start;
        End = vm.End;
        AllDay = vm.AllDay;
        Recurring = vm.Recurring;
        BackgroundColor = vm.BackgroundColor;
        TextColor = vm.TextColor;
    }

    internal EventViewModel(
        string organizerEmail,
        string subject,
        DateTime start,
        DateTime end,
        bool allDay,
        bool recurring,
        string backgroundColor,
        string textColor)
    {
        OrganizerEmail = organizerEmail;
        Subject = subject;
        Start = start;
        End = end;
        AllDay = allDay;
        Recurring = recurring;
        BackgroundColor = backgroundColor;
        TextColor = textColor;
    }

    public string OrganizerEmail { get; }

    public string Subject { get; }

    public DateTime Start { get; }

    public DateTime End { get; }

    public bool AllDay { get; }

    public bool Recurring { get; }

    public string BackgroundColor { get; }

    public string TextColor { get; }

    public EventViewModel Clone() => new(this);

    public override string ToString()
    {
        var eventString = (AllDay ? string.Empty : Start.ToTimeString()) + " " + Subject;
        return eventString;
    }
}

public class EventViewModelList : List<EventViewModel>
{
}

public static class EventViewModelExtensions
{
    public static EventViewModelList ExpandMultiDayEvent(this EventViewModelList list)
    {
        EventViewModelList expandedList = new();
        foreach (var item in list)
        {
            //Add first event
            expandedList.Add(item);

            // If the actual dates are different, we have either an all day event, or a multi day event
            if (item.Start.Date != item.End.Date &&
                item.Start.AddDays(1) != item.End) // All day events end on the next day, but at the same time as the start day
            {
                var t = item.Start;
                while (t < item.End)
                {
                    t = t.AddDays(1);
                    if (t != item.End) // Only add if we aren't at the end
                    {
                        var dup = new EventViewModel(item.OrganizerEmail, item.Subject, t, item.End, item.AllDay, item.Recurring, item.BackgroundColor, item.TextColor);
                        expandedList.Add(dup);
                    }
                }
            }
        }
        return expandedList;
    }

    public static EventViewModelList ToEventViewModelList(this IEnumerable<EventViewModel> enumerable)
    {
        EventViewModelList list = new();
        list.AddRange(enumerable);
        return list;
    }
}