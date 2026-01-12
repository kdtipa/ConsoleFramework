using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleFramework;

public static class ConsoleInterfaceExtensionMethods
{
    /// <summary>
    /// When you supply a color with the string, it will output the text in that color.
    /// </summary>
    /// <param name="value">The text to output.</param>
    /// <param name="textColor">The color to show the text in.</param>
    public static void WriteColor(this IConsole cnsl, string value, ConsoleColor textColor)
    {
        var saved = cnsl.ForegroundColor;
        cnsl.ForegroundColor = textColor;

        cnsl.Write(value);

        cnsl.ForegroundColor = saved;
    }

    public static void WriteColor(this IConsole cnsl, string value, ConsoleColor textColor, ConsoleColor backColor)
    {
        var saved = cnsl.ForegroundColor;
        cnsl.ForegroundColor = textColor;

        var savedBack = cnsl.BackgroundColor;
        cnsl.BackgroundColor = backColor;

        cnsl.Write(value);

        cnsl.ForegroundColor = saved;
        cnsl.BackgroundColor = savedBack;
    }

    /// <summary>
    /// When you supply a color with the string, it will output the text in that color 
    /// and include the line break at the end.
    /// </summary>
    /// <param name="value">The text to output.</param>
    /// <param name="textColor">The color to show the text in.</param>
    public static void WriteColorLine(this IConsole cnsl, string value, ConsoleColor textColor)
    {
        var saved = cnsl.ForegroundColor;
        cnsl.ForegroundColor = textColor;

        cnsl.WriteLine(value);

        cnsl.ForegroundColor = saved;
    }

    public static void WriteColorLine(this IConsole cnsl, string value, ConsoleColor textColor, ConsoleColor backColor)
    {
        var saved = cnsl.ForegroundColor;
        cnsl.ForegroundColor = textColor;

        var savedBack = cnsl.BackgroundColor;
        cnsl.BackgroundColor = backColor;

        cnsl.Write(value);

        cnsl.ForegroundColor = saved;
        cnsl.BackgroundColor = savedBack;

        cnsl.WriteLine(); // doing this after makes sure the back color only affects the color behind the value characters
    }


    public static void WriteRainbow(this IConsole cnsl, string value, bool? randomOrder = null)
    {
        var saved = cnsl.ForegroundColor;
        var colorCount = ConsoleRainbow.Length;
        var rng = new Random();
        bool useRandom = randomOrder is not null && randomOrder.Value == true;
        int currentColor = -1;

        foreach (char c in value)
        {
            // start with changing the color
            if (useRandom) { currentColor = rng.Next(0, colorCount); }
            else
            {
                currentColor++;
                if (currentColor >= colorCount) { currentColor = 0; }
            }
            cnsl.ForegroundColor = ConsoleRainbow[currentColor];

            // then write the character
            cnsl.Write(c);
        }

        cnsl.ForegroundColor = saved;
    }

    public static void WriteRainbowLine(this IConsole cnsl, string value, bool? randomOrder = null)
    {
        cnsl.WriteRainbow(value, randomOrder);
        cnsl.WriteLine();
    }

    /// <summary>
    /// Helps you format items of text as a bullet list in the console.  It uses the window width 
    /// as a limit for line breaking and indents the next lines the same width as the bullet.  The 
    /// method will fail if the available window width is not enough to support the bullet and some 
    /// text.
    /// </summary>
    /// <param name="bulletColor">
    /// The color you would like the bullet to be.  Pass the console's ForegroundColor if you want 
    /// it to not change.
    /// </param>
    /// <param name="bulletString">
    /// This is the string you want used as the bullet.  For example " - " would put a dash at the 
    /// beginning of each bullet item.  The output will fail if the bullet is wider than a quarter 
    /// of the available window width.
    /// </param>
    /// <param name="itemColor">
    /// The color you would like the item text to be.  Pass the console's ForegroundColor if you want 
    /// it to not change.
    /// </param>
    /// <param name="bulletItems">
    /// The collection of items for the bullet list.
    /// </param>
    public static void WriteBulletList(this IConsole cnsl, 
        ConsoleColor bulletColor, string bulletString, 
        ConsoleColor itemColor, params string[] bulletItems)
    {
        // if we didn't get anything to print, just leave
        if (bulletItems.Length == 0) { return; }

        // figure out pertinent lengths
        int bulletLen = bulletString.Length;
        int availableWidth = cnsl.WindowWidth - bulletLen;

        // if the bullet takes up too much space or the available space for text is too small, just leave
        if (bulletLen > availableWidth / 4 || availableWidth < 12) { return; }

        // now we know we have at least a reasonable bit to work with, so we can write a list.
        var saved = cnsl.ForegroundColor;

        // let's make sure we're starting on our own line...
        if (cnsl.CursorLeft > 0) { cnsl.WriteLine(); }

        foreach (var item in bulletItems)
        {
            // handle the bullet
            cnsl.ForegroundColor = bulletColor;
            cnsl.Write(bulletString);

            // handle the text
            cnsl.ForegroundColor = itemColor;
            bool firstLine = true;
            foreach (var bulletLine in BreakLine(item, availableWidth))
            {
                if (!firstLine) { cnsl.Write("".PadRight(bulletLen)); }  // add the indent if needed
                cnsl.WriteLine(bulletLine);
                firstLine = false;
            }
        }

        // and last, just set the foreground color back to what it was before
        cnsl.ForegroundColor = saved;
    }

    /// <summary>
    /// Leaving this as a private function because I'm not putting anything 
    /// in to deal with existing line breaks.  Just don't pass things with 
    /// existing line breaks.
    /// </summary>
    private static IEnumerable<string> BreakLine(string sourceString, int availableWidth)
    {
        var line = new StringBuilder();
        var word = new StringBuilder();
        char[] breakableChars = { ' ', '\t' };

        foreach (char c in sourceString)
        {
            int lineLen = line.Length;
            int wordLen = word.Length;

            if (breakableChars.Contains(c))
            {
                // this is the case where we need to see if we should add the word to the line and/or return the line
                if (lineLen + wordLen <= availableWidth)
                {
                    // in this case we don't need to return anything... just add to the line
                    line.AppendLine($"{word.ToString()} ");
                    word.Clear();
                }
                else
                {
                    // in this case, adding the word to the line would make it too long, so return the line
                    yield return line.ToString();
                    line.Clear();
                    line.Append($"{word.ToString()} ");
                    word.Clear();
                }
            }
            else
            {
                // here we just add the character to the word
                word.Append(c);
            }
        }

        // now we just need to make sure there isn't anything left over in the line...
        if (line.Length > 0) { yield return line.ToString(); }
    }


    public static char HorizontalSingleLine { get; } = '─';
    public static char HorizontalDoubleLine { get; } = '═';
    public static char[] AcceptedHorizontalRuleCharacters { get; } = [
        '-', '=', '_', '~', '#', '*', '<', '^', 'v', '>', 
        '.', ':', '/', '|', '\\', 'x', 'X', 'o', 'O',
        HorizontalSingleLine, HorizontalDoubleLine
        ];

    public static char CleanHorizontalRuleCharacter(char? desiredChar)
    {
        if (desiredChar is null || !AcceptedHorizontalRuleCharacters.Contains(desiredChar.Value))
        {
            return AcceptedHorizontalRuleCharacters[0];
        }
        return desiredChar.Value;
    }

    /// <summary>
    /// If you want an easy way to write a horizontal rule in the Console, this method is it.  
    /// It accepts the character you want repeated, but will internally use the CleanHorizontalRuleCharacter 
    /// method to verify or adjust which character actually gets used.  There are properties with the 
    /// Horizontal Line characters that are single or double lines that are considered valid if you want to 
    /// use those.  The AcceptedHorizontalRuleCharacters collection has the complete list
    /// </summary>
    /// <param name="repeatedChar">The character you want to use, defaulting to '-'</param>
    /// <param name="lineColor">The optional color you'd like to use</param>
    /// <param name="lineLength">The length of line from 1 to window width, defaulting to window width.</param>
    public static void WriteHorizontalRule(this IConsole cnsl, char repeatedChar, ConsoleColor? lineColor, int? lineLength = null)
    {
        char lineChar = CleanHorizontalRuleCharacter(repeatedChar);
        int availableWidth = cnsl.WindowWidth;
        if (lineLength is not null && lineLength.Value > 0 && lineLength.Value <= availableWidth) { availableWidth = lineLength.Value; }

        if (lineColor is not null)
        {
            cnsl.WriteLine("".PadRight(availableWidth, lineChar), lineColor.Value);
        }
        else
        {
            cnsl.WriteLine("".PadRight(availableWidth, lineChar));
        }
        
    }


    //=================================================================================================================

    /// <summary>
    /// A utility method that helps you put a prompt at the beginning of user input 
    /// (doing the write for you).  Additionally it allows you to change the color 
    /// of the prompt and/or the user input.
    /// </summary>
    /// <param name="inputPrompt">The text to show before waiting for user input</param>
    /// <param name="promptColor">The color for the prompt to be</param>
    /// <param name="inputColor">The color for the user input to be</param>
    /// <returns>Returns the user's input</returns>
    public static string? ReadLine(this IConsole cnsl, string inputPrompt, ConsoleColor? promptColor = null, ConsoleColor? inputColor = null)
    {
        var saved = cnsl.ForegroundColor;

        // handle the prompt
        if (!string.IsNullOrEmpty(inputPrompt))
        {
            if (promptColor is not null) { cnsl.ForegroundColor = promptColor.Value; }
            cnsl.Write(inputPrompt);
        }

        // handle the input
        if (inputColor is not null) { cnsl.ForegroundColor = inputColor.Value; }
        else { cnsl.ForegroundColor = saved; }
        var userInput = cnsl.ReadLine();

        cnsl.ForegroundColor = saved;
        return userInput;
    }

    /// <summary>
    /// This is the list of character considered valid for masking user input.
    /// </summary>
    public static char[] ValidMaskingCharacters { get; } = ['*', 'X', 'x', '+', '-', '.', '#', '~', '?', ' '];
    /// <summary>
    /// This is the method to use to make sure the masking character chosen is valid, or 
    /// if it is not valid, it will return the default of asterisk.  It is used internally 
    /// by the ReadLine method that allows masking, so you don't need to use it in advance 
    /// if you use that method for your masking.
    /// </summary>
    public static char GetMaskChar(char? desiredMaskingChar = null)
    {
        if (desiredMaskingChar is null || !ValidMaskingCharacters.Contains(desiredMaskingChar.Value)) { return ValidMaskingCharacters[0]; }
        return desiredMaskingChar.Value;
    }

    /// <summary>
    /// This version of ReadLine allows you to make it behave like a password entry input.  
    /// With the InputMaskingOptions parameter, you can tell it to act normal; let you see 
    /// what you're typing and then overwrite what's on the screen when you hit enter; or 
    /// just mask the characters as you type them.
    /// </summary>
    /// <param name="maskOption">Clear, show as you type, hide while you type</param>
    /// <param name="maskChar">What character is used to mask your typing?</param>
    /// <param name="inputPrompt">a string preceeding the input like "password: "</param>
    /// <param name="promptColor">What color is the prompt?</param>
    /// <param name="inputColor">What color is the user input?</param>
    /// <returns></returns>
    public static string? ReadLine(this IConsole cnsl, 
        InputMaskingOptions maskOption, char maskChar, 
        string inputPrompt, ConsoleColor? promptColor = null, 
        ConsoleColor? inputColor = null)
    {
        var saved = cnsl.ForegroundColor;
        string? userInput = null;
        char mc = GetMaskChar(maskChar);

        // handle the prompt
        if (!string.IsNullOrEmpty(inputPrompt))
        {
            if (promptColor is not null) { cnsl.ForegroundColor = promptColor.Value; }
            cnsl.Write(inputPrompt);
        }

        // change color
        if (inputColor is not null) { cnsl.ForegroundColor = inputColor.Value; }
        else { cnsl.ForegroundColor = saved; }

        // now the complicated mess of getting the user's input...
        switch (maskOption)
        {
            case InputMaskingOptions.Hide:
                userInput = ReadLineHidden(cnsl, mc);
                break;
            case InputMaskingOptions.ShowWhileTyping:
                userInput = ReadLineShowWhileTyping(cnsl, mc);
                break;
            default:
                userInput = cnsl.ReadLine();
                break;
        }

        // reset the color to what it was and then return the user input
        cnsl.ForegroundColor = saved;
        return userInput;
    }

    private static string? ReadLineHidden(IConsole cnsl, char maskChar)
    {
        var returnVal = new StringBuilder();
        var listening = true;
        int startPosition = cnsl.CursorLeft;

        while (listening)
        {
            var currentCursorPosition = cnsl.CursorLeft;
            var currentInputLen = returnVal.Length;
            var userKey = cnsl.ReadKey(true);

            if (userKey.Key == ConsoleKey.Enter)
            {
                cnsl.WriteLine();
                listening = false;
            }
            else if (userKey.Key == ConsoleKey.Backspace)
            {
                // if there's nothing ahead of the cursor, just ignore the key press
                if (currentCursorPosition - startPosition == 0) { continue; }

                // remove a mask char from the end of the line
                cnsl.CursorLeft = returnVal.Length;
                cnsl.Write("\b \b");
                cnsl.CursorLeft = currentCursorPosition - 1;

                // remove character from user input at cursor location if there's anything ahead of the cursor
                if (returnVal.Length >= cnsl.CursorLeft - startPosition)
                {
                    returnVal.Remove(cnsl.CursorLeft - startPosition, 1);
                }
            }
            else if (userKey.Key == ConsoleKey.Delete)
            {
                // if the cursor is not at the end of the input, remove the character after it
                if (currentCursorPosition - startPosition > currentInputLen) { continue; }

                if (currentInputLen > 0)
                {
                    returnVal.Remove(currentCursorPosition - startPosition, 1);
                    cnsl.CursorLeft = currentInputLen;
                    cnsl.Write("\b \b");
                    cnsl.CursorLeft = currentCursorPosition - startPosition;
                }
            }
            else if (userKey.Key == ConsoleKey.LeftArrow || userKey.Key == ConsoleKey.UpArrow)
            {
                // move cursor left
                if (cnsl.CursorLeft > startPosition) { cnsl.CursorLeft--; }
            }
            else if (userKey.Key == ConsoleKey.RightArrow || userKey.Key == ConsoleKey.DownArrow)
            {
                // move cursor right
                if (cnsl.CursorLeft < returnVal.Length + startPosition) { cnsl.CursorLeft++; }
            }
            else
            {
                // ToDo: fix this so it doesn't throw an exception
                // should be a letter key... so... figure out where to add what was typed
                if (currentCursorPosition - startPosition >= currentInputLen)
                {
                    returnVal.Append(userKey.KeyChar);
                }
                else
                {
                    returnVal.Insert(currentCursorPosition - startPosition, userKey.KeyChar);
                }


                // and write mask char to screen
                int savedPosition = cnsl.CursorLeft;
                cnsl.CursorLeft = startPosition + returnVal.Length - 1;
                cnsl.Write(maskChar);
                cnsl.CursorLeft = savedPosition + 1;
                

            }

        }

        return returnVal.ToString();
    }

    private static string? ReadLineShowWhileTyping(IConsole cnsl, char maskChar)
    {
        var currentCol = cnsl.CursorLeft;
        
        // get the user input
        var userInput = cnsl.ReadLine() ?? "";
        var inputLen = userInput.Length;

        // mask the input
        cnsl.CursorTop--;
        cnsl.CursorLeft = currentCol;
        cnsl.Write("".PadRight(inputLen, maskChar));
        cnsl.CursorTop++;
        cnsl.CursorLeft = 0;

        // return
        return userInput;
    }


    public static ConsoleColor[] ConsoleRainbow { get; } = { ConsoleColor.Red, ConsoleColor.Yellow, ConsoleColor.Green, ConsoleColor.Blue, ConsoleColor.DarkMagenta };

}

public enum InputMaskingOptions
{
    Clear = 0,
    ShowWhileTyping = 1, 
    Hide = 2
}
