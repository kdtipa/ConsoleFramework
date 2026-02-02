using ConsoleFramework.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleFramework;

/// <summary>
/// This class is a collection of a few methods that improve on the 
/// normal methods for Console.  Specifically, it gives a way of 
/// having access to some of the neat methods for the IConsole 
/// interface without having to create the instance of a concrete 
/// class like ConsoleClass.  Write in color and enter passwords 
/// to your heart's content.
/// </summary>
public static class CnslExt
{
    /// <summary>
    /// Just so you can continue using CnslExt for normal writes too
    /// </summary>
    public static void Write(string? value = null)
    {
        Console.Write(value);
    }

    /// <summary>
    /// Just so you can continue using CnslExt for normal write-lines too
    /// </summary>
    /// <param name="value"></param>
    public static void WriteLine(string? value = null)
    {
        Console.WriteLine(value);
    }


    public static void WriteColor(string value, ConsoleColor textColor, ConsoleColor? backgroundColor = null)
    {
        var saved = Console.ForegroundColor;
        Console.ForegroundColor = textColor;

        var savedBG = Console.BackgroundColor;
        if (backgroundColor is not null) { Console.BackgroundColor = backgroundColor.Value; }

        Console.Write(value);

        Console.ForegroundColor = saved;
        Console.BackgroundColor = savedBG;
    }

    public static void WriteColorLine(string value, ConsoleColor textColor, ConsoleColor? backgroundColor = null)
    {
        var saved = Console.ForegroundColor;
        Console.ForegroundColor = textColor;

        var savedBG = Console.BackgroundColor;
        if (backgroundColor is not null) { Console.BackgroundColor = backgroundColor.Value; }

        Console.WriteLine(value);

        Console.ForegroundColor = saved;
        Console.BackgroundColor = savedBG;
    }

    public static void WriteRainbow(string value, bool? randomOrder = null)
    {
        var saved = Console.ForegroundColor;

        ConsoleColor[] colors =
        {
            ConsoleColor.Red, ConsoleColor.Yellow, ConsoleColor.Green, 
            ConsoleColor.Blue, ConsoleColor.Magenta
        };
        int colorCount = colors.Length;

        bool rnd = randomOrder ?? false;

        int colorIndex = 0;
        Random rcg = new Random();

        foreach (char c in value)
        {
            if (rnd) 
            {
                // get our random number
                colorIndex = rcg.Next(0, colorCount);
                Thread.Sleep(colorIndex + 1);
            }

            Console.ForegroundColor = colors[colorIndex];
            Console.Write(c);

            // increment the index whether it's random or not
            colorIndex++;
            if (colorIndex >= colorCount) { colorIndex = 0; }
        }

        Console.ForegroundColor = saved;
    }

    public static void WriteRainbowLine(string value, bool? randomOrder = null)
    {
        WriteRainbow(value, randomOrder);
        Console.WriteLine();
    }


    /// <summary>
    /// Gives you a simple way to write a bullet list which should appear 
    /// reasonably nice in a Console text display.  By default, it uses 
    /// the normal colors and a spaced dash like, " - " as the bullet.  
    /// You can adjust the list as you wish with the optional parameters.
    /// </summary>
    /// <param name="items">The list of strings you want to use.</param>
    /// <param name="itemColor">The color of the text.</param>
    /// <param name="bullet">The appearance of the bullet, defaulting to " - ".</param>
    /// <param name="bulletColor">The color of the bullet.</param>
    /// <param name="forceWidth">
    /// Console apps have options and differing defaults on handling text wrapping.  With 
    /// this, you can tell the method that you want it to take responsibility for the line 
    /// breaks.  Without a value, it will just write the items out and the Console app will 
    /// be responsible.  The benefit of using the method's wrapping is that indents will be 
    /// made to keep the left margin the same.  The benefit of the Console app is that it 
    /// will adjust to window resizing.  Choose as you see fit.
    /// </param>
    public static void WriteBulletList(
        List<string> items, 
        ConsoleColor? itemColor = null,
        string? bullet = null, 
        ConsoleColor? bulletColor = null, 
        int? forceWidth = null)
    {
        var thing = WrapText("test", 60, 3, false);

        // clean up
        var saved = Console.ForegroundColor;
        ConsoleColor ic = itemColor ?? Console.ForegroundColor;
        ConsoleColor bc = bulletColor ?? Console.ForegroundColor;
        string b = bullet ?? " - ";
        int bLen = b.Length;
        int fw = forceWidth ?? int.MaxValue;

        foreach (string item in items)
        {
            Console.ForegroundColor = bc;
            Console.Write(b);

            Console.ForegroundColor = ic;

            foreach (string line in WrapText(item, fw, bLen, false))
            {
                Console.WriteLine(line);
            }
        }

        Console.ForegroundColor = saved;
    }

    private static IEnumerable<string> WrapText(string text, int maxWidth, int? indentLen, bool? indentFirstLine = null)
    {
        // set up some basics
        int indentAmt = indentLen ?? 0;
        if (indentAmt < 0) { indentAmt = 0; }
        bool indent = indentFirstLine ?? false;
        string worker = text;
        if (indent) { worker = worker.PadLeft(indentAmt, ' '); }
        int workerLen = worker.Length;
        
        // if the max width is unreasonable or the text is shorter than it needs to be anyway...
        if (maxWidth < 1 || workerLen <= maxWidth) { yield return text; yield break; }

        // if we got here, we need line breaks, so...
        while (worker.Length > 0)
        {
            string subWorker = worker.TruncateSmart(maxWidth, false, false, true);
            int subWLen = subWorker.Length;
            yield return subWorker;

            worker = worker.Substring(subWLen).Trim();
            if (indent && worker.Length > 0) { worker = worker.PadLeft(indentAmt, ' '); }
        }
    }





    /// <summary>
    /// The normal readline method but it gives you some parameters that make it 
    /// convenient to use.
    /// </summary>
    /// <param name="inputPrompt">
    /// Tell the user what they're entering information for like writing a question 
    /// or make even just a prompt like ">> ".
    /// </param>
    /// <param name="promptColor">What color is the prompt?</param>
    /// <param name="inputColor">What color is the text the user is typing?</param>
    /// <returns>The standard user input nullable string</returns>
    public static string? ReadLine(
        string inputPrompt, 
        ConsoleColor? promptColor = null, 
        ConsoleColor? inputColor = null)
    {
        var saved = Console.ForegroundColor;

        // handle the prompt
        if (!string.IsNullOrEmpty(inputPrompt))
        {
            if (promptColor is not null) { Console.ForegroundColor = promptColor.Value; }
            Console.Write(inputPrompt);
        }

        // handle the input
        if (inputColor is not null) { Console.ForegroundColor = inputColor.Value; }
        else { Console.ForegroundColor = saved; }
        var userInput = Console.ReadLine();

        Console.ForegroundColor = saved;
        return userInput;
    }

    /// <summary>
    /// For the two masking read line methods, this method makes sure the 
    /// chosen character is an acceptable one from the InputMaskCharacters 
    /// property.  It is used internally to those methods so you don't have 
    /// to run it again before calling them, but it is public in case you 
    /// have other reasons to make sure a character is accepted as a mask.
    /// </summary>
    /// <param name="desiredMaskChar">
    /// The optional parameter where you can test a character.  If you pass 
    /// nothing, you'll get back the character in the first position of the 
    /// array of valid options, which at the moment is asterisk (*).
    /// </param>
    /// <returns>An accepted character for use in the masking methods.</returns>
    public static char GetAcceptedMaskChar(char? desiredMaskChar = null)
    {
        if (desiredMaskChar is null) { return InputMaskCharacters[0]; }

        if (InputMaskCharacters.Any(c => c == desiredMaskChar))
        {
            return desiredMaskChar.Value;
        }

        return InputMaskCharacters[0];
    }

    public static char[] InputMaskCharacters { get; } =
        [ '*', ' ', 'X', 'x', '+', '-', '.', '#', '~', '?', MaskChar_CenteredDot, MaskChar_Square ];

    public static char MaskChar_Square { get; } = '■';
    public static char MaskChar_CenteredDot { get; } = '∙';

    /// <summary>
    /// An overly complicated method under the hood, it intercepts key strokes 
    /// by the user; stores them; writes the mask character to the screen where 
    /// the user input would normally be; and then returns the clear text to the 
    /// method caller.
    /// </summary>
    /// <param name="inputPrompt"></param>
    /// <param name="promptColor"></param>
    /// <param name="inputColor"></param>
    /// <param name="maskingChar"></param>
    /// <returns></returns>
    public static string? ReadLineMasked(
        string inputPrompt, 
        ConsoleColor? promptColor = null, 
        ConsoleColor? inputColor = null, 
        char? maskingChar = null)
    {
        var saved = Console.ForegroundColor;
        char maskChar = GetAcceptedMaskChar(maskingChar);
        int promptLen = inputPrompt.Length;

        // handle the prompt
        if (!string.IsNullOrEmpty(inputPrompt))
        {
            if (promptColor is not null) { Console.ForegroundColor = promptColor.Value; }
            Console.Write(inputPrompt);
        }

        // handle the input color
        if (inputColor is not null) { Console.ForegroundColor = inputColor.Value; }
        else { Console.ForegroundColor = saved; }

        // now do the one key at a time ridiculousness...
        var userInput = new StringBuilder();
        bool keepGoing = true;

        while (keepGoing)
        {
            var userKey = Console.ReadKey(true);

            if (userKey.Key == ConsoleKey.Enter) { keepGoing = false; continue; }

            if (userKey.Key == ConsoleKey.Escape)
            {
                keepGoing = false;
                userInput.Clear();
                continue;
            }

            if (userKey.Key == ConsoleKey.Backspace) { Console.Write("\b \b"); continue; }

            if (userKey.Key == ConsoleKey.LeftArrow)
            {
                if (Console.CursorLeft > promptLen) { Console.CursorLeft--; }
                continue;
            }

            if (userKey.Key == ConsoleKey.RightArrow)
            {
                if (Console.CursorLeft < promptLen + userInput.Length) { Console.CursorLeft++; }
                continue;
            }

            if (char.IsLetterOrDigit(userKey.KeyChar))
            {
                //unfortunately, I need to know where in the input we are and insert or append
                if (Console.CursorLeft == promptLen + userInput.Length)
                {
                    // we're at the end... so do the easy case
                    userInput.Append(userKey.KeyChar);
                    Console.Write(maskChar);
                    continue;
                }

                // and if we got here, we're doing the stupid thing
                int currentCursorLocation = Console.CursorLeft;
                Console.CursorLeft = promptLen + userInput.Length - 1;
                Console.Write(maskChar);
                Console.CursorLeft = currentCursorLocation + 1;
                userInput.Insert(currentCursorLocation - promptLen, userKey.KeyChar);
            }
        }

        Console.ForegroundColor = saved;
        return userInput.ToString();
    }

    /// <summary>
    /// A variant of the read line masked method that lets the user see what they are 
    /// typing and then covers it up when they are done.  It has the obvious advantage 
    /// of making it clear what they are typing, but the disadvantage of leaving the 
    /// typed text in the input history (up and down arrow can still see what they typed).
    /// </summary>
    /// <param name="inputPrompt"></param>
    /// <param name="promptColor"></param>
    /// <param name="inputColor"></param>
    /// <param name="maskingChar"></param>
    /// <returns></returns>
    public static string? ReadLineShowWhileTyping(
        string inputPrompt,
        ConsoleColor? promptColor = null,
        ConsoleColor? inputColor = null,
        char? maskingChar = null)
    {
        var saved = Console.ForegroundColor;
        char maskChar = GetAcceptedMaskChar(maskingChar);
        int promptLen = inputPrompt.Length;

        // handle the prompt
        if (!string.IsNullOrEmpty(inputPrompt))
        {
            if (promptColor is not null) { Console.ForegroundColor = promptColor.Value; }
            Console.Write(inputPrompt);
        }

        // handle the input color
        if (inputColor is not null) { Console.ForegroundColor = inputColor.Value; }
        else { Console.ForegroundColor = saved; }

        // handle the nice simple allowance of the user typing normally...
        var userInput = Console.ReadLine() ?? string.Empty;

        // go back and cover it up
        Console.CursorTop--; // go up one line
        Console.CursorLeft = promptLen; // move to the beginning of the text to hide
        Console.Write("".PadRight(userInput.Length, maskChar)); // overwrite with the mask
        Console.CursorLeft = 0; // go back to the beginning
        Console.CursorTop++; // go down one line

        // clean up and return
        Console.ForegroundColor = saved;
        return userInput;
    }

    /// <summary>
    /// Use the MaskType enum to tell this combo read line method which type of 
    /// read line you want to use.  This means you can programmatically choose 
    /// between which type of read line you want to use, and that means run time 
    /// decisions are easier to make.
    /// </summary>
    /// <param name="maskType"></param>
    /// <param name="inputPrompt"></param>
    /// <param name="promptColor"></param>
    /// <param name="inputColor"></param>
    /// <param name="maskingChar"></param>
    /// <returns></returns>
    public static string? ReadLineSelect(
        MaskType maskType,
        string inputPrompt,
        ConsoleColor? promptColor = null,
        ConsoleColor? inputColor = null,
        char? maskingChar = null)
    {
        switch (maskType)
        {
            case MaskType.MaskWhileTyping: return ReadLineMasked(inputPrompt, promptColor, inputColor, maskingChar);
            case MaskType.ShowWhileTyping: return ReadLineShowWhileTyping(inputPrompt, promptColor, inputColor, maskingChar);
            default: return ReadLine(inputPrompt, promptColor, inputColor);
        }
    }

    /// <summary>
    /// A simple input that separates year, month, and day so we don't need to worry about 
    /// formatting or order for dates like 4/5/26.  Allows month names too including abbreviations.
    /// </summary>
    /// <param name="promptColor">The color the text of the prompt will be</param>
    /// <param name="inputColor">The color the text of the user input will be</param>
    /// <param name="yearPrompt">defaults to "year: ", but can be adjusted if you like</param>
    /// <param name="monthPrompt">defaults to "month: ", but can be adjusted if you like</param>
    /// <param name="dayPrompt">defaults to "day: ", but can be adjusted if you like</param>
    /// <returns>A DateOnly object if all three values were input successfully or null if there was a problem.</returns>
    public static DateOnly? ReadDate(
        ConsoleColor? promptColor = null, 
        ConsoleColor? inputColor = null, 
        string? yearPrompt = null, 
        string? monthPrompt = null, 
        string? dayPrompt = null)
    {
        var savedColor = Console.ForegroundColor;
        int maxPromptLen = 12;

        string yp = "year: ";
        if (!string.IsNullOrEmpty(yearPrompt)) { yp = yearPrompt; }
        if (yp.Length > maxPromptLen) { yp = yp.Substring(0, maxPromptLen); }

        string mp = "month: ";
        if (!string.IsNullOrEmpty(monthPrompt)) { mp = monthPrompt; }
        if (mp.Length > maxPromptLen) { mp = mp.Substring(0, maxPromptLen); }

        string dp = "day: ";
        if (!string.IsNullOrEmpty(dayPrompt)) { dp = dayPrompt; }
        if (dp.Length > maxPromptLen) { dp = dp.Substring(0, maxPromptLen); }

        // get year first so we can know if it's a leap year
        // ===========================================================================
        int yr = -1;
        Console.ForegroundColor = promptColor ?? savedColor;
        Console.Write(yp);

        Console.ForegroundColor = inputColor ?? savedColor;
        var yearInput = Console.ReadLine();
        if (int.TryParse(yearInput, out int parsedYear))
        {
            if (parsedYear >= 0 && parsedYear <= 99)
            {
                yr = DateTimeHelper.GetClosestFourDigitYear(parsedYear) ?? -1;
            }
            else if (parsedYear > 99)
            {
                yr = parsedYear;
            }
        }

        if (yr == -1) { return null; }


        // get month next so we can know what days are valid
        // ===========================================================================
        int mn = -1;
        Console.ForegroundColor = promptColor ?? savedColor;
        Console.Write(mp);

        Console.ForegroundColor = inputColor ?? savedColor;
        var monthInput = Console.ReadLine();
        if (int.TryParse(monthInput, out int parsedMonth))
        {
            if (parsedMonth >= 1 && parsedMonth <= 12) { mn = parsedMonth; }
        }
        else if (DateTimeHelper.TryParseMonth(monthInput ?? string.Empty, out var parsedStrMonth, out _) && parsedStrMonth is not null)
        {
            mn = parsedStrMonth ?? -1;
        }

        if (mn == -1) { return null; }


        // finally get the day of the month
        // ===========================================================================
        int dy = -1;
        Console.ForegroundColor = promptColor ?? savedColor;
        Console.Write(dp);

        Console.ForegroundColor = inputColor ?? savedColor;
        var dayInput = Console.ReadLine();
        var maxDay = DateTimeHelper.DaysInMonth(mn, yr);
        if (int.TryParse(dayInput, out int parsedDay) && parsedDay >= 1 && parsedDay <= maxDay)
        {
            dy = parsedDay;
        }

        if (dy == -1) { return null; }


        // now we know we have a good date, so let's make sure the color is back to normal and then return.
        Console.ForegroundColor = savedColor;
        return new DateOnly(yr, mn, dy);

    }


    public static int? ReadInt(
        string? inputPrompt = null, 
        ConsoleColor? promptColor = null, 
        ConsoleColor? inputColor = null)
    {
        ConsoleColor saved = Console.ForegroundColor;

        if (!string.IsNullOrEmpty(inputPrompt))
        {
            Console.ForegroundColor = promptColor ?? saved;
            Console.Write(inputPrompt);
        }

        int firstCol = Console.CursorLeft;  // need to know where input starts for deletions and arrow keys
        bool keepListening = true;
        var userInput = new StringBuilder();
        Console.ForegroundColor = inputColor ?? saved;

        while (keepListening)
        {
            var userKey = Console.ReadKey(true);

            if (userKey.Key == ConsoleKey.Escape)
            {
                userInput.Clear();
                keepListening = false;
            }
            else if (userKey.Key == ConsoleKey.Enter)
            {
                keepListening = false;
            }
            else if (userKey.Key == ConsoleKey.Backspace)
            {
                if (Console.CursorLeft > firstCol)
                {
                    // need to remove the character from the stringbuilder and from the console display
                    int currentPos = Console.CursorLeft;
                    int strBldIndex = currentPos - firstCol - 1;
                    userInput.Remove(strBldIndex, 1);
                    Console.CursorLeft = firstCol;
                    Console.Write($"{userInput.ToString()} ");
                    Console.CursorLeft = currentPos - 1;
                }
            }
            else if (userKey.Key == ConsoleKey.Delete)
            {
                if (firstCol + userInput.Length > Console.CursorLeft)
                {
                    // need to remove the character from the stringbuilder and from the console display
                    int currentPos = Console.CursorLeft;
                    int strBldIndex = currentPos - firstCol;
                    userInput.Remove(strBldIndex, 1);
                    Console.CursorLeft = firstCol;
                    Console.Write($"{userInput.ToString()} ");
                    Console.CursorLeft = currentPos;
                }
            }
            else if (userKey.Key == ConsoleKey.OemMinus || userKey.Key == ConsoleKey.Subtract)
            {
                if (Console.CursorLeft == firstCol)
                {
                    if (userInput.Length == 0) { userInput.Append('-'); Console.Write('-'); }
                    else if (userInput[0] != '-')
                    {
                        userInput.Insert(0, '-');
                        Console.Write(userInput.ToString());
                        Console.CursorLeft = firstCol + 1;
                    }
                }
            }
            else if (Utility.ConsoleKeyIsNumeric(userKey.Key, out var numChar))
            {
                if (Console.CursorLeft < firstCol + userInput.Length)
                {
                    // need an insert
                    userInput.Insert(Console.CursorLeft - firstCol, numChar);
                    int currentPos = Console.CursorLeft;
                    Console.CursorLeft = firstCol;
                    Console.Write(userInput.ToString());
                    Console.CursorLeft = currentPos + 1;
                }
                else
                {
                    // need an append
                    userInput.Append(numChar);
                    Console.Write(numChar);
                }
                
            }
            else if (userKey.Key == ConsoleKey.LeftArrow)
            {
                if (Console.CursorLeft > firstCol)
                {
                    Console.CursorLeft -= 1;
                }
            }
            else if (userKey.Key == ConsoleKey.RightArrow)
            {
                if (Console.CursorLeft < firstCol + userInput.Length)
                {
                    Console.CursorLeft += 1; 
                }
            }
        }


        Console.WriteLine();
        Console.ForegroundColor = saved;

        if (userInput.Length > 0 && int.TryParse(userInput.ToString(), out int parsedInput))
        {
            return parsedInput;
        }
        return null;
    }

    




    public enum MaskType
    {
        None = 0,
        MaskWhileTyping = 1, 
        ShowWhileTyping = 2
    }
}
