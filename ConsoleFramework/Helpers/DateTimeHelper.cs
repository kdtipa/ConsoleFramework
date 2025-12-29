using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleFramework.Helpers;

/// <summary>
/// A small collection of methods to help with DateTime objects, 
/// especially in the context of Console display.
/// </summary>
public static class DateTimeHelper
{
    public static bool IsLeapYear(this DateTime dateTime)
    {
        return IsLeapYear(dateTime.Year);
    }

    public static bool IsLeapYear(int theYear)
    {
        if (theYear < 1) { return false; }

        if ((theYear % 4 == 0 && theYear % 100 != 0) || theYear % 400 == 0)
        {
            return true;
        }
        return false;
    }

    public static string MonthName(this DateTime dateTime)
    {
        return dateTime.ToString("MMMM");
    }

    public static string? MonthName(int theMonth)
    {
        if (theMonth < 1 || theMonth > 12) { return null; }

        var dt = new DateTime(2000, theMonth, 1);
        return dt.ToString("MMMM");
    }


    public static string DayOfWeekName(this DateTime dateTime)
    {
        return dateTime.ToString("dddd");
    }

    public static string DayOfWeekName(DayOfWeek whichDay)
    {
        var worker = DateTime.Now;
        var emergencyCounter = 0;

        while (worker.DayOfWeek != whichDay && emergencyCounter < 10) { worker.AddDays(-1); emergencyCounter++; }

        if (worker.DayOfWeek == whichDay) { return worker.ToString("dddd"); }
        return string.Empty; // should never get here, but hey it's not hurting anything
    }

    /// <summary>
    /// Gives you the collection of days of the week in order, usually starting with Sunday, 
    /// unless you're neurotic like me and want the weekend to be at the end of the week, 
    /// which means the start of the week is Monday.  It makes so much more sense.  But I 
    /// also realize everything is set up to treat Sunday as the beginning of the week, so 
    /// Sunday being first is the default.
    /// </summary>
    /// <param name="useAbbreviatedDayNames">
    /// Defaults to using the full name, but setting this to true will get you the short version. 
    /// </param>
    /// <param name="useMondayFirst">
    /// Defaults to using Sunday first which is how everything is set up in DateTime, but it 
    /// makes more sense for the weekend to be at the end of the week, so Monday really SHOULD 
    /// be the first day of the week.  If you want to, this method allows the good way.
    /// </param>
    /// <returns>The collection of days of the week in order.</returns>
    public static IEnumerable<string> DaysOfTheWeek(bool? useAbbreviatedDayNames = null, bool? useMondayFirst = null)
    {
        string dayNamePattern = useAbbreviatedDayNames is not null && useAbbreviatedDayNames == true ? "ddd" : "dddd";

        bool useMon1 = useMondayFirst is not null && useMondayFirst == true;
        DayOfWeek first = useMon1 ? DayOfWeek.Monday : DayOfWeek.Sunday;

        var worker = new DateTime();
        while (worker.DayOfWeek != first) { worker.AddDays(1.0); }

        for (int counter = 1; counter <= 7; counter++)
        {
            yield return worker.ToString(dayNamePattern);
            worker.AddDays(1.0);
        }
    }

    /// <summary>
    /// This method is meant to help when you need a 4 digit year from a 
    /// year representation that is one or two digits (and not negative).  
    /// If the current year is 2025 and you pass in 95, you should get 
    /// 1995 because it is closer in time than 2095 or 2195.
    /// </summary>
    /// <param name="TwoDigitYear">the year from 0 to 99</param>
    /// <returns>the year as given but with the closest century added.</returns>
    public static int? GetClosestFourDigitYear(int TwoDigitYear)
    {
        if (TwoDigitYear < 1) { return null; }

        var cleanTwoDigits = TwoDigitYear % 100;
        var currentYear = DateTime.Now.Year;
        var currentCentury = currentYear - (currentYear % 100);
        var previousCentury = currentCentury - 100;
        var nextCentury = currentCentury + 100;

        // get a difference for the current century
        var currentDiff = currentYear - (currentCentury + cleanTwoDigits);
        if (currentDiff < 0) { currentDiff *= -1; }

        // get a difference for the previous century
        var previousDiff = currentYear - (previousCentury + cleanTwoDigits);
        if (previousDiff < 0) { previousDiff *= -1; }

        // get a difference for the next century
        var nextDiff = currentYear - (nextCentury + cleanTwoDigits);
        if (nextDiff < 0) { nextDiff *= -1; }

        // now figure out which one is the lowest difference
        if (currentDiff <= previousDiff && currentDiff <= nextDiff) { return currentCentury + cleanTwoDigits; } // favor the current century
        if (previousDiff < currentDiff && previousDiff < nextDiff) { return previousCentury + cleanTwoDigits; }
        if (nextDiff < currentDiff && nextDiff < previousDiff) { return nextCentury + cleanTwoDigits; }

        return null; // we shouldn't be able to get here, but whatever.
    }


    private static int[] _daysInMonth { get; } = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];

    /// <summary>
    /// Gets you the number of days in the given month
    /// </summary>
    /// <param name="month">a number from 1 to 12 representing the month</param>
    /// <param name="year">
    /// providing a year means the method can tell if it's a leap year or not, 
    /// and potentially give you the 29 for a leap year February.  If no year 
    /// is provided, it will use the current year.
    /// </param>
    /// <returns>The number of days in the month you provide</returns>
    public static int? DaysInMonth(int month, int? year)
    {
        if (month < 1 || month > 12) { return null; }

        bool isLeapYear = IsLeapYear(year ?? DateTime.Now.Year); // if they don't specify a year, we'll use the current one

        if (isLeapYear && month == 2) { return 29; }

        return _daysInMonth[month - 1];
    }

    /// <summary>
    /// Gets you the number of days in the month and year of this instance of DateTime
    /// </summary>
    public static int? DaysInMonth(this DateTime dateTime)
    {
        return DaysInMonth(dateTime.Month, dateTime.Year);
    }


    public static string SQLDateTimeFormat(
        this DateTime dateTime, 
        bool? IncludeDate = null, 
        bool? IncludeTime = null, 
        bool? IncludeMilliseconds = null)
    {
        // set up our bools so we know what we're including
        bool inclD = IncludeDate ?? true;
        bool inclT = IncludeTime ?? true;
        bool inclM = IncludeMilliseconds ?? false;

        // this method isn't meant for just retrieving the milliseconds, so 
        // if the user wants the milliseconds, we need the time too.
        if (inclM && !inclT) { inclT = true; }

        // get our return variable...
        var retVal = new StringBuilder();

        // handle date
        if (inclD) { retVal.Append(dateTime.ToString("yyyy-MM-dd")); }

        // handle time
        if (inclT)
        {
            // need a space if we have a date already
            if (inclD) { retVal.Append(' '); }

            retVal.Append(dateTime.ToString("HH:mm:ss"));
        }

        // handle millisecond
        if (inclM) { retVal.Append(dateTime.ToString(".fff")); }

        // return our nifty well formatted value...
        return retVal.ToString();
    }

    /// <summary>
    /// This method gives you a string that gives date and time information 
    /// in a concise and clear way.  The date is in the format 4 January 2025, 
    /// so that there can't be confusion about which number in 4/1/25 is the 
    /// month and which is the day.  The time is always a 24 hour time, so we 
    /// don't need to worry about which half of the day we're in, like 17:42
    /// </summary>
    /// <param name="includeDate">
    /// Defaults to true, but can be excluded if all you want is the time. 
    /// If you pass false for both, includeDate will be defaulted to true.
    /// </param>
    /// <param name="includeTime">
    /// Defaults to false.  Set to true if you want the time to be included 
    /// in the return string.
    /// </param>
    /// <param name="fullMonthName">
    /// Defaults to true in which case it will show something like "January".  
    /// If set to false, it would show something like "Jan" instead.
    /// </param>
    /// <returns>A string representing a date and/or time in a clear way.</returns>
    public static string ClearDateTimeFormat(
        this DateTime dateTime, 
        bool? includeDate = null, 
        bool? includeTime = null, 
        bool? fullMonthName = null)
    {
        var retVal = new StringBuilder();

        bool inclD = includeDate ?? true;
        bool inclT = includeTime ?? false;

        if (!inclD && !inclT) { inclD = true; }

        string mn = fullMonthName ?? true ? "MMMM" : "MMM";
        string dtStr = $"d {mn} yyyy";

        if (inclD) { retVal.Append(dateTime.ToString(dtStr)); }
        if (inclD && inclT) { retVal.Append(", "); }
        if (inclT) { retVal.Append(dateTime.ToString("H:mm")); }

        return retVal.ToString();
    }

    /// <summary>
    /// Specifically a method to imply that each time it is 
    /// called, code is run.  So, preferrably you would run 
    /// it once and store the result.  The keys are the month 
    /// names in whatever language is the current one, and 
    /// the values are the numbers from 1 to 12.
    /// </summary>
    public static Dictionary<string, int> MonthNames()
    {
        var retVal = new Dictionary<string, int>();
        var worker = new DateTime(2025, 1, 5);

        while (worker.Month <= 12)
        {
            retVal.Add(worker.ToString("MMMM"), worker.Month);
            worker.AddMonths(1);
        }

        return retVal;
    }

    /// <summary>
    /// Potentially userful method for when you're parsing text for date parts.
    /// </summary>
    /// <returns>null if no match is found, or 1 to 12</returns>
    public static bool TryParseMonth(string sourceStr, [NotNullWhen(true)] out int? monthNumber, [NotNullWhen(true)] out string? fullMonthName)
    {
        monthNumber = null;
        fullMonthName = null;
        if (string.IsNullOrWhiteSpace(sourceStr) || sourceStr.Length < 3) { return false; }

        foreach (var item in MonthNames())
        {
            if (item.Key.StartsWith(sourceStr, StringComparison.OrdinalIgnoreCase)) 
            {
                fullMonthName = item.Key;
                monthNumber = item.Value;
                return true; 
            }
        }

        return false;
    }

    /// <summary>
    /// The minimum year that DateTime supports
    /// </summary>
    public static int MinYear() 
    {
        return DateTime.MinValue.Year;
    }

    /// <summary>
    /// The maximum year that DateTime supports
    /// </summary>
    public static int MaxYear()
    {
        return DateTime.MaxValue.Year;
    }

    /// <summary>
    /// looks for a one or two digit hour; colon; two digit minute; 
    /// possible colon; possible second; possible space; and possible 
    /// AM or PM (case insensitive and with possible periods).
    /// </summary>
    public static Regex TimeFormatRegex { get; } = new Regex("^([0-9]{1,2}):([0-9]{2}):?([0-9]{2})? ?([AaPp]\\.?[Mm]?\\.?)?", RegexOptions.Compiled);

    public static bool TryParseTimeString(
        string sourceString, 
        [NotNullWhen(true)] out int? hour,
        [NotNullWhen(true)] out int? minute,
        [NotNullWhen(true)] out int? second)
    {
        // defaults
        hour = null;
        minute = null;
        second = null;
        bool foundTime = false;

        // empty source
        if (string.IsNullOrWhiteSpace(sourceString)) { return false; }

        MatchCollection timeParts = TimeFormatRegex.Matches(sourceString);
        int partCount = timeParts.Count; // has to be 3 for hour and minute, 4 to include seconds or am/pm, and 5 to include seconds AND am/pm
        if (partCount >= 3)
        {
            if (int.TryParse(timeParts[1].Value, out int parsedHour) 
             && int.TryParse(timeParts[2].Value, out int parsedMinute))
            {
                if (parsedHour >= 0 && parsedHour <= 23) { hour = parsedHour; }
                if (parsedMinute >= 0 && parsedMinute <= 59) { minute = parsedMinute; }
                if (hour is not null && minute is not null) { foundTime = true; }
            }
        }

        // we only need to bother checking further if foundTime is true...
        if (foundTime)
        {
            second = 0; // update the default away from null since we found hour an minute

            if (partCount > 3)
            {
                // we can check the next group value for either a second or am/pm
                if (int.TryParse(timeParts[3].Value, out int parsedSecond))
                {
                    // this means the third value is the number of seconds
                    if (parsedSecond >= 0 && parsedSecond <= 59) { second = parsedSecond; }

                    if (partCount > 4)
                    {
                        // this means there's an am/pm...
                        char fourthVal = timeParts[4].Value.ToLower()[0];
                        if (fourthVal == 'p')
                        {
                            // in this case we have to see if the hour is 1 to 12 and adjust to 13-23,0
                            if (hour >= 1 && hour <= 12) { hour += 12; }
                            if (hour == 24) { hour = 0; }
                        }
                    }
                }
                else
                {
                    // this means we didn't find a second in that third spot, so see if it's am/pm
                    char fourthVal = timeParts[3].Value.ToLower()[0];
                    if (fourthVal == 'p')
                    {
                        // in this case we have to see if the hour is 1 to 12 and adjust to 13-23,0
                        if (hour >= 1 && hour <= 12) { hour += 12; }
                        if (hour == 24) { hour = 0; }
                    }
                }
            }
        }

        return foundTime;
    }
    
}

/// <summary>
/// DateTime objects are not easy to edit.  You can't just set the month for example.  
/// You have to either create a new object or use the AddMonths method.  This is a 
/// pain sometimes.  This class gives you a way of storing and easily manipulating 
/// the date data, and then getting the DateTime object built with the data you stored.
/// </summary>
public struct DateTimeLoader : IEquatable<DateTimeLoader>, IComparable<DateTimeLoader>
{
    public DateTimeLoader() { }

    public DateTimeLoader(DateTime initialValues)
    {
        Year = initialValues.Year;
        Month = initialValues.Month;
        Day = initialValues.Day;
        Hour = initialValues.Hour;
        Minute = initialValues.Minute;
        Second = initialValues.Second;
    }

    public DateTimeLoader(int year, int? month = null, int? day = null, int? hour = null, int? minute = null, int? second = null)
    {
        Year = year;
        Month = month ?? 1;
        Day = day ?? 1;
        Hour = hour ?? 0;
        Minute = minute ?? 0;
        Second = second ?? 0;
    }

    private int _year = 2025;
    public int Year
    {
        get { return _year; }
        set
        {
            int minYr = DateTime.MinValue.Year;
            int maxYr = DateTime.MaxValue.Year;
            int cleanYear = value;
            if (cleanYear >= 0 && cleanYear <= 99) { DateTimeHelper.GetClosestFourDigitYear(cleanYear); }
            if (cleanYear < minYr || cleanYear > maxYr) { throw new ArgumentOutOfRangeException(nameof(value), $"{value} must be from {minYr} to {maxYr}"); }
            _year = cleanYear;
        }
    }

    private int _month = 1;
    /// <summary>
    /// Must be a number from 1 to 12 or it 
    /// will throw an argument exception.
    /// </summary>
    public int Month
    {
        get { return _month; }
        set
        {
            if (value < 1 || value > 12) { throw new ArgumentOutOfRangeException("value", $"{value} is not a value from 1 to 12"); }
            _month = value;
            int dayLimit = DateTimeHelper.DaysInMonth(value, Year) ?? 31;
            if (Day > dayLimit) { _day = dayLimit; }
        }
    }

    private int _day = 1;
    /// <summary>
    /// Must be a number from 1 to the number of days in whatever Month is 
    /// set and will throw an exception if not.
    /// </summary>
    public int Day
    {
        get { return _day; }
        set
        {
            int limit = DateTimeHelper.DaysInMonth(Month, Year) ?? 31;
            if (value < 1 || value > limit) { throw new ArgumentOutOfRangeException("value", $"{value} is not from 1 to {limit}"); }
            _day = value;
        }
    }

    private int _hour = 0;
    /// <summary>
    /// This is a 24 hour clock, so 6PM is 18.  The range of 
    /// values is 0 to 23 since midnight is 0:00 and 11:59PM 
    /// is 23:59.  If out the range of 0 to 23, will throw 
    /// an exception.
    /// </summary>
    public int Hour
    {
        get { return _hour; }
        set
        {
            if (value < 0 || value > 23) { throw new ArgumentOutOfRangeException("value", $"{value} is not from 0 to 23"); }
        }
    }

    private int _minute = 0;
    public int Minute
    {
        get { return _minute; }
        set 
        { 
            if (value < 0 || value > 59) { throw new ArgumentOutOfRangeException("value", $"{value} must be from 0 to 59"); }
            _minute = value;
        }
    }

    private int _second = 0;
    public int Second
    {
        get { return _second; }
        set
        {
            if (value < 0 || value > 59) { throw new ArgumentOutOfRangeException("value", $"{value} must be from 0 to 59"); }
            _second = value;
        }
    }

    public DateTime GetDateTime()
    {
        return new DateTime(Year, Month, Day, Hour, Minute, Second);
    }

    public bool Equals(DateTimeLoader other)
    {
        return Year == other.Year && Month == other.Month && Day == other.Day 
            && Hour == other.Hour && Minute == other.Minute && Second == other.Second;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) { return false; }

        if (obj is DateTimeLoader dtlObj) { return Equals(dtlObj); }

        if (obj is DateTime dtObj) { return Equals(new DateTimeLoader(dtObj)); }

        return false;
    }

    public override int GetHashCode()
    {
        return GetDateTime().GetHashCode();
    }

    public override string ToString()
    {
        return GetDateTime().ToString();
    }

    public int CompareTo(DateTimeLoader other)
    {
        return DateTime.Compare(GetDateTime(), other.GetDateTime());
    }

    public static bool operator ==(DateTimeLoader a, DateTimeLoader b) { return a.Equals(b); }
    public static bool operator !=(DateTimeLoader a, DateTimeLoader b) { return !a.Equals(b); }


}

/// <summary>
/// This struct is here to help when parsing dates from longer strings that 
/// don't work in a DateTime.TryParse kind of scenario.  If you break a string 
/// up by spaces, you can feed each piece into a list of DateTimeParts
/// </summary>
public struct DateTimePart
{
    public DateTimePart() { }

    public DateTimePart(string sourcePartString)
    {
        SourceValue = sourcePartString;
    }

    private int minYear = DateTimeHelper.MinYear();
    private int maxYear = DateTimeHelper.MaxYear();


    private string _sourceValue = string.Empty;
    public string SourceValue
    {
        get { return _sourceValue; }
        set
        {
            // clear the properties...
            Clear();

            // set the base thing
            _sourceValue = value;

            // see if the string can be parsed as an integer
            if (int.TryParse(value, out int parsedInt)) 
            { 
                _intValue = parsedInt;
                IntValueIsPossibleMonth = parsedInt >= 1 && parsedInt <= 12;
                IntValueIsPossibleDayOfMonth = parsedInt >= 1 && parsedInt <= 31;
                IntValueIsPossibleYear = parsedInt >= minYear && parsedInt <= maxYear;
            }
            // see if it's the text string representation of a month
            else if (DateTimeHelper.TryParseMonth(value, out var mNum, out var fmn))
            {
                SourceIsMonthName = true;
                _intValue = mNum;
                MonthName = fmn;
            }
            // see if it's a time string
            else if (DateTimeHelper.TryParseTimeString(value, out var parsedH, out var parsedM, out var parsedS))
            {
                Hour = parsedH.Value;
                Minute = parsedM.Value;
                Second = parsedS.Value;
                SourceIsTime = true;
                TimeString = $"{Hour.Value:00}:{Minute.Value:00}:{Second.Value:00}";
            }

        }
    }

    private int? _intValue = null;
    public int? IntValue { get { return _intValue; } }

    public bool IntValueIsPossibleMonth { get; private set; } = false;

    /// <summary>
    /// returns true if the integer value is from 1 to 12, but that 
    /// true is still not a guarantee because it could be a day of 
    /// the month or the year.  It is really mostly helpful when you 
    /// only have one value that could be a month
    /// </summary>
    public bool LikelyMonth { get { return _intValue is not null && _intValue >= 1 && _intValue <= 12; } }

    public bool IntValueIsPossibleDayOfMonth { get; private set; } = false;

    /// <summary>
    /// If it's not from a text month name, can't also be a month number, 
    /// and can be a day of the month, this bool guesses that it is likely 
    /// a day of the month, but could also be a year even if true like 
    /// 25 being 2025, but also fitting as a day of the month.  And if the 
    /// number is 1 to 12, it realizes it could be a month, day, or year, 
    /// so returns false.
    /// </summary>
    public bool LikelyADayOfMonth
    {
        get
        {
            return _intValue is not null && !SourceIsMonthName && !IntValueIsPossibleMonth && IntValueIsPossibleDayOfMonth;
        }
    }

    public bool IntValueIsPossibleYear { get; private set; } = false;

    /// <summary>
    /// LikelyYear returns true if there is an integer value that is not 
    /// from a text month name; cannot be a month number; and cannot 
    /// be a day of the month.  So, it STILL MIGHT BE A YEAR even if this 
    /// is false because 12 could be 2012 for example.  Be careful with 
    /// those stupid two digit years.
    /// </summary>
    public bool LikelyYear
    {
        get
        {
            return _intValue is not null && !SourceIsMonthName && !IntValueIsPossibleMonth && !IntValueIsPossibleDayOfMonth;
        }
    }

    public bool IsFourDigitYear { get { return _intValue is not null && _intValue > 999 && _intValue < 10000; } }

    public bool SourceIsMonthName { get; private set; } = false;

    public string MonthName { get; private set; } = string.Empty;


    public bool SourceIsTime { get; private set; } = false;

    public string TimeString { get; private set; } = string.Empty;

    public int? Hour { get; private set; } = null;

    public int? Minute { get; private set; } = null;

    public int? Second { get; private set; } = null;

    /// <summary>
    /// This property helps when trying to parse a bigger string that 
    /// might contain more than just date or time information.  As you 
    /// parse each piece, this object will tell you if it's possibly 
    /// relevant information.
    /// </summary>
    public readonly bool NotAPossibleDateTimePart => (_intValue is null || _intValue < 1) && !SourceIsTime;

    public void Clear()
    {
        _sourceValue = string.Empty;
        _intValue = null;
        IntValueIsPossibleMonth = false;
        IntValueIsPossibleDayOfMonth = false;
        IntValueIsPossibleYear = false;
        SourceIsMonthName = false;
        MonthName = string.Empty;
        SourceIsTime = false;
        TimeString = string.Empty;
        Hour = null;
        Minute = null;
        Second = null;
    }
}


