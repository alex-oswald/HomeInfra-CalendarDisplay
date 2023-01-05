using System;
using Xunit;

namespace CalendarDisplay.Tests;

public class DateTimeWithZoneTests
{
    [Fact]
    public void UtcNow_Creation()
    {
        var utc = DateTimeZone.UtcNow();

        Assert.Equal(DateTimeKind.Utc, utc.LocalTime.Kind);
        Assert.Equal(DateTimeKind.Utc,utc.UniversalTime.Kind);

        Assert.Equal(TimeZoneInfo.Utc.Id, utc.TimeZone.Id);
        Assert.Equal(utc.UniversalTime, utc.LocalTime);
    }

    [Fact]
    public void UtcNow_Creation_TimeZone_Parameter()
    {
        var timeZoneId = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
        var dateTimeZone = DateTimeZone.UtcNow(timeZoneId);
        var pst = TimeZoneInfo.ConvertTimeFromUtc(dateTimeZone.UniversalTime, timeZoneId);

        Assert.Equal(DateTimeKind.Unspecified, dateTimeZone.LocalTime.Kind);
        Assert.Equal(DateTimeKind.Utc, dateTimeZone.UniversalTime.Kind);

        Assert.Equal(pst, dateTimeZone.LocalTime);
        Assert.Equal(timeZoneId.Id, dateTimeZone.TimeZone.Id);
    }
    [Fact]
    public void FromTimeZone_Creation()
    {
        var timeZoneId = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
        var utcNow = DateTime.UtcNow;
        var pst = TimeZoneInfo.ConvertTimeFromUtc(utcNow, timeZoneId);

        var dateTime = DateTimeZone.FromTimeZone(pst, timeZoneId);

        Assert.Equal(DateTimeKind.Unspecified, dateTime.LocalTime.Kind);
        Assert.Equal(DateTimeKind.Utc, dateTime.UniversalTime.Kind);

        Assert.Equal(utcNow, dateTime.UniversalTime);
        Assert.Equal(pst, dateTime.LocalTime);
        Assert.Equal(timeZoneId.Id, dateTime.TimeZone.Id);
    }
}