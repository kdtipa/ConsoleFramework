using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleFramework.Helpers;

public struct RomanNumeral : IEquatable<RomanNumeral>, IComparable<RomanNumeral>
{
    public RomanNumeral() { }

    public RomanNumeral(int initialValue)
    {
        IntValue = initialValue;
    }

    public RomanNumeral(string initialValue)
    {
        if (TryParseRNStrToInt(initialValue, out int? parsedVal))
        {
            IntValue = parsedVal.Value;
        }
        else
        {
            throw new ArgumentException("Unable to parse the given value as a roman numeral.");
        }
    }




    private int _intValue = 1;

    public int IntValue
    {
        get { return _intValue; } 
        set
        {
            if (value < MinValue || value > MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"Cannot express {value} as a Roman Numeral.  Must be {MinValue} to {MaxValue}.");
            }

            _intValue = value;
        }
    }

    public static int MaxValue { get; } = 4999;
    public static int MinValue { get; } = 1;

    public static bool IsRomanNumeralValue(int srcValue)
    {
        return srcValue < MaxValue && srcValue > MinValue;
    }


    public string Value
    {
        get { return CalcSubtractiveString(_intValue); }
        set
        {
            if (TryParseRNStrToInt(value, out int? parsedVal))
            {
                _intValue = parsedVal.Value;
            }
            else
            {
                throw new ArgumentException("Invalid Roman Numeral");
            }
        }
    }

    public string ValueAdditive
    {
        get { return CalcAdditiveString(_intValue); }
        set
        {
            if (TryParseRNStrToInt(value, out int? parsedVal))
            {
                _intValue = parsedVal.Value;
            }
            else
            {
                throw new ArgumentException("Invalid Roman Numeral");
            }
        }
    }

    public override string ToString()
    {
        return Value;
    }

    public override int GetHashCode()
    {
        return _intValue;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) { return false; }

        if (obj is RomanNumeral rnObj) { return Equals(rnObj); } 

        if (obj is int iObj) { return _intValue == iObj; }

        return false;
    }

    public bool Equals(RomanNumeral other)
    {
        return _intValue == other._intValue;
    }


    public int CompareTo(RomanNumeral other)
    {
        return _intValue.CompareTo(other._intValue);
    }



    public static Dictionary<char, int> RomanNumeralDigits { get; } = new()
    {
        { 'I', 1 }, { 'i', 1 }, { 'V', 5 }, { 'v', 5 },
        { 'X', 10 }, { 'x', 10 }, { 'L', 50 }, { 'l', 50 },
        { 'C', 100 }, { 'c', 100 }, { 'D', 500 }, { 'd', 500 },
        { 'M', 1000 }, { 'm', 1000 }
    };

    public static Dictionary<int, char> RomanNumeralDigitFromInt { get; } = new()
    {
        { 1, 'I' }, { 5, 'V' }, { 10, 'X' }, { 50, 'L' }, { 100, 'C' }, { 500, 'D' }, { 1000, 'M' }
    };

    public static Dictionary<int, string> CalcSubtractive { get; } = new()
    {
        { 1000, "M" }, { 900, "CM" }, { 500, "D" }, { 400, "CD" },
        { 100, "C" }, { 90, "XC" }, { 50, "L" }, { 40, "XL" },
        { 10, "X" }, { 9, "IX" }, { 5, "V" }, { 4, "IV" },
        { 1, "I" }
    };

    public static string CalcSubtractiveString(int intVal, bool? useLowerCase = null)
    {
        if (!IsRomanNumeralValue(intVal)) { return string.Empty; }

        int worker = intVal;
        var result = new StringBuilder();
        foreach (var item in CalcSubtractive)
        {
            while (worker > item.Key)
            {
                result.Append(item.Value);
                worker -= item.Key;
            }
        }

        if (useLowerCase is not null && useLowerCase.Value == true) { return result.ToString().ToLower(); }
        return result.ToString();
    }

    public static Dictionary<int, char> CalcAdditive { get; } = new()
    {
        { 1000, 'M' }, { 500, 'D' },
        { 100, 'C' }, { 50, 'L' },
        { 10, 'X' }, { 5, 'V' },
        { 1, 'I' }
    };

    public static string CalcAdditiveString(int intVal, bool? useLowerCase = null)
    {
        if (!IsRomanNumeralValue(intVal)) { return string.Empty; }

        int worker = intVal;
        var result = new StringBuilder();
        foreach (var item in CalcAdditive)
        {
            while (worker > item.Key)
            {
                result.Append(item.Value);
                worker -= item.Key;
            }
        }

        if (useLowerCase is not null && useLowerCase.Value == true) { return result.ToString().ToLower(); }
        return result.ToString();
    }


    public static bool ValidRomanNumeralString(string sourceString)
    {
        if (string.IsNullOrWhiteSpace(sourceString)) { return false; }

        string worker = sourceString.Trim().ToUpper();

        // this is to keep track that these letters only appear in the string once, or it's not okay
        Dictionary<char, int> solitaryDigits = new() 
        {
            { 'D', 0 }, { 'L', 0 }, { 'V', 0 }
        };

        // make sure the letters are valid, and count the solitary characters
        foreach (char c in worker)
        {
            if (!CalcAdditive.Values.Contains(c)) { return false; }

            if (solitaryDigits.ContainsKey(c)) { solitaryDigits[c]++; }
        }

        // if there are 2 V characters for example, it's invalid, like "XVIV" which should be "XIX"
        if (solitaryDigits.Values.Any(v => v > 1)) { return false; }

        // now all that's left is making sure it's in the right order.  Highest values down to lowest
        var vals = GetIntValues(DelimitedIntVals(worker));

        if (!IsDescending(vals)) { return false; }

        // there are still some things that won't be caught yet.  IIIII would work for example
        return true;
    }


    private static bool IsDescending(List<int> vals)
    {
        if (vals.Count <= 1) { return true; }

        int currentVal = vals[0];
        foreach (int val in vals)
        {
            if (currentVal < val) { return false; }
            currentVal = val;
        }

        return true;
    }


    private static List<int> GetIntValues(string delimitedIntVals)
    {
        char[] delims = { '|', ' ', ':', '(', ')' };
        string[] parts = delimitedIntVals.Split(delims, StringSplitOptions.RemoveEmptyEntries);
        var result = new List<int>();
        foreach (var part in parts)
        {
            if (int.TryParse(part, out int parsedVal))
            {
                result.Add(parsedVal);
            }
        }
        return result;
    }

    private static string DelimitedIntVals(string sourceString)
    {
        string worker = sourceString.Trim().ToUpper();
        foreach (var item in ReplaceVals)
        {
            worker = worker.Replace(item.Key, item.Value);
        }

        return worker;
    }

    private static Dictionary<string, string> ReplaceVals { get; } = new()
    {
        { "CM", "|900|" }, { "CD", "|400|" }, { "XC", "|90|" }, { "XL", "|40|" }, { "IX", "|9|" }, { "IV", "|4|" },
        { "M", "|1000|" }, { "D", "|500|" }, { "C", "|100|" }, { "L", "|50|" }, { "X", "|10|" }, { "V", "|5|" }, { "I", "|1|" }
    };

    public static bool TryParseRNStrToInt(string sourceString, [NotNullWhen(true)] out int? result)
    {
        result = null;
        if (string.IsNullOrWhiteSpace(sourceString)) { return false; }

        var valList = GetIntValues(DelimitedIntVals(sourceString));
        if (valList.Count == 0) { return false; }

        result = valList.Sum();
        return true;
    }

    public static bool TryParse(string sourceString, [NotNullWhen(true)] out RomanNumeral? result)
    {
        if (TryParseRNStrToInt(sourceString, out var parsedInt) && parsedInt is not null)
        {
            result = new RomanNumeral(parsedInt.Value);
            return true;
        }

        result = null;
        return false;
    }


    public static IEnumerable<RomanNumeral> EnumerateRomanNumerals()
    {
        for (int i = MinValue; i <= MaxValue; i++)
        {
            yield return new RomanNumeral(i);
        }
    }


}
