using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleFramework.Helpers;

/// <summary>
/// This is a type instead of a helper really, but if you're dealing with 
/// date ranges, then it does try to help.  It keeps a start and end date 
/// gives you some useful methods for dealing with them as a unit, including 
/// some ToString helpers.
/// </summary>
public struct DateTimeRange : IEquatable<DateTimeRange>, IComparable<DateTimeRange>
{
    public DateTimeRange() { }

    public DateTimeRange(DateTime start, DateTime end) 
    { 
        _start = start; // don't need the logic the first time through
        End = end; // for the second part, use the public property so it tests the values
    }

    public DateTimeRange(DateTime start, TimeSpan length)
    {
        _start = start;
        _end = start + length;
    }

    public DateTimeRange(string stringRange, bool? favorMonthBeforeDayOrder = null)
    {
        if (TryParse(stringRange, out DateTimeRange? parsedResult, favorMonthBeforeDayOrder))
        {
            _start = parsedResult.Value.Start;
            _end = parsedResult.Value.End;
        }
    }

    public DateTimeRange(string start, string end)
    {
        if (DateTime.TryParse(start, out var parsedStart) 
         && DateTime.TryParse(end, out var parsedEnd))
        {
            _start = parsedStart;
            End = parsedEnd;
        }
    }

    private DateTime _start = DateTime.Now;
    private DateTime _end = DateTime.Now;
    private TimeSpan _range = TimeSpan.Zero;

    public DateTime Start
    {
        get { return _start; }
        set
        {
            if (value > _end)
            {
                _start = _end;
                _end = value;
            }
            else
            {
                _start = value;
            }
            _range = _end - _start;
        }
    }

    public DateTime End
    {
        get { return _end; }
        set
        {
            if (value < _start)
            {
                _end = _start;
                _start = value;
            }
            else
            {
                _end = value;
            }
            _range = _end - _start;
        }
    }

    public TimeSpan Range { get { return _range; } }

    public bool Equals(DateTimeRange other)
    {
        return _start == other._start && _end == other._end;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) { return false; }

        if (obj is DateTimeRange dtrObj) { return Equals(dtrObj); }

        if (obj is DateTime[] dtaObj && dtaObj.Length > 1)
        {
            return Equals(new DateTimeRange(dtaObj[0], dtaObj[1]));
        }

        if (obj is List<DateTime> dtlObj && dtlObj.Count > 1)
        {
            return Equals(new DateTimeRange(dtlObj[0], dtlObj[1]));
        }

        if (obj is Tuple<DateTime, DateTime> dttsObj)
        {
            return Equals(new DateTimeRange(dttsObj.Item1, dttsObj.Item2));
        }

        if (obj is Tuple<DateTime, TimeSpan> dttdObj)
        {
            return Equals(new DateTimeRange(dttdObj.Item1, dttdObj.Item2));
        }

        // ToDo: consider a try parse for a string

        return false;
    }

    public override int GetHashCode()
    {
        return _start.GetHashCode();
    }

    /// <summary>
    /// The default ToString that gives you "[start] to [end]" with 
    /// the date format "yyyy-MM-dd HH:mm:ss" for each date.
    /// </summary>
    public override string ToString()
    {
        var str = new StringBuilder();

        str.Append(_start.ToString("yyyy-MM-dd HH:mm:ss"));
        str.Append(" to ");
        str.Append(_end.ToString("yyyy-MM-dd HH:mm:ss"));

        return str.ToString();
    }

    /// <summary>
    /// Same as the default ToString, but allows you to choose 
    /// the DateTime format.  There's no error checking on the 
    /// format string.  It is passed right into the DateTime 
    /// ToString method, so will behave exactly as that method 
    /// would given your parameter.
    /// </summary>
    public string ToString(string dateTimeFormat)
    {
        var str = new StringBuilder();

        str.Append(_start.ToString(dateTimeFormat));
        str.Append(" to ");
        str.Append(_end.ToString(dateTimeFormat));

        return str.ToString();
    }

    /// <summary>
    /// This version of ToString will look at the dates and 
    /// times and return a shorter string if possible.  If 
    /// the dates are the same, but the times are different, 
    /// it will display the date once, and then give you the 
    /// time range.  If the times are the same but the dates 
    /// are different, it will just give the date range.  If 
    /// they're just the same, it will give the single date 
    /// and time
    /// </summary>
    /// <param name="dateFormat">
    /// Date format using y, M, and d.  Defaults to "yyyy-MM-dd".
    /// </param>
    /// <param name="timeFormat">
    /// Time format using h, H, m, s, and f. Defaults to "HH:mm:ss".
    /// </param>
    /// <returns>The most concise representation of the range is can</returns>
    public string ToStringSmart(string? dateFormat = null, string? timeFormat = null)
    {
        // clean up those nullable strings...
        string df = string.IsNullOrWhiteSpace(dateFormat) ? "yyyy-MM-dd" : dateFormat;
        string tf = string.IsNullOrWhiteSpace(timeFormat) ? "HH:mm:ss" : timeFormat;

        bool sameDate = _start.Year == _end.Year && _start.Month == _end.Month && _start.Day == _end.Day;
        bool sameTime = _start.Hour == _end.Hour && _start.Minute == _end.Minute && _start.Second == _end.Second && _start.Millisecond == _end.Millisecond;
        var result = new StringBuilder();

        if (sameDate && sameTime)
        {
            // both are the same, so just send back the date and time
            result.Append(_start.ToString(dateFormat));
            result.Append(' ');
            result.Append(_start.ToString(timeFormat));
        }
        else if (sameDate)
        {
            // the date is the same, so send back the date, and then the time range
            result.Append(_start.ToString(dateFormat));
            result.Append(", ");
            result.Append(_start.ToString(timeFormat));
            result.Append(" to ");
            result.Append(_end.ToString(timeFormat));
        }
        else if (sameTime)
        {
            // the time is the same on each day, so we'll deduce that the user wants just a date range
            result.Append(_start.ToString(dateFormat));
            result.Append(" to ");
            result.Append(_end.ToString(dateFormat));
        }
        else
        {
            // all parts are different, so just use the basic ToString that lets the user pick the format
            return ToString($"{dateFormat} {timeFormat}");
        }
        
        return result.ToString();
    }

    /// <summary>
    /// An overly complex version of ToString that lets you put in tokens to tell the 
    /// method what values you want put in.  Use an open square bracket ([) to start a 
    /// token and a closing square bracket (]) to end the token.  The first letter of 
    /// the token indicates start date with an 's', end date with an 'e', or range 
    /// with an 'r'.  Such a token without one of those letters first is ignored and 
    /// left out of the result.  The next letters inside the token are the format to 
    /// use for that component.  Remember that TimeSpans (for the range) use in general 
    /// d for days, h for hours, m for minutes, s for seconds, and f for fractions of 
    /// a second.
    /// </summary>
    /// <param name="outputForm">Example: "Starting [sMMM d] at [sHH:mm] for [rh] hours"</param>
    /// <returns>Your format string with the tokens replaced by values</returns>
    public string ToStringCustom(string outputForm)
    {
        var result = new StringBuilder();
        bool insideToken = false;
        bool needWhichVal = false;
        bool foundBadToken = false;
        char whichVal = 'x'; // s, e, or r when in token
        var formatBuild = new StringBuilder();

        foreach (char c in outputForm)
        {
            if (needWhichVal)
            {
                // this means the previous character opened up a token, so we're looking for which value to apply the upcoming format to
                switch (c)
                {
                    case 'S':
                    case 's':
                        whichVal = 's';
                        foundBadToken = false;
                        break;
                    case 'E':
                    case 'e':
                        whichVal = 'e';
                        foundBadToken = false;
                        break;
                    case 'R':
                    case 'r':
                        whichVal = 's';
                        foundBadToken = false;
                        break;
                    default:
                        whichVal = 'x';
                        foundBadToken = true;
                        break;
                }

                needWhichVal = false;
            }
            else if (insideToken)
            {
                if (c == ']')
                {
                    // we found the end of the token
                    if (!foundBadToken)
                    {
                        switch (whichVal)
                        {
                            case 's':
                                result.Append(_start.ToString(formatBuild.ToString()));
                                break;
                            case 'e':
                                result.Append(_end.ToString(formatBuild.ToString()));
                                break;
                            case 'r':
                                result.Append(_range.ToString(formatBuild.ToString()));
                                break;
                        }
                    }

                    formatBuild.Clear();
                    whichVal = 'x';
                    foundBadToken = false;
                    insideToken = false;
                    needWhichVal = false;
                }
                else
                {
                    formatBuild.Append(c);
                }
            }
            else if (c == '[')
            {
                // we're starting up a token...
                insideToken = true;
                needWhichVal = true;
                foundBadToken = false;
            }
            else
            {
                // just tack on the letter since it didn't trigger anything else
                result.Append(c);
            }
        }

        // it's possible the format ends with a token, and they didn't close it... so we'll be nice and include it
        if (insideToken && formatBuild.Length > 0 && !foundBadToken)
        {
            switch (whichVal)
            {
                case 's':
                    result.Append(_start.ToString(formatBuild.ToString()));
                    break;
                case 'e':
                    result.Append(_end.ToString(formatBuild.ToString()));
                    break;
                case 'r':
                    result.Append(_range.ToString(formatBuild.ToString()));
                    break;
            }
        }

        return result.ToString();
    }

    public int CompareTo(DateTimeRange other)
    {
        if (_start != other._start) { return DateTime.Compare(_start, other._start); }

        return DateTime.Compare(_end, other._end);
    }

    public bool OverLaps(DateTimeRange other)
    {

        if (_start <= other._end && _end >= other._start) { return true; }
        return false;
    }

    public static bool OverLaps(DateTimeRange dtr1, DateTimeRange dtr2)
    {
        return dtr1.OverLaps(dtr2);
    }

    /// <summary>
    /// INCOMPLETE - This parsing is ridiculous.  The number of ways to write a date range is just ridiculous.  
    /// The method handles some cases right now, but misses a few too.  I'm pausing finishing it hoping I can 
    /// think of a more modular way to write this method instead of the wall of code that it is currently.
    /// </summary>
    /// <param name="sourceRange"></param>
    /// <param name="result"></param>
    /// <param name="favorMonthDayOrder"></param>
    /// <returns></returns>
    public static bool TryParse(string sourceRange, [NotNullWhen(true)] out DateTimeRange? result, bool? favorMonthDayOrder = null)
    {
        result = null;
        bool favorMD = favorMonthDayOrder ?? true; // default to month/day/year, or if false day/month/year

        // first we need to try to break the source string into two pieces without
        // knowing what the middle delimiter looks like.  It might be " to "... but 
        // it could be a lot of things.  Also it could be something like... 
        // "january 1, 2025 4:00pm - 7:00pm" and so we need a stupidly complex 
        // parsing effort.
        char[] partDelims = { ' ', '-', '/', '\\', '\t', ',' };
        string[] parts = sourceRange.Split(partDelims, StringSplitOptions.RemoveEmptyEntries);
        List<DateTimePart> workerParts = new();
        foreach (string part in parts)
        {
            var possiblePart = new DateTimePart(part);
            if (possiblePart.NotAPossibleDateTimePart) { continue; }
            workerParts.Add(possiblePart);
        }

        // Now we have a list of parts that should be relevant to dates and times.
        // We need at least two parts to at least infer a range.  More parts means 
        // more chance we can do that, but if we don't have at least that many, we 
        // can return false right now...
        int workCount = workerParts.Count;
        if (workCount < 2) { return false; }

        List<int> timeIndexes = new();
        List<int> definiteMonthIndexes = new();
        List<int> monthIndexes = new();
        List<int> dayIndexes = new();
        List<int> yearIndexes = new();
        List<int> fourDigitYearIndexes = new();
        
        for (int i = 0; i < workCount; i++)
        {
            // if it's definitely a time, it's nothing else, so continue
            if (workerParts[i].SourceIsTime) { timeIndexes.Add(i); continue; }

            // if it's definitely a month because they used text like "january", it's nothing else
            if (workerParts[i].SourceIsMonthName) { definiteMonthIndexes.Add(i); continue; }

            // if it's a four digit number, it's a year
            if (workerParts[i].IsFourDigitYear) { fourDigitYearIndexes.Add(i); continue; }

            // the next checks can all happen together, so no continues left
            if (workerParts[i].IntValueIsPossibleMonth) { monthIndexes.Add(i); }
            if (workerParts[i].IntValueIsPossibleDayOfMonth) { dayIndexes.Add(i); }
            if (workerParts[i].IntValueIsPossibleYear) { yearIndexes.Add(i); }
        }

        int timeCount = timeIndexes.Count;
        int definiteMonths = definiteMonthIndexes.Count;
        int monthCount = monthIndexes.Count;
        int dayCount = dayIndexes.Count;
        int yearCount = yearIndexes.Count;
        int fourDigitYearCount = fourDigitYearIndexes.Count;

        // if we don't have times or months or years, we don't have enough information
        if (timeCount == 0 && (definiteMonths + monthCount == 0) && yearCount == 0) { return false; }

        // here are our data stores
        var parsedStartDate = new DateTimeLoader();
        var parsedEndDate = new DateTimeLoader();

        bool startYFound = false;
        bool startMFound = false;
        bool startDFound = false;
        bool startTFound = false;
        bool endYFound = false;
        bool endMFound = false;
        bool endDFound = false;
        bool endTFound = false;

        // start with definite months
        if (definiteMonths > 0)
        {
            parsedStartDate.Month = workerParts[definiteMonthIndexes[0]].IntValue ?? 1;
            startMFound = true;
        }

        if (definiteMonths > 1)
        {
            parsedEndDate.Month = workerParts[definiteMonthIndexes[1]].IntValue ?? 1;
            endMFound = true;
        }

        // next do times since those are specific too
        if (timeCount == 2)
        {
            parsedStartDate.Hour = workerParts[timeIndexes[0]].Hour ?? 0;
            parsedStartDate.Minute = workerParts[timeIndexes[0]].Minute ?? 0;
            parsedStartDate.Second = workerParts[timeIndexes[0]].Second ?? 0;
            startTFound = true;

            parsedEndDate.Hour = workerParts[timeIndexes[1]].Hour ?? 0;
            parsedEndDate.Minute = workerParts[timeIndexes[1]].Minute ?? 0;
            parsedEndDate.Second = workerParts[timeIndexes[1]].Second ?? 0;
            endTFound = true;
        }
        else if (timeCount == 1)
        {
            // a time is listed, but there's only one, so we'll use the same for both date/times
            parsedStartDate.Hour = workerParts[timeIndexes[0]].Hour ?? 0;
            parsedStartDate.Minute = workerParts[timeIndexes[0]].Minute ?? 0;
            parsedStartDate.Second = workerParts[timeIndexes[0]].Second ?? 0;
            startTFound = true;

            parsedEndDate.Hour = workerParts[timeIndexes[0]].Hour ?? 0;
            parsedEndDate.Minute = workerParts[timeIndexes[0]].Minute ?? 0;
            parsedEndDate.Second = workerParts[timeIndexes[0]].Second ?? 0;
            endTFound = true;
        }

        // next let's do years in case we got 4 digit years which can't be months or days of the month
        if (fourDigitYearCount == 2)
        {
            parsedStartDate.Year = workerParts[fourDigitYearIndexes[0]].IntValue ?? DateTime.Now.Year;
            startYFound = true;

            parsedEndDate.Year = workerParts[fourDigitYearIndexes[1]].IntValue ?? DateTime.Now.Year;
            endYFound = true;
        }
        else if (fourDigitYearCount == 1)
        {
            // in this case it is most likely that the person gave us only one year, like if they 
            // wrote May 4, 2019 10:00am to 4:00pm.  But it's also normal to write things like 
            // 2020-23 which is short for 2020-2023.  There are just too many ways to write dates.  
            // But what we can do is set it as part of the start date; remove indexes before that 
            // from the possible year indexes; and if we don't have enough values to give us an 
            // end year, we'll use the start date's year in the end date later in the code.
            parsedStartDate.Year = workerParts[fourDigitYearIndexes[0]].IntValue ?? DateTime.Now.Year;
            startYFound = true;

            for (int yi = 0; yi < fourDigitYearIndexes[0]; yi++)
            {
                yearIndexes.Remove(yi);
            }
            yearCount = yearIndexes.Count;

            if (yearCount == 0)
            {
                // no more values can be a year, so we can do this now
                parsedEndDate.Year = parsedStartDate.Year;
                endYFound = true;
            }
        }

        // we've done definite months, times, and four digit years.  Now comes the annoying part of deducing which things are which.
        // To start with, let's see if we need a month still, and if so... are there only 1 or 2 values that can be months?
        if (!startMFound)
        {
            // if we haven't found the start month, we haven't found the end month.  If there's only one number that can possibly be a month...
            if (monthCount == 1)
            {
                parsedStartDate.Month = workerParts[monthIndexes[0]].IntValue ?? 1;
                startMFound = true;
                parsedEndDate.Month = workerParts[monthIndexes[0]].IntValue ?? 1;
                endMFound = true;

                // remove that one index from the other lists
                yearIndexes.Remove(monthIndexes[0]);
                yearCount = yearIndexes.Count;
                dayIndexes.Remove(monthIndexes[0]); 
                dayCount = dayIndexes.Count;
            }
            else if (monthCount == 2)
            {
                parsedStartDate.Month = workerParts[monthIndexes[0]].IntValue ?? 1;
                startMFound = true;
                parsedEndDate.Month = workerParts[monthIndexes[1]].IntValue ?? 1;
                endMFound = true;

                // now clear those two indexes from the day and year lists
                foreach (int mi in monthIndexes)
                {
                    yearIndexes.Remove(mi);
                    yearCount = yearIndexes.Count;
                    dayIndexes.Remove(mi);
                    dayCount = dayIndexes.Count;
                }
            }
        }

        // At this point, if we haven't found months yet, it's because there are too many options, like if we have 2, 4, 6, 5, and 11, 12 (2/4/06 and 5/11/12).
        // Actually... it is possible to reach this point having parsed zero values.  I hate parsing dates.  Heck, we could have 3 values, 6 values, 4 values, 
        // or 2 values that might represent dates... man I really hate parsing dates.

        /*
        If we got here without times, we won't have times.  If we got here without years, we don't have four digit years.  If we got here without 
        months, the user gave us numeric months.  What we have left at this point is just numeric values.  I think what I need to do is look for 
        unique indexes in the years list first and if we have those, I can use those as the years.  This will only be when we have values from 32 
        on up.
        */
        if (!startYFound)
        {
            List<int> onlyYearIndexes = yearIndexes.GetItemsOnlyInThisList(dayIndexes, monthIndexes).ToList();
            int oyiCount = onlyYearIndexes.Count;

            if (oyiCount == 2)
            {
                parsedStartDate.Year = workerParts[onlyYearIndexes[0]].IntValue ?? DateTime.Now.Year;
                startYFound = true;
                parsedEndDate.Year = workerParts[onlyYearIndexes[1]].IntValue ?? DateTime.Now.Year;
                endYFound = true;
            }
            else if (oyiCount == 1)
            {
                // we're taking a risk that the only one that must be a year is listed before the end date year if there is one
                parsedStartDate.Year = workerParts[onlyYearIndexes[0]].IntValue ?? DateTime.Now.Year;
                startYFound = true;
            }
        }

        // we've tried everything I can think of.  I think our next step is if we haven't found anything yet for mdy, we have to rely on order of values
        if (!startYFound && !startMFound)
        {
            List<int> remainingInts = workerParts.Where(item => item.IntValue is not null).Select(item => item.IntValue ?? -1).ToList();
            int remainingIntCount = remainingInts.Count;

            if (remainingIntCount == 0)
            {
                // this situation means we haven't found enough to do anything useful
                return false;
            }

            if (remainingIntCount == 6)
            {
                // we think this means two dates with mdy or dmy
                parsedStartDate.Month = favorMD ? remainingInts[0] : remainingInts[1];
                startMFound = true;
                parsedStartDate.Day = favorMD ? remainingInts[1] : remainingInts[0];
                startDFound = true;
                parsedStartDate.Year = remainingInts[2];
                startYFound = true;

                parsedEndDate.Month = favorMD ? remainingInts[3] : remainingInts[4];
                endMFound = true;
                parsedEndDate.Day = favorMD ? remainingInts[4] : remainingInts[3];
                endDFound = true;
                parsedEndDate.Year = remainingInts[5];
                endYFound = true;
            }
            else if (remainingIntCount == 4)
            {
                // this is most likely md to md or dm to dm
            }

        }
        
        
        // if we have times but no month or day and we have two values left, we'll see if those can be a month and day.
        if (startTFound && endTFound && !startMFound)


        // if we got here and didn't find a year but we DID find month, day, and/or times, we can use the current year
        if (startMFound && !startYFound)
        {
            parsedStartDate.Year = DateTime.Now.Year;
            parsedEndDate.Year = parsedStartDate.Year;
            // not updating the bools because we didn't find a year... just using a default because we found other potentially useful info.
        }

        // make the end month the same as start month if we didn't find an end month
        if (startMFound && !endMFound) { parsedEndDate.Month = parsedStartDate.Month; }

        // here's where we say that if we found a start year but not an end year, just use start for both
        if (startYFound && !endYFound) { parsedEndDate.Year = parsedStartDate.Year; }


        if ((startTFound && endTFound) || (startMFound && endMFound) || (startDFound && endDFound))
        {
            // we're using this as the case for having successfully parsed a range
            result = new DateTimeRange(parsedStartDate.GetDateTime(), parsedEndDate.GetDateTime());
            return true;
        }

        // if we got all the way to the end without returning true, false is the default
        return false;
    }

    public static bool operator == (DateTimeRange left, DateTimeRange right) { return left.Equals(right); }
    public static bool operator !=(DateTimeRange left, DateTimeRange right) { return !left.Equals(right); }

}
