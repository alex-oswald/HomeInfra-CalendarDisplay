using CalendarDisplay.Data;
using CalendarDisplay.Options;
using CalendarDisplay.ViewModels;
using Microsoft.Extensions.Options;
using Moq;
using System;
using Xunit;

namespace CalendarDisplay.Tests
{
    public class CalendarViewModelTests
    {
        [Fact]
        public void CreateCalendarTest()
        {
            var options = new Mock<IOptions<CalendarOptions>>().Object;
            var calendarService = new Mock<ICalendarManager>().Object;
            var viewModel = new CalendarViewModel(options, calendarService);
            var grid = viewModel.CreateCalendar(new DateTime(2021, 2, 1), new());

            Assert.NotNull(grid);
            Assert.Equal(5, grid.CalendarWeeks.Count);
        }
    }
}