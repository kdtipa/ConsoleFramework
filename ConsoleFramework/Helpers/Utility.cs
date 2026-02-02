using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleFramework.Helpers;

public static class Utility
{
    public static bool ConsoleKeyIsNumeric(ConsoleKey srcKey, [NotNullWhen(true)] out char? numericChar)
    {
        numericChar = null;

        if (srcKey == ConsoleKey.D0 || srcKey == ConsoleKey.NumPad0)
        {
            numericChar = '0';
            return true;
        }

        if (srcKey == ConsoleKey.D1 || srcKey == ConsoleKey.NumPad1)
        {
            numericChar = '1';
            return true;
        }

        if (srcKey == ConsoleKey.D2 || srcKey == ConsoleKey.NumPad2)
        {
            numericChar = '2';
            return true;
        }

        if (srcKey == ConsoleKey.D3 || srcKey == ConsoleKey.NumPad3)
        {
            numericChar = '3';
            return true;
        }

        if (srcKey == ConsoleKey.D4 || srcKey == ConsoleKey.NumPad4)
        {
            numericChar = '4';
            return true;
        }

        if (srcKey == ConsoleKey.D5 || srcKey == ConsoleKey.NumPad5)
        {
            numericChar = '5';
            return true;
        }

        if (srcKey == ConsoleKey.D6 || srcKey == ConsoleKey.NumPad6)
        {
            numericChar = '6';
            return true;
        }

        if (srcKey == ConsoleKey.D7 || srcKey == ConsoleKey.NumPad7)
        {
            numericChar = '7';
            return true;
        }

        if (srcKey == ConsoleKey.D8 || srcKey == ConsoleKey.NumPad8)
        {
            numericChar = '8';
            return true;
        }

        if (srcKey == ConsoleKey.D9 || srcKey == ConsoleKey.NumPad9)
        {
            numericChar = '9';
            return true;
        }

        return false;
    }
}
