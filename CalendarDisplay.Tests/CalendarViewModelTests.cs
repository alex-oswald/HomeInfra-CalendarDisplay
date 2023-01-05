using CalendarDisplay.Data;
using CalendarDisplay.Options;
using CalendarDisplay.ViewModels;
using Microsoft.Extensions.Options;
using Moq;
using System;
using Xunit;

namespace CalendarDisplay.Tests;

public class CalendarViewModelTests
{
    [Fact]
    public void CreateCalendarTest()
    {
        var calendarOptions = new Mock<IOptions<CalendarOptions>>().Object;
        var timeZoneOptions = new Mock<IOptions<TimeZoneOptions>>().Object;
        var calendarService = new Mock<ICalendarManager>().Object;
        var viewModel = new CalendarViewModel(calendarOptions, timeZoneOptions, calendarService);
        var timeZoneId = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
        var dateTimeZone = DateTimeZone.FromTimeZone(new DateTime(2021, 2, 1), timeZoneId);
        var grid = viewModel.CreateCalendar(dateTimeZone, new());

        Assert.NotNull(grid);
        Assert.Equal(5, grid.CalendarWeeks.Count);
    }
}