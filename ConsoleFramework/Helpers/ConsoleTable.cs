using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleFramework.Helpers;

public class ConsoleTable
{
    public ConsoleTable() { }

    /// <summary>
    /// The list of labels to use at the top of the columns when creating the text string that will 
    /// function to display the table in a fixed width text display like the Console.  You CAN set 
    /// the column labels and then not display them by setting the ShowColumnLabels property.
    /// </summary>
    public List<string> ColumnLabels { get; set; } = new();

    /// <summary>
    /// Just tells you if there are any Column Labels set.
    /// </summary>
    public bool HasColumnLabels { get { return ColumnLabels.Count > 0; } }

    /// <summary>
    /// When getting the string to display the table, this property tells 
    /// the code whether to include the column label row.
    /// </summary>
    public bool ShowColumnLabels { get; set; } = true;


    
    /// <summary>
    /// The list of labelsto use at the left of each row when creating the text string that will 
    /// function to display the table in a fixed width text display like the Console.  You CAN set 
    /// the row labels and then not display them by setting the ShowRowLabels property.
    /// </summary>
    public List<string> RowLabels { get; set; } = new();

    /// <summary>
    /// Just tells you if there are any Row Labels set.
    /// </summary>
    public bool HasRowLabels { get { return RowLabels.Count > 0; } }

    /// <summary>
    /// Determines whether any set row labels are used when the table is 
    /// produced for display in a fixed width text environment like Console.  
    /// Defaults to false because the most common table displays have column 
    /// headers but not row headers.
    /// </summary>
    public bool ShowRowLabels { get; set; } = false;
    


    public List<List<string>> CellContents { get; set; } = new();

    /// <summary>
    /// Tells the text generation code whether to be sure there is a space to either side of the contents 
    /// or not.  Includes the width of the column header labels if those are showing.
    /// </summary>
    public bool PadCellContents { get; set; } = true;



    /// <summary>
    /// Tells the code what the outside rectangle of the table should look like
    /// </summary>
    public ConsoleTableLineStyle OutsideBorder { get; set; } = ConsoleTableLineStyle.Single;

    /// <summary>
    /// The lines between the labels and the content can be different from the rest
    /// </summary>
    public ConsoleTableLineStyle LabelSeparator { get; set; } = ConsoleTableLineStyle.Single;

    /// <summary>
    /// The lines between the cells of the table that have the main content
    /// </summary>
    public ConsoleTableLineStyle CellSeparator { get; set; } = ConsoleTableLineStyle.Single;




    /// <summary>
    /// Loops through the cell contents, giving you a simple line break (\n) at 
    /// the end of each row so you know when to switch rows in your display.  This is 
    /// mostly useful if you're going to build your own display and all you want is the 
    /// contents.  For full automation, use the GetTable() method.
    /// </summary>
    public IEnumerable<string> GetCellContents()
    {
        int rowCount = CellContents.Count;
        bool pastFirstRow = false;

        for (int r = 0; r < rowCount; r++)
        {
            // send a line break to indicate this is a new row only after the first row
            if (pastFirstRow) { yield return "\n"; }

            // figure out how many columns THIS row has
            int columnCount = CellContents[r].Count;

            // loop through the columns on this row and yield return contents
            for (int c = 0; c < columnCount; c++)
            {
                yield return CellContents[r][c];
            }

            // we're past the first row
            pastFirstRow = true;
        }
    }


    public string GetTable()
    {
        var joined = new StringBuilder();

        foreach (var text in GetTableByLines())
        {
            joined.AppendLine(text);
        }

        return joined.ToString();
    }

    public IEnumerable<string> GetTableByLines()
    {
        int rowLabelWidth = WidestRowLabel();
        int labelColCount = ColumnLabels.Count;
        int contentColCount = CellContents.Max(sub => sub.Count);
        int highestColCount = labelColCount > contentColCount ? labelColCount : contentColCount;

        List<int> colWidths = new List<int>();
        for (int col = 0; col < highestColCount; col++)
        {
            // data is set up by row, but we need to figure out the widest data within a column.
            // So, our outside loop is the column so that when we loop through rows, we're getting 
            // all the values from the one column we're on.
            int currentWidth = 0;
            if (ShowColumnLabels && ColumnLabels.Count < col) { currentWidth = ColumnLabels[col].Length; }
            for (int row = 0; row < CellContents.Count; row++)
            {
                if (CellContents[row].Count <= col) { continue; }
                int cellWidth = CellContents[row][col].Length;
                if (cellWidth > currentWidth) { currentWidth = cellWidth; }
            }
            colWidths.Add(currentWidth);
        }

        // now we have the column widths in a list.  holy crap that was a pain. 
        // but now we should be able to build text strings that build the table...
        

        // first thing we do is make the top border
        var topLeftCorner = new ConsoleTableLineChar(0, (int)OutsideBorder, (int)OutsideBorder, 0);
        var topRightCorner = new ConsoleTableLineChar(0, 0, (int)OutsideBorder, (int)OutsideBorder);
        var horizontalBorder = new ConsoleTableLineChar(0, (int)OutsideBorder, 0, (int)OutsideBorder);
        var topCellCorners = new ConsoleTableLineChar(0, (int)OutsideBorder, (int)CellSeparator, (int)OutsideBorder);

        var topLine = new StringBuilder();
        topLine.Append(topLeftCorner.Line); // first corner
        if (ShowRowLabels)
        {
            // handle that empty cell in the top left
            topLine.Append("".PadRight(rowLabelWidth, horizontalBorder.Line));
            topLine.Append(topCellCorners.Line);
        }

        // now loop through columns
        bool firstDone = false;
        foreach (int colW in colWidths)
        {
            if (firstDone) { topLine.Append(topCellCorners.Line); }
            topLine.Append("".PadRight(colW, horizontalBorder.Line));
            firstDone = true;
        }
        topLine.Append(topRightCorner.Line);
        yield return topLine.ToString();

        // next... if we're
        

    }


    private int WidestRowLabel()
    {
        int widest = 0;
        foreach (string rl in RowLabels)
        {
            if (rl.Length > widest) { widest = rl.Length; }
        }
        return widest;
    }

    private int GetColumnContentWidth(int columnIndex)
    {
        if (columnIndex < 0) { return 0; }

        int widest = 0;

        if (ColumnLabels.Count > columnIndex)
        {
            widest = ColumnLabels[columnIndex].Length;
        }

        foreach (var row in CellContents)
        {
            if (row is null) { continue; }
            if (row.Count > columnIndex && row[columnIndex].Length > widest) { widest = row[columnIndex].Length; }
        }

        return widest;
    }





    public void Clear()
    {
        ColumnLabels.Clear();
        RowLabels.Clear();
        CellContents.Clear();
    }
    public void ClearColumnLabels() { ColumnLabels.Clear(); }
    public void ClearRowLabels() { RowLabels.Clear(); }
    public void ClearContents() { CellContents.Clear(); }


    public void AddRow(params string[] rowContents)
    {
        var addVals = rowContents.ToList();
        if (addVals is null) { return; }
        CellContents.Add(addVals);
    }

    public void AddRow(List<string> rowContents)
    {
        // just to be certain we're not using references...
        var addVals = new List<string>();
        foreach (string cell in rowContents) { addVals.Add(cell); }
        CellContents.Add(addVals);
    }

    public bool RemoveRow(int rowIndex)
    {
        if (rowIndex < 0 || rowIndex >= CellContents.Count) { return false; }
        CellContents.RemoveAt(rowIndex);
        return true;
    }

    public void InsertRow(int targetIndex, params string[] rowContents)
    {
        InsertRow(targetIndex, rowContents.ToList());
    }

    public void InsertRow(int targetIndex, List<string> rowContents)
    {
        // target index doesn't exist.  Might be an exception so the caller knows what happened, but for now, this is fine.
        if (targetIndex < 0 || targetIndex > CellContents.Count) { return; }

        // if the index doesn't exist but WOULD BE the next index, just add it.
        if (targetIndex == CellContents.Count) { AddRow(rowContents); }

        // now we know a normal insert should work...
        var insertVals = new List<string>();
        foreach (string cntnt in rowContents) { insertVals.Add(cntnt); }
        CellContents.Insert(targetIndex, insertVals);
    }





    
    





}

public enum ConsoleTableLineStyle
{
    None = 0, 
    Single = 1, 
    Double = 2
}


public struct ConsoleTableLineChar : IEquatable<ConsoleTableLineChar>
{
    public ConsoleTableLineChar() { }

    public ConsoleTableLineChar(int up, int right, int down, int left)
    {
        Up = up; Right = right; Down = down; Left = left;
    }

    public ConsoleTableLineChar(char srcChar)
    {
        

        Line = srcChar;
    }

    private int _up = 0;
    private int _right = 0;
    private int _down = 0;
    private int _left = 0;
    
    public int Up
    {
        get { return _up; }
        set
        {
            if (value <= 0) { _up = 0; }
            else if (value >= 2) { _up = 2; }
            else { _up = 1; }
        }
    }

    public int Right
    {
        get { return _right; }
        set
        {
            if (value <= 0) { _right = 0; }
            else if (value >= 2) { _right = 2; }
            else { _right = 1; }
        }
    }

    public int Down
    {
        get { return _down; }
        set
        {
            if (value <= 0) { _down = 0; }
            else if (value >= 2) { _down = 2; }
            else { _down = 1; }
        }
    }

    public int Left
    {
        get { return _left; }
        set
        {
            if (value <= 0) { _left = 0; }
            else if (value >= 2) { _left = 2; }
            else { _left = 1; }
        }
    }

    private int GetCode()
    {
        return Up * 1000 + Right * 100 + Down * 10 + Left;
    }

    private bool SetByCode(int srcCode)
    {
        if (srcCode < 0 || srcCode > 2222) { return false; }

        int worker = srcCode;

        // left first 
        int l = worker % 10;
        Left = l;
        worker = (worker - l) / 10;

        // down next
        int d = worker % 10;
        Down = d;
        worker = (worker - d) / 10;

        // right next
        int r = worker % 10;
        Right = r;
        worker = (worker - r) / 10;

        // finally up
        Up = worker;

        return true;
    }

    private void Clear()
    {
        _up = 0; _down = 0; _left = 0; _right = 0;
    }

    public char Line
    {
        get { return LineCharacters[GetCode()]; }
        set
        {
            char worker = value;
            if (LineCharacterAlts.TryGetValue(value, out char parsedLine))
            {
                worker = parsedLine;
            }

            foreach (var item in LineCharacters)
            {
                if (item.Value == worker)
                {
                    SetByCode(item.Key);
                    return; // we got something so leave
                }
            }

            // we made it through the loop finding nothing, so clear it
            Clear();
        }
    }



    public bool Equals(ConsoleTableLineChar other)
    {
        return Up == other.Up && Right == other.Right && Left == other.Left && Down == other.Down;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) { return false; }

        if (obj is ConsoleTableLineChar ctlcObj) { return Equals(ctlcObj); }

        if (obj is char cObj) { return cObj == Line; }

        if (obj is int iObj) { return iObj == GetCode(); }

        return false;
    }

    public override int GetHashCode()
    {
        return GetCode();
    }

    public override string ToString()
    {
        return Line.ToString();
    }


    public static bool operator ==(ConsoleTableLineChar a, ConsoleTableLineChar b) { return a.Equals(b); }
    public static bool operator !=(ConsoleTableLineChar a, ConsoleTableLineChar b) { return !a.Equals(b); }

    public static bool operator ==(ConsoleTableLineChar a, char b) { return a.Equals(b); }
    public static bool operator !=(ConsoleTableLineChar a, char b) { return !a.Equals(b); }

    public static bool operator ==(ConsoleTableLineChar a, int b) { return a.Equals(b); }
    public static bool operator !=(ConsoleTableLineChar a, int b) { return !a.Equals(b); }

    public static string operator +(ConsoleTableLineChar a, ConsoleTableLineChar b) { return $"{a.Line}{b.Line}"; }
    public static string operator +(char a, ConsoleTableLineChar b) { return $"{a}{b.Line}"; }
    public static string operator +(string a, ConsoleTableLineChar b) { return $"{a}{b.Line}"; }





    public static Dictionary<int, char> LineCharacters = new()
    {
        { 0, ' ' },

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

    private static Dictionary<char, char> LineCharacterAlts = new()
    {
        { '+', '┼' }, { '|', '│' }, { '-', '─' }, { 't', '┬' }, { 'T', '┬' },
        { '#', '╬' }, { '=', '═' }
    };
}
