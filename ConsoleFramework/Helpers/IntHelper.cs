using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleFramework.Helpers;

public static class IntHelper
{
    /// <summary>
    /// This is not reliable for creating a unique StringID with an integer because 
    /// each day is separate for the generations.  If done during the same 
    /// hour and minute as the previous day, you could be trying to generate 
    /// the StringID at the same time resulting in the same StringID.  This is mostly 
    /// useful for testing where you need randomly generated items representing 
    /// things with unique StringID
    /// </summary>
    /// <returns></returns>
    public static int GenerateLikelyUniqueID()
    {
        // 9 reliably available digits with an int.  HHmmssfff
        DateTime source = DateTime.Now;
        Thread.Sleep(1); // just to be sure we won't land on the same millisecond
        return source.Hour * 10000000 + source.Minute * 100000 + source.Second * 1000 + source.Millisecond;
    }

    public static int MinimumZero(this int value, int? maxValue = null)
    {
        if (value <= 0) { return 0; } // the method minimum return value is zero, so if the value is less, just return the zero

        int max = maxValue ?? int.MaxValue;
        if (max <= 0) { return 0; } // the person for some reason limited it to the minimum
        if (max <= value) { return max; } // we know max is greater than zero so it's a valid limit.  if the value is greater than the max, just return that.

        // if we got here, the value is greater than zero and less than the maximum
        return value;
    }

    public static bool IsPrime(this int value)
    {
        if (value <= 1) { return false; }
        if (value == 2) { return true; }

        for (int n = 3; n <= value / n; n += 2)
        {
            if (value % n == 0) { return false; }
        }

        return true;
    }

    public static bool IsSquare(this int value)
    {
        if (value <= 0) { return false; }

        for (int n = 1; n <= value / n; n++)
        {
            if (n * n == value) { return true; }
        }

        return false;
    }

    public static Regex IntPattern { get; } = new Regex("[-+]?[1-9]+[0-9]*|0", RegexOptions.Compiled);

    /// <summary>
    /// Takes a string and strips out the int values in it.  Does NOT pay attention 
    /// to decimal points, so something like 12.99 would come back as 12 and 99 
    /// separately.  But is very useful for things like 2, 3, 4, 6, 9, 21 and getting 
    /// back each number regardless of the delimiter.
    /// </summary>
    public static IEnumerable<int> ParseInts(this string srcStr)
    {
        var matches = IntPattern.Matches(srcStr);
        foreach (Match match in matches)
        {
            if (!match.Success) { continue; }
            if (int.TryParse(match.Value, out int parsedInt))
            {
                yield return parsedInt;
            }
        }
    }


    public static IEnumerable<int> GetFactors(this int srcVal)
    {
        if (srcVal == 0) { yield return 0; yield break; }

        bool isNegative = srcVal < 0;
        int worker = srcVal * (isNegative ? -1 : 1);

        yield return 1;
        if (isNegative) { yield return -1; }

        for (int f = 2; f <= worker / 2; f++)
        {
            if (worker % f == 0)
            {
                yield return f;
                if (isNegative) { yield return f * -1; }
            }
        }

        yield return worker;
        if (isNegative) { yield return worker * -1; }
    }

    /// <summary>
    /// Gets you the absolute value of this integer, but there 
    /// is one exception case where this method is called on the 
    /// minimum int value because it cannot be represented as a 
    /// positive integer.
    /// </summary>
    public static int Absolute(this int val)
    {
        return val * (val < 0 ? -1 : 1);
    }

    /// <summary>
    /// If you use a negative number or zero, you'll get null.  If you use a 
    /// non-square number and don't set getClosest to true, you'll get null.  
    /// If you use a square number or have getClosest set to true, you'll get 
    /// an integer.
    /// </summary>
    /// <param name="getClosest">if set to true, will give you a root close enough.</param>
    /// <returns></returns>
    public static int? SquareRoot(this int val, bool? getClosest = null)
    {
        if (val <= 0) { return null; }

        int lowBound = 1;
        int highBound = 1;
        int worker;

        // loop through looking for exact matches and tracking close ones
        for (int r = 1; r <= val / r; r++)
        {
            worker = r * r;
            if (worker == val) { return r; }
            if (worker < val) { lowBound = r; }
            else { highBound = r;}
        }

        // get out if the user doesn't want an approximation
        if (getClosest is null || getClosest.Value == false) { return null; }

        // figure out which bound is closer as a root and return that
        if (highBound < lowBound) { highBound = lowBound + 1; }

        int lowSquare = lowBound * lowBound;
        int highSquare = highBound * highBound;

        int lowDiff = val - lowSquare;
        int highDiff = highSquare - val;

        if (lowDiff <= highDiff) { return lowBound; }
        else { return highBound; }
    }

    /// <summary>
    /// Only handles positive exponents.  Negative ones result in a value less 
    /// than 1 (fractional), so an integer return type doesn't do that.  Because 
    /// I want to avoid the null return type, I'm using an exception instead.  
    /// Also keep in mind that powers get to high values quickly so beware exceptions 
    /// for values too high for integers.
    /// </summary>
    /// <param name="exponent">a value zero or above.</param>
    /// <returns>The power of the base raised to the exponent</returns>
    public static int Power(this int val, int exponent)
    {
        if (exponent < 0) { throw new ArgumentOutOfRangeException(nameof(exponent), $"Exponent cannot be negative for this method.  Value given: {exponent}"); }

        if (exponent == 0) { return 1; } // anything to the zeroth power is one.

        if (exponent == 1) { return val; } // anything to the first power is itself.

        int worker = 1;
        for (int m = 1; m <= exponent; m++)
        {
            worker *= val;
        }
        return worker;
    }


    public static IEnumerable<int> FibonacciSequence()
    {
        yield return 0;
        yield return 1;

        int low = 0;
        int high = 1;
        int sum = 1;

        while (sum < int.MaxValue - high)
        {
            yield return sum;
            low = high;
            high = sum;
            sum = low + high;
        }

        yield return sum;
    }

    public static IEnumerable<int> BinarySequence()
    {
        int worker = 1;

        while (worker <= int.MaxValue / 2)
        {
            yield return worker;
            worker = worker * 2;
        }

        yield return worker;
    }


    public static IEnumerable<int> PrimeNumberSequence()
    {
        yield return 2;

        for (int p = 3; p < int.MaxValue / p; p += 2)
        {
            if (p.IsPrime()) { yield return p; }
        }
    }


    public static int IntegerDivide(this int srcVal, int divisor, out int remainder)
    {
        remainder = srcVal % divisor;
        return (srcVal - remainder) / divisor;
    }

    /// <summary>
    /// If you have 97 and want it rounded to 100, this method can help you.  Pass 
    /// in a multiple like 10, and whatever integer is in your variable will be 
    /// rounded to the nearest multiple of 10.  You can do this with any multiple.
    /// If you pass in 1, it will just return the number itself.  Nearest multiple 
    /// of zero is always 0.  And 2 is a bit like rounding to an even number.
    /// </summary>
    /// <param name="multiple">The multiple you want to use as the guide.</param>
    /// <returns>A number that is a multiple of your multiple parameter, closest to the source variable value.</returns>
    public static int RoundToMultiple(this int srcVal, int multiple)
    {
        if (multiple == 0) { return 0; }
        if (multiple < 0) { multiple *= -1; }
        if (multiple == 1) { return srcVal; }

        int remainder = srcVal % multiple;
        int lowBound = srcVal - remainder;
        int highBound = lowBound + (multiple * (srcVal < 0 ? -1 : 1));

        int lowDiff = (srcVal - lowBound).Absolute();
        int highDiff = (srcVal - highBound).Absolute();

        if (lowDiff < highDiff) { return lowBound; }
        else { return highBound; }
    }

    public static Dictionary<int, string> DigitWords { get; } = new()
    {
        { 0, "Zero" }, { 1, "One" }, { 2, "Two" }, { 3, "Three" }, { 4, "Four" },
        { 5, "Five" }, { 6, "Six" }, { 7, "Seven" }, { 8, "Eight" }, { 9, "Nine" }
    };


    public static int FeetPerMile { get; } = 5280;
    public static int YardsPerMile { get; } = 1760;
    public static int InchesPerFoot { get; } = 12;





}
