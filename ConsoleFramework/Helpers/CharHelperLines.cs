using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleFramework.Helpers;

public static class CharHelperLines
{

    /// <summary>
    /// This dictionary has the line characters used to make frames/boxes 
    /// with characters for displays like you'd find in a Console.  The 
    /// keys arebased on the directions the lines in the given symbols go.  
    /// With four digits, it is top, right, bottom, left.  0 means no line, 
    /// 1 means a single line, and 2 means a double line.  To make it easier, 
    /// you can just use the GetLineChar method from this same class.
    /// </summary>
    public static Dictionary<int, char> LineCharacters = new()
    {
        { 1010, '│' }, { 101, '─' }, { 1111, '┼' }, 
        { 1011, '┤' }, { 1110, '├' }, { 1101, '┴' }, { 111, '┬' },
        { 11, '┐' }, { 1001, '┘' }, { 110, '┌'}, { 1100, '└' },

        { 2020, '║' }, { 202, '═' }, { 2222, '╬' },
        { 2022, '╣' }, { 2220, '╠' }, { 2202, '╩' }, { 222, '╦' },
        { 22, '╗' }, { 2002, '╝' }, { 220, '╔' }, { 2200, '╚' },

        { 2121, '╫' }, { 1212, '╪' }, 
        { 1012, '╡' }, { 2021, '╢' }, { 1210, '╞' }, { 2120, '╟' },
        { 1202, '╧' }, { 2101, '╨' }, { 212, '╤' }, { 121, '╥' },
        { 21, '╖' }, { 12, '╕' }, { 2001, '╜' }, { 1002, '╛' },
        { 2100, '╙' }, { 1200, '╘' }, { 120, '╓' }, { 210, '╒' }
    };

    /// <summary>
    /// This will get you the character for a given part of a frame or 
    /// box for the special characters like ╠ which would be retrieved 
    /// with parameter values of 2, 2, 2, 0.  You can use this method 
    /// to help you draw boxes in character driven UIs like you might 
    /// find in a Console interface.  There must be at least two values 
    /// above 0, and the ones across from each other must be the same 
    /// if above zero (so top and bottom cannot be 1 and 2).  If the 
    /// values pass fail the requirements, you'll get null.
    /// </summary>
    /// <param name="up">0 for none, 1 for single line, 2 for double line on the top</param>
    /// <param name="right">0 for none, 1 for single line, 2 for double line on the right</param>
    /// <param name="down">0 for none, 1 for single line, 2 for double line on the bottom</param>
    /// <param name="left">0 for none, 1 for single line, 2 for double line on the left</param>
    /// <returns>the actual character to be displayed</returns>
    public static char? GetLineChar(int up, int right, int down, int left)
    {
        // first let's see if the numbers are all in range of 0 to 2.
        bool goodNumbers = Limit(up, 0, 2, out int lu);
        goodNumbers &= Limit(right, 0, 2, out int lr);
        goodNumbers &= Limit(down, 0, 2, out int ld);
        goodNumbers &= Limit(left, 0, 2, out int ll);
        if (!goodNumbers) { return null; }

        // next we need to be sure we have at least two values that are not zero
        int directionCount = (lu > 0 ? 1 : 0) + (lr > 0 ? 1 : 0) + (ld > 0 ? 1 : 0) + (ll > 0 ? 1 : 0);
        if (directionCount < 2) { return null; }

        // next we need to make sure the ones on the same axis are valid compared to each other.  Can't 
        // have an Up of 1 and a Down of 2 for example because the lines can't change mid symbol.  We 
        // have access to '│' (1, 1) or '║' (2, 2), but can't represent a top of 1 and a bottom of 2.
        if (lu != 0 && ld != 0 && lu != ld) { return null; } // top and bottom
        if (lr != 0 && ll != 0 && lr != ll) { return null; } // left and right

        // now we know we have a valid combination, so let's build our key for the dictionary...
        int k = (lu * 1000) + (lr * 100) + (ld * 10) + ll;
        return LineCharacters[k];
    }

    private static bool Limit(int sourceInt, int lowerLimit, int upperLimit, out int closestInRange)
    {
        // make sure limits are okay
        if (lowerLimit > upperLimit) { (lowerLimit, upperLimit) = (upperLimit, lowerLimit); }

        // ideal scenario
        if (sourceInt >= lowerLimit && sourceInt <= upperLimit)
        {
            closestInRange = sourceInt;
            return true;
        }
        else if (sourceInt < lowerLimit) { closestInRange = lowerLimit; }
        else { closestInRange = upperLimit; }
        
        return false;
    }

    /// <summary>
    /// Gets you a two dimensional array of characters with the box line 
    /// characters around the edges, with the middle filled with spaces.  
    /// This can get you an input source for writing a framed bit of text 
    /// to a Console UI.  You just need to overwrite the spaces with the 
    /// text you want.
    /// </summary>
    /// <param name="contentColumns">
    /// How many columns do you want for the content you will be adding?  
    /// The horizontal array length will be 2 higher than that to hold 
    /// the frame characters on the sides.
    /// </param>
    /// <param name="contentRows">
    /// How many rows do you want for the content you will be adding?  
    /// The vertical array length will be 2 higher than that to hold 
    /// the frame characters on the top and bottom.
    /// </param>
    /// <param name="useDoubleLines">
    /// If you pass true, it will use the double line versions of the 
    /// symbols instead of the single line ones.
    /// </param>
    /// <returns>
    /// The two dimensional array with the frame characters spanning 
    /// the corners (0,0; 0,N; N,0; and N,N).  
    /// </returns>
    public static char[,] GetSizedFrame(int contentColumns, int contentRows, bool? useDoubleLines = null)
    {
        int cols = contentColumns;
        if (cols < 1) { cols = 1; }
        else if (cols > int.MaxValue - 2) { cols = int.MaxValue - 2; }
        cols += 2; // add the columns for the frame character

        int rows = contentRows;
        if (rows < 1) { rows = 1; }
        else if (rows > int.MaxValue - 2) { rows = int.MaxValue - 2; }
        rows += 2; // add the rows for the frame character

        int l = useDoubleLines is not null && useDoubleLines == true ? 2 : 1;

        char[,] retVal = new char[cols, rows];
        char tl = GetLineChar(0, l, l, 0) ?? '+';
        char tr = GetLineChar(0, 0, l, l) ?? '+';
        char bl = GetLineChar(l, l, 0, 0) ?? '+';
        char br = GetLineChar(l, 0, l, 0) ?? '+';
        char h = GetLineChar(0, l, 0, l) ?? '-';
        char v = GetLineChar(l, 0, l, 0) ?? '|';

        for (int c = 0; c < cols; c++)
        {
            for (int r = 0; r < rows; r++)
            {
                if (c == 0 && r == 0) { retVal[c, r] = tl; } // top left corner
                else if (c == 0 && r == rows - 1) { retVal[c, r] = bl; } // bottom left corner
                else if (c == cols - 1 && r == 0) { retVal[c, r] = tr; } // top right corner
                else if (c == cols - 1 && r == rows - 1) { retVal[c, r] = br; } // bottom right
                else if (c == 0 || c == cols - 1) { retVal[c, r] = v; } // vertical edge
                else if (r == 0 || r == rows - 1) { retVal[c, r] = h; } // horizontal edge
                else { retVal[c, r] = ' '; }
            }
        }

        return retVal;

    }


    /// <summary>
    /// This method accepts a block of text as a string and a frame width, and begins returning 
    /// strings that represent what to write to a Console output in order to show the block of 
    /// text framed by lines and line wrapped to fit in the width specified.  Keep in mind this 
    /// is a string operation and it can be slow, especially with large blocks of text.
    /// </summary>
    /// <param name="sourceText">The text that you want to be use as the contents of the frame.</param>
    /// <param name="frameWidth">
    /// The number of columns wide you want the total frame to be.  The number must be a positive number 
    /// at least as long as the longest word in your text plus two.
    /// </param>
    /// <param name="useDoubleLines">If set to true, it will use the double line symbols</param>
    /// <param name="includeBufferSpace">
    /// If set to true, a space will be added at the beginning and end of each line inside the 
    /// frame character and an extra line of spaces will be before the content and after the 
    /// content so that there is a distinct buffer space between the frame borders and the 
    /// content instead of being right up against it.
    /// </param>
    /// <returns>
    /// A series of strings without line breaks that should be written to the text display 
    /// on new lines in order, to achieve the look of text within a box frame.  Note that 
    /// the font needs to be a fixed width font for the layout to work.
    /// </returns>
    public static IEnumerable<string> FrameText(string sourceText, int frameWidth, bool? useDoubleLines = null, bool? includeBufferSpace = null)
    {
        // some parameter clean up
        int frameLines = useDoubleLines is not null && useDoubleLines == true ? 2 : 1;
        bool buffer = includeBufferSpace is not null && includeBufferSpace == true;

        // expensive operation to find the longest word in the source text...
        string[] words = sourceText.Split(CharHelper.WhiteSpaceCharacters, StringSplitOptions.RemoveEmptyEntries);
        int bigWord = words.Max(w => w.Length);

        if (frameWidth < bigWord + 2) { frameWidth = bigWord + 2; }

        yield return "end of line";
    }
}
