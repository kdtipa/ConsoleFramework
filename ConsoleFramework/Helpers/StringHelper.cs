using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace ConsoleFramework.Helpers;

public static class StringHelper
{
    public static string GenerateHorizontalRule(char displayChar, int lineLength)
    {
        if (!CharHelper.PrintableCharacters.Contains(displayChar)) { displayChar = '-'; }

        var line = new StringBuilder();
        for (int n = 1; n <= lineLength; n++) { line.Append(displayChar); }
        return line.ToString();
    }

    public static IEnumerable<string> WrapLines(string sourceString, int availableWidth, int? newLineIndentSpaces = null)
    {
        if (string.IsNullOrEmpty(sourceString)) { yield break; }

        int indentSp = newLineIndentSpaces ?? 0;
        // if we don't have room to write words, give ourselves room
        if (indentSp > availableWidth) { indentSp = availableWidth - 9; }
        // if set to below zero either by the user or our above adjustment...
        if (indentSp < 0) { indentSp = 0; }

        var line = new StringBuilder(availableWidth);
        var word = new StringBuilder();
        bool firstLine = true;
        bool prevSpace = false; // needed in case of multiple spaces

        foreach (char c in sourceString)
        {
            if (c == ' ' || c == '\t')
            {
                // first, if we have a space, we tack it on to the end of the word
                word.Append(' '); // we're ignoring tabs because they can mess up our spacing
                prevSpace = true;
            }
            else if (CharHelper.LineBreakCharacters.Contains(c))
            {
                // in this situation, we need to break the line now regardless 
                // of whether we've filled the available width
                line.Append(word);
                yield return line.ToString();
                line.Clear();
                if (!firstLine && indentSp > 0) { line.Append("".PadRight(indentSp, ' ')); }
                word.Clear();
                prevSpace = false;
                firstLine = false;
            }
            else
            {
                int trimmedWordLen = TrimmedLength(word);
                // now we know we have a character, we add to the word and figure out if we reached our limit
                if (!prevSpace)
                {
                    word.Append(c);
                    if (line.Length + trimmedWordLen > availableWidth)
                    {
                        // in this case we need to return just the line, and then set the line object to just the word and clear the word
                        yield return line.ToString();
                        line.Clear();
                        if (!firstLine && indentSp > 0) { line.Append("".PadRight(indentSp, ' ')); }
                        firstLine = false;
                    }
                }
                else
                {
                    if (line.Length + trimmedWordLen < availableWidth)
                    {
                        // if we got in here, the previous character was a space and the previous word fits on the line
                        line.Append(word);
                    }
                    else
                    {
                        // if we got in here, we don't have space left on the line so return it, clear it, and add the current word that ends in spaces
                        yield return line.ToString();
                        line.Clear();
                        if (!firstLine && indentSp > 0) { line.Append("".PadRight(indentSp, ' ')); }
                        firstLine = false;
                        line.Append(word);
                    }

                    word.Clear();
                    word.Append(c);
                    prevSpace = false;
                }
            }

            if (line.Length > 0) { yield return line.ToString(); }
        }
    }


    public static string Truncate(this string sourceString, int maxLength, bool? endWithEllipses = null)
    {
        // handle the simple edge cases that don't require extra work...
        if (string.IsNullOrEmpty(sourceString)) { return sourceString; }
        if (maxLength <= 0) { return string.Empty; }
        if (sourceString.Length <= maxLength) { return sourceString; }

        bool ewe = endWithEllipses ?? false;

        StringBuilder result = new StringBuilder(sourceString);
        result.Length = maxLength;

        if (ewe && maxLength > 1)
        {
            result[maxLength - 1] = '…';
        }

        return result.ToString();
    }

    public static string TruncateSmart(
        this string sourceString, 
        int maxLength, 
        bool? endWithEllipses = null, 
        bool? useEllipseChar = null, 
        bool? breakOnSpace = null)
    {
        // handle the simple edge cases that don't require extra work...
        if (string.IsNullOrEmpty(sourceString)) { return sourceString; }
        if (maxLength <= 0) { return string.Empty; }
        if (sourceString.Length <= maxLength) { return sourceString; }

        bool ewe = endWithEllipses ?? false;
        bool uec = useEllipseChar ?? false;
        bool bos = breakOnSpace ?? false;

        int ellipseNeeds = uec ? 1 : 3;

        StringBuilder result = new StringBuilder(sourceString);
        result.Length = maxLength; // get it to our max length right off

        // now if we need to find a space from the end, we'll try that...
        if (bos && result.TryFindIndex(' ', out int? foundIndex, false))
        {
            result.Length = foundIndex.Value;
        }

        // and if needed, put in the ellipse
        if (ewe)
        {
            if (result.Length + ellipseNeeds <= maxLength)
            {
                // in this case we can append the ellipse...
                if (uec) { result.Append('…'); }
                else { result.Append("..."); }
            }
            else
            {
                // in this case we need to overwrite the end...
                if (uec) { result[result.Length - 1] = '…'; }
                else
                {
                    int currentResultLen = result.Length;
                    for (int mod = -1; mod >= -3; mod--)
                    {
                        result[currentResultLen + mod] = '.';
                    }
                }
            }
        }

        return result.ToString();
    }

    public static bool Contains(this StringBuilder source, char searchFor)
    {
        int srcLen = source.Length;
        for (int i = 0; i < srcLen; i++)
        {
            if (source[i] == searchFor) { return true; }
        }
        return false;
    }

    public static bool Contains(this StringBuilder source, string searchFor)
    {
        if (source.Length == 0 || searchFor.Length == 0) { return false; }
        if (searchFor.Length == 1) { return source.Contains(searchFor[0]); }

        int srcLen = source.Length - searchFor.Length; // don't need to check the last few
        int searchLen = searchFor.Length;

        for (int startI = 0; startI < srcLen; startI++) 
        {
            if (source[startI] == searchFor[0])
            {
                // we have a found a possible match... so compare the rest
                for (int compareI = 1; compareI < searchLen; compareI++)
                {
                    if (source[startI + compareI] != searchFor[compareI]) { return false; }
                }

                // if we got through that internal for-loop, everything matched, so return true
                return true;
            }
        }

        return false;
    }


    public static bool TryFindIndex(
        this StringBuilder source, 
        char searchFor, 
        [NotNullWhen(true)] out int? index, 
        bool? findFirst = null)
    {
        index = null;

        if (source.Length == 0) { return false; }

        bool ff = findFirst ?? true;
        int srcLen = source.Length;

        if (ff)
        {
            // we're going from front to end
            for (int i = 0; i < srcLen; i++)
            {
                if (source[i] == searchFor) { index = i; return true; }
            }
        }
        else
        {
            // going from the end back toward the beginning
            for (int i = srcLen - 1; i >= 0; i--)
            {
                if (source[i] == searchFor) { index = i; return true; }
            }
        }

        return false;
    }


    /// <summary>
    /// There is a need for knowing how much a string builder has in it that 
    /// wouldn't be trimmed off by the string Trim method, and the idea of 
    /// running ToString on the string builder repeatedly to just use the Trim 
    /// method is a bit rough.  This method does the check without ToString, 
    /// working first from the start of the string until it find a non-white-space 
    /// character.  If it doesn't find a printable character, it returns 0, but 
    /// had to check every item (making it Order of N).  If it does find a character 
    /// it will then loop from the end of the string to find the last printable 
    /// character.  The worst case scenario is a long string with only 1 printable 
    /// character.  But this is the most efficient way to check when you have a 
    /// long string with perhaps a few spaces at the beginning and/or end.
    /// </summary>
    /// <returns>The length of the character array after white-space is trimmed.</returns>
    public static int TrimmedLength(this StringBuilder source)
    {
        bool foundChar = false;
        int firstCharI = 0;
        int lastCharI = 0;
        int srcLen = source.Length;

        if (srcLen == 0) { return 0; }

        // start from the beginning and find the first non-white space
        for (int i = 0; i < srcLen; i++)
        {
            if (!source[i].IsWhiteSpaceCharacter())
            {
                firstCharI = i;
                foundChar = true;
                break;
            }
        }

        // if we didn't find a character at all, the whole string is white space that would be trimmed, so return 0
        if (!foundChar) { return 0; }

        // if we got here, we know there's at least one non-white space, so we check starting from the end of the string
        for (int i = srcLen - 1; i > 0; i--)
        {
            if (!source[i].IsWhiteSpaceCharacter())
            {
                lastCharI = i;
                break;
            }
        }

        return lastCharI - firstCharI + 1;  // need the +1 because a length 5 string with all non-space characters would be 4 - 0 and 4 isn't the length.
    }

    /// <summary>
    /// Utility method to help with dealing with nullable 
    /// strings like from Console.Readline().
    /// </summary>
    /// <param name="defaultValue">
    /// an optional default like if you want "[n/a]" to 
    /// show up if the string was null.  Defaults to an 
    /// empty string.
    /// </param>
    /// <returns>the source string unless it was null.</returns>
    public static string NonNull(this string? source, string? defaultValue = null)
    {
        string dv = defaultValue is null ? string.Empty : defaultValue;
        return source ?? dv;
    }

    /// <summary>
    /// Allows you to test for whether a string is composed entirely of the characters 
    /// you specify.
    /// </summary>
    /// <param name="ingredients">
    /// If you specify no ingredients, it will return false.  But after that, it will 
    /// loop through the string making sure each character exists in the ingredient 
    /// list you specified.
    /// </param>
    /// <returns>True if all characters in this string exist in the ingredient list.</returns>
    public static bool IsComposedOf(this string sourceString, params char[] ingredients)
    {
        if (ingredients.Length == 0) { return false; }

        foreach (char c in sourceString)
        {
            if (!ingredients.Contains(c)) { return false; }
        }
        return true;
    }

    /// <summary>
    /// Gets you a dictionary of characters with their counts that exist in the source string.  
    /// If you provide no parameters, the contains a listing for every character in the string.
    /// </summary>
    /// <param name="characterSet">Allows you to limit the characters that exist in the dictionary.</param>
    /// <returns>
    /// Returns a dictionary of the characters that exist in the string or in your character set 
    /// with their counts within the string.
    /// </returns>
    public static Dictionary<char, int> CharacterInventory(this string sourceString, params char[] characterSet)
    {
        var result = new Dictionary<char, int>();

        if (string.IsNullOrEmpty(sourceString)) { return result; }

        bool hasCharSet = characterSet.Length > 0;

        // the main loop
        foreach (char c in sourceString)
        {
            if (!hasCharSet || characterSet.Contains(c))
            {
                // if there is no character set or the character contains the character we're on... do something about it.
                if (result.ContainsKey(c)) { result[c] += 1; }
                else { result.Add(c, 1); }
            }
        }

        // if they provided a character set to look for, we can be nice and make sure that the inventory includes zeroes...
        foreach (char cs in characterSet)
        {
            if (!result.ContainsKey(cs)) { result.Add(cs, 0); }
        }

        return result;
    }


    public static int IndexOfAny(this string sourceString, bool ignoreCase, params char[] lookFor)
    {
        // if we don't look for anything, we can't find anything
        if (lookFor.Length == 0) { return -1; }

        int srcLen = sourceString.Length;
        for (int i = 0; i < srcLen; i++)
        {
            char c = sourceString[i];
            foreach (char lf in lookFor)
            {
                if (c.CharacterEqual(lf, ignoreCase)) { return i; }
            }
        }

        
        // if we made it through the for loop without finding it, just return the -1
        return -1;
    }


    /// <summary>
    /// If you need to split up user input, you might only need a string split 
    /// on a space delimiter, but if you are trying to allow your user to provide 
    /// more complicated input like double quoted strings or the ability to name 
    /// arguments and provide their values via an equal sign, you might need something 
    /// more complicated.  This method is meant to help with that.  You provide the 
    /// complete string of the user's input, and this tries to break it up keeping 
    /// double quoted strings together while ignoring escaped double quotes within the 
    /// string.  
    /// </summary>
    /// <param name="userInput">The complete user input string</param>
    /// <param name="coalesceEquals">
    /// Do you want this method to press argument names and values together like 
    /// "arg = 15" coming back as "arg=15" instead of as three separate strings?  
    /// It defaults to false.
    /// </param>
    /// <returns>a collection of the separated strings</returns>
    public static IEnumerable<string> BreakUpUserInput(string? userInput, bool? coalesceEquals = null)
    {
        // no input means no output
        if (string.IsNullOrWhiteSpace(userInput)) { yield break; }

        // see if they want us to smoosh stuff like "arg = 4" into one string like "arg=4"
        bool cEq = coalesceEquals ?? false;

        // now get the indexes of double quotes and backslashes
        int uiLen = userInput.Length;
        List<int> dblQtIndexes = new();
        List<int> bkSlsIndexes = new();
        for (int i = 0; i < uiLen; i++)
        {
            if (userInput[i] == '"') { dblQtIndexes.Add(i); }
            else if (userInput[i] == '\\') { bkSlsIndexes.Add(i); }
        }

        bool inQuotes = false;
        List<int> quotePoints = new();

        foreach (var i in dblQtIndexes)
        {
            if (inQuotes)
            {
                // this is for those back slash escaped double quotes within a double quoted string
                if (bkSlsIndexes.Contains(i - 1)) { continue; }
                quotePoints.Add(i);
                inQuotes = false;
            }
            else
            {
                inQuotes = true;
            }
        }

        List<string> firstBreaks = new();
        bool inclQt = false;
        int previousPoint = 0;
        if (quotePoints.Count > 0)
        {
            foreach (var qp in quotePoints)
            {
                if (qp == 0) { inclQt = true; continue; }

                int strLen = qp - previousPoint;
                if (inclQt) { strLen += 1; }
                firstBreaks.Add(userInput.Substring(previousPoint, strLen));

                previousPoint = qp;
                if (inclQt) { previousPoint += 1; }

                // toggle it so that [movie titles: "casablanca", "star wars", "friday the 13th"] will alternately extend the length to include the quote and not
                inclQt = !inclQt;
            }

            // we need to tack on the end still...
            if (previousPoint < uiLen - 1)
            {
                string workStr = userInput.Substring(previousPoint);
                if (workStr.StartsWith('"') && !workStr.EndsWith('"')) { workStr += '"'; }
                firstBreaks.Add(workStr);
            }
        }
        else { firstBreaks.Add(userInput); }

        
        // at this point, we want to break up unquoted strings by space
        List<string> secondBreaks = new();
        char[] spaceDelim = { ' ' };
        foreach (string part in firstBreaks)
        {
            // if it's quoted, we just add it
            if (part.StartsWith('"')) { secondBreaks.Add(part); continue; }

            // now we break it up if it's not quoted, and add each string separately
            string[] subParts = part.Split(spaceDelim, StringSplitOptions.RemoveEmptyEntries);
            foreach (string subPart in subParts)
            {
                secondBreaks.Add(subPart);
            }
        }


        // next, if we have to squish equals parts together, we need to go through our second break list, and see if there are any equals 
        // that need to be combined with surrounding strings, or if non-quoted strings have equals that have spaces around them.
        ConcatList thirdParts = new(secondBreaks);
        if (cEq)
        {
            thirdParts.CollapseBy('=');
        }

        // now we should have the correct strings...
        foreach (string item in thirdParts.SourceItems)
        {
            yield return item;
        }
    }

    /// <summary>
    /// This method is pretty specifically to make sure there are double quotes where 
    /// they are needed in command line arguments.  For an argument to include spaces, 
    /// it needs double quotes around it.  So, if one of the actual command line inputs 
    /// comes in with its double quotes removed, this can put them back in.  But it's 
    /// important too that we pay attention to when we have names parameters that users 
    /// can type in and provide a string as the value for, like [argname = this is a string 
    /// with spaces] which we need to turn into [argname="this is a string with spaces"].
    /// </summary>
    /// <param name="equalsCharacters">
    /// By default, we're looking for the character '='.  But if you want to ignore that, 
    /// provide a different character, or provide a list of characters to accept, you can 
    /// specify them here.  If you provide something other than a printable character, it 
    /// will be ignored and we'll still at least default to '='.
    /// </param>
    /// <returns>
    /// Returns the original string if there are no spaces. Returns a string with spaces and 
    /// none of the equals characters preceeded by no spaces with double quotes bookending 
    /// the string.  And if it finds one of the equals characters preceeded by no spaces 
    /// between "words", it will treat that as an argument name and only double quote the 
    /// value, returning that.
    /// </returns>
    public static string EnsureDoubleQuotes(this string sourceString, params char[] equalsCharacters)
    {
        // if it's empty or doesn't contain spaces we don't need to do anything
        if (string.IsNullOrEmpty(sourceString) || !sourceString.Contains(' ')) { return sourceString; }

        // if it's just a string of spaces, let's put quotes around it and give it back
        if (sourceString.IsComposedOf(' ', '\t')) { return $"\"{sourceString}\""; }

        // now we need to start figuring things out
        List<char> eqChars = new();
        if (equalsCharacters.Length > 0)
        {
            foreach (char c in equalsCharacters)
            {
                if (!eqChars.Contains(c) && CharHelper.PrintableCharacters.Contains(c)) { eqChars.Add(c); }
            }
        }

        // make sure we at least have one
        if (eqChars.Count == 0) { eqChars.Add('='); }

        // get a list of characters we care about...
        var inventoryChars = new List<char>();
        inventoryChars.AddRange(eqChars);
        inventoryChars.Add(' ');
        inventoryChars.Add('"');
        var charCounts = sourceString.CharacterInventory(inventoryChars.ToArray());

        string worker = sourceString;
        while (worker.Contains(" =")) { worker = worker.Replace(" =", "="); }

        int firstSpaceIndex = worker.IndexOf(' '); // can't come back -1 because we already know there's a space
        int firstEquals = worker.IndexOfAny(true, eqChars.ToArray()); // might be -1

        if (firstSpaceIndex > firstEquals && firstEquals > -1)
        {
            // this is the ideal case where we can put a double quote after the equal sign and at the end
            worker.Insert(firstEquals + 1, "\"");
            worker += '"';
        }
        else if (firstEquals == -1)
        {
            // the case where we have spaces, but no equals, or the equals is after the first space
            worker = $"\"{worker}\"";
        }

        while (worker.Contains("\"\"")) { worker = worker.Replace("\"\"", "\""); } // just in case we duplicated existing quotes

        return worker;
    }


    public static IEnumerable<string> CleanCommandLineArgs(params string[] args)
    {
        List<string> parts = new();
        foreach (string arg in args)
        {
            if (arg.Contains(' ')) { parts.Add(arg.EnsureDoubleQuotes()); }
            else { parts.Add(arg); }
        }

        foreach (string cleanArg in BreakUpUserInput(string.Join(" ", parts), true))
        {
            yield return cleanArg;
        }
    }

    /// <summary>
    /// Mostly useful for testing.  Gives a string composed of the lower case 
    /// letters from a to z and the digits from 0 to 9.  Uses a tiny varying 
    /// thread sleep to improve randomness, so generating lots of strings may 
    /// impact performance.
    /// </summary>
    /// <param name="length">Allows range of 5 to 20, and defaults to 10.</param>
    /// <returns>A string of the specified length composed of random characters.</returns>
    public static string GenerateRandomString(int? length = null)
    {
        // fix the length
        int len = length ?? 10;
        if (len < 5) { len = 5; }
        else if (len > 20) { len = 20; }

        // set up our variables...
        int sleepCount = 11;
        Random rng = new Random();
        char[] values = {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };
        int valCount = values.Length;
        StringBuilder result = new StringBuilder();

        for (int l = 1; l <= len; l++)
        {
            int rn = rng.Next(0, valCount);
            result.Append(values[rn]);
            Thread.Sleep(sleepCount);
            sleepCount = (sleepCount % (rn + 1)) + 3;
        }

        return result.ToString();
    }


    public static string ToStringReadable(this Enum val)
    {
        // start with looking for a description that is likely the best option for a ToString
        var valField = val.GetType().GetField(val.ToString());
        if (valField is not null)
        {
            var valDescription = Attribute.GetCustomAttribute(valField, typeof(DescriptionAttribute)) as DescriptionAttribute;
            if (valDescription is not null)
            {
                return valDescription.Description;
            }
        }

        // now if that didn't already return, we'll see what the string result for ToString
        // is, and if it has underscores or pascal casing, we can break it up
        string normalToString = val.ToString();
        var result = new StringBuilder();
        bool needUpper = true;

        foreach (char c in normalToString)
        {
            if (c == '_' || c == ' ')
            {
                // not sure if spaces are possible in enum field names, but just in case someone went to a lot of trouble...
                result.Append(' ');
                needUpper = true;
                continue;
            }

            if (c.IsUpper())
            {
                result.Append($" {c}");
                needUpper = false;
                continue;
            }

            if (needUpper)
            {
                // means we had a space or underscore before this and wrote a space, so this letter needs to be upper case
                result.Append(c.ToUpper());
                needUpper = false;
                continue;
            }

            // and this is the default situation where we're just tacking the letter on
            result.Append(c);
        }


        string worker = result.ToString().Trim();
        while (worker.Contains("  ")) { worker = worker.Replace("  ", " "); }
        return worker;
    }


    private static Regex IntegerPattern { get; } = new Regex("-?[1-9]+[0-9]*|0", RegexOptions.Compiled);
    public static List<int> GetIntegersFrom(this string sourceStr)
    {
        List<int> retVal = new();

        var matches = IntegerPattern.Matches(sourceStr);
        foreach (Match match in matches)
        {
            if (int.TryParse(match.Value, out int parsedVal)) { retVal.Add(parsedVal); }
        }

        return retVal;
    }
}
