using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleFramework.Helpers;

/// <summary>
/// This class is meant to make it simpler to give the user a prompt 
/// with an option list above it to choose from, allowing arrow keys 
/// to be used to make the selection, typing to find the right selection, 
/// and the enter key to make the selection.  This was fun to write, and 
/// hopefully someone finds it helpful.
/// </summary>
public class OptionsHelper<T> where T : IEquatable<T>, new()
{
    public OptionsHelper(IConsole targetConsole, string? title = null, bool? showInstructions = null)
    {
        TargetConsole = targetConsole;

        Title = title ?? string.Empty;
        ShowInstructions = showInstructions ?? false;
    }

    public OptionsHelper(IConsole targetConsole, params OptionsHelperItem<T>[] options)
    {
        TargetConsole = targetConsole;
        foreach (var item in options) { Options.Add(item); }
    }

    public OptionsHelper(IConsole targetConsole, 
        string title, 
        ConsoleColor titleTextColor, 
        ConsoleColor titleBackgroundColor, 
        bool showInstructions, 
        ConsoleColor showInstructionsTextColor, 
        ConsoleColor showInstructionsBackgroundColor, 
        List<OptionsHelperItem<T>> options, 
        ConsoleColor optionsTextColor, 
        ConsoleColor optionsBackgroundColor, 
        ConsoleColor optionsTextHighlight, 
        ConsoleColor optionsBackgroundHighlight)
    {
        TargetConsole = targetConsole;
        Title = title;
        TitleTextColor = titleTextColor;
        TitleBackgroundColor = titleBackgroundColor;
        ShowInstructions = showInstructions;
        ShowInstructionsTextColor = showInstructionsTextColor;
        ShowInstructionsBackgroundColor = showInstructionsBackgroundColor;
        Options = options;
        OptionsTextColor = optionsTextColor;
        OptionsBackgroundColor = optionsBackgroundColor;
        OptionsTextHighlight = optionsTextHighlight;
        OptionsBackgroundHighlight = optionsBackgroundHighlight;
    }

    public IConsole TargetConsole { get; set; }

    /// <summary>
    /// This string is displayed at the top of the list, likely most useful 
    /// for letting the user know what list they ended up in.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    public ConsoleColor TitleTextColor { get; set; } = ConsoleColor.White;
    public ConsoleColor TitleBackgroundColor { get; set; } = ConsoleColor.Black;

    /// <summary>
    /// Gives brief instructions when the list is displayed on how to make 
    /// a selection or cancel the selection.  When displayed, it goes between 
    /// the title and the list.
    /// </summary>
    public bool ShowInstructions { get; set; } = false;

    public ConsoleColor ShowInstructionsTextColor { get; set; } = ConsoleColor.DarkGray;
    public ConsoleColor ShowInstructionsBackgroundColor { get; set; } = ConsoleColor.Black;

    private static string InstructionsText { get; set; } = "type to make a prediction; up and down arrow to select; enter key to go; escape key to cancel";


    /// <summary>
    /// The list of options you want to offer
    /// </summary>
    public List<OptionsHelperItem<T>> Options { get; set; } = new();

    /// <summary>
    /// Some text to display before the option.  Defaults to none, 
    /// but could be some spaces to make an indent; a dash to make 
    /// a bulleted list; or something you think nicely indicates 
    /// each option.
    /// </summary>
    public string OptionPrefix { get; set; } = string.Empty;

    public ConsoleColor OptionsTextColor { get; set; } = ConsoleColor.Gray;

    public ConsoleColor OptionsBackgroundColor { get; set; } = ConsoleColor.Black;

    public ConsoleColor OptionsTextHighlight { get; set; } = ConsoleColor.White;

    public ConsoleColor OptionsBackgroundHighlight { get; set; } = ConsoleColor.DarkGreen;


    /// <summary>
    /// The prompt that shows at the bottom of the list for the user to type or use arrows.
    /// </summary>
    public string InputPrompt { get; set; } = string.Empty;

    public ConsoleColor InputPromptColor { get; set; } = ConsoleColor.Gray;

    public ConsoleColor InputPromptBackColor { get; set; } = ConsoleColor.Black;

    public ConsoleColor InputTextColor { get; set; } = ConsoleColor.Gray;

    public ConsoleColor InputTextBackColor { get; set; } = ConsoleColor.Black;


    /// <summary>
    /// This is the primary method on this class.  Use it to display the list to the user 
    /// and allow the user to interact with it and make a selection.  If it comes back as 
    /// false, it means the user cancelled the selection and the out parameter will come 
    /// back as null.  If true though, the out parameter will contain the user's selection.
    /// </summary>
    public bool RunOptionList([NotNullWhen(true)] out OptionsHelperItem<T>? selection)
    {
        // set the out parameter to the default
        selection = null;
        
        // get a fresh line if needed
        if (TargetConsole.CursorLeft != 0) { TargetConsole.WriteLine(); }

        // write the initial option list...
        if (!string.IsNullOrEmpty(Title)) { TargetConsole.WriteColorLine(Title, TitleTextColor, TitleBackgroundColor); }
        if (ShowInstructions) { TargetConsole.WriteColorLine(InstructionsText, ShowInstructionsTextColor, ShowInstructionsBackgroundColor); }

        int firstOptionLine = TargetConsole.CursorTop;
        foreach (var option in Options)
        {
            TargetConsole.WriteColorLine($"{OptionPrefix}{option.Text}", OptionsTextColor, OptionsBackgroundColor);
        }
        int lastOptionLine = TargetConsole.CursorTop - 1; // -1 since it was a writeline and we're on a new line

        // set up a couple useful variables
        int selectedIndex = -1;
        int highlightedLine = -1;
        int optionCount = Options.Count;

        int promptLen = InputPrompt.Length; // just we don't have to rewrite it when the use scrolls with the arrows
        var userText = new StringBuilder();
        TargetConsole.WriteColor(InputPrompt, InputPromptColor, InputPromptBackColor);
        int inputLine = TargetConsole.CursorTop; // need this for which line to highlight and which line to un-highlight

        bool keepListening = true;
        
        while (keepListening)
        {
            int userTextLen = userText.Length; // used in a fair number of the tests, so just get it once

            // get a key from the user and don't forget to interrupt
            var userInput = TargetConsole.ReadKey(true);

            if (userInput.Key == ConsoleKey.Escape) 
            {
                // user cancelled
                TargetConsole.WriteLine(); // need to move the cursor before calling process gets back control
                selection = null; 
                return false; 
            }

            if (userInput.Key == ConsoleKey.Enter)
            {
                TargetConsole.WriteLine(); // need to move the cursor before calling process gets back control
                keepListening = false; // not really important since we're just returning... but I want to

                if (selectedIndex == -1) 
                {
                    // user hit enter but didn't choose an option
                    selection = null; 
                    return false; 
                }

                // an option is selected and we can return the selection and true
                selection = Options[selectedIndex];
                return true;
            }


            if (userInput.Key == ConsoleKey.Backspace) 
            {
                if (TargetConsole.CursorLeft > promptLen)
                {
                    TargetConsole.Write("\b \b"); // moves the cursor; writes a space; moves the cursor again
                    if (userText.Length > 0) { userText.Length -= 1; }
                    if (userText.Length == 0) 
                    { 
                        selectedIndex = -1;
                        unhighlight(highlightedLine, firstOptionLine);
                        highlightedLine = -1;
                    }
                    
                }
                continue;
            }

            if (userInput.Key == ConsoleKey.Delete)
            {
                if (TargetConsole.CursorLeft < promptLen + userTextLen)
                {
                    // update the user text so we can use it to overwrite the displayed text
                    int savedIndex = TargetConsole.CursorLeft;
                    int deleteIndex = savedIndex - promptLen;
                    userText.Remove(deleteIndex, 1);
                    TargetConsole.CursorLeft = promptLen;
                    TargetConsole.WriteColor(userText.ToString() + ' ', InputTextColor, InputTextBackColor);
                    TargetConsole.CursorLeft = savedIndex;

                    if (userText.Length == 0)
                    {
                        selectedIndex = -1;
                        unhighlight(highlightedLine, firstOptionLine);
                        highlightedLine = -1;
                    }
                }
                
                continue;
            }

            if (userInput.Key == ConsoleKey.LeftArrow) 
            {
                if (TargetConsole.CursorLeft > promptLen) { TargetConsole.CursorLeft--; }
                continue;
            }

            if (userInput.Key == ConsoleKey.RightArrow) 
            { 
                if (TargetConsole.CursorLeft < promptLen + userText.Length) { TargetConsole.CursorLeft++; }
                continue; 
            }

            if (userInput.Key == ConsoleKey.Home) 
            {
                TargetConsole.CursorTop = inputLine; // just to be sure it's there
                TargetConsole.CursorLeft = promptLen;
                continue;
            }

            if (userInput.Key == ConsoleKey.End)
            {
                TargetConsole.CursorTop = inputLine; // just to be sure it's there
                TargetConsole.CursorLeft = promptLen + userTextLen;
                continue;
            }

            if (userInput.Key == ConsoleKey.PageUp || userInput.Key == ConsoleKey.PageDown) { continue; } // we're not using these keys

            if (userInput.Key == ConsoleKey.Tab)
            {
                // fun one.  It requires that there be user input text.  If you hit tab, it will see 
                // if it can match one of the option texts to the user input, and if only one matches, 
                // it will select that one.  
            }

            // handle up and down arrow keys
            if (userInput.Key == ConsoleKey.DownArrow || userInput.Key == ConsoleKey.UpArrow)
            {
                // get the current selected index
                // up arrow indicated a lower index in the option list, so -1
                selectedIndex += userInput.Key == ConsoleKey.UpArrow ? -1 : 1;
                if (selectedIndex < -1) { selectedIndex = optionCount - 1; } // -1 is nothing selected, so if we hit up arrow from there we're going to the bottom item
                else if (selectedIndex > optionCount - 1) { selectedIndex = -1; } // if we're at the bottom of the list and arrow down, we have to select the prompt line -1

                // un-highlight if there is a current highlighted line
                unhighlight(highlightedLine, firstOptionLine);

                // now that we've cleared the previously highlighted line, we'll figure out what the new currently highlighted line should be
                highlightedLine = highlight(firstOptionLine, selectedIndex);
                if (selectedIndex != -1)
                {
                    userText.Clear();
                    userText.Append(Options[selectedIndex].Text);
                }

                // overwrite the user's input with the option text because they used an arrow to get here
                TargetConsole.CursorLeft = promptLen;
                TargetConsole.CursorTop = inputLine; // this is so important so that text always gets written here if the user types characters


                string selectedText = string.Empty;
                bool resetCursor = true;
                if (selectedIndex >= 0) 
                { 
                    selectedText = Options[selectedIndex].Text;
                    resetCursor = false;
                }
                selectedText = selectedText.PadRight(userText.Length, ' ');
                TargetConsole.WriteColor(selectedText, InputTextColor, InputTextBackColor);
                if (resetCursor)
                {
                    // we just overwrote the previous text with blank space, but the cursor should be at the prompt...
                    TargetConsole.CursorLeft = promptLen;
                }

                continue;
            } // end up and down arrow keys


            // ToDo: something is broken where the cursor ends up at the end of the spaces overwrite line... 
            // if we got here, it should mean the user is typing a character.  If there's a bug, it's probably because we didn't handle something before we got here
            char theCharacter = userInput.KeyChar;
            // add the character to the user text variable we're using
            int targetIndex = TargetConsole.CursorLeft - promptLen;
            if (targetIndex >= userTextLen) { userText.Append(theCharacter); }
            else { userText.Insert(targetIndex, theCharacter); }
            TargetConsole.WriteColor(theCharacter.ToString(), InputTextColor, InputTextBackColor); // write the character whereever the cursor is
        }

        // if we somehow got out of the while loop without setting our selection and returning already...
        selection = null;
        return false;
    } // end RunOptionList

    private void unhighlight(int current, int first)
    {
        if (current == -1) { return; }
        TargetConsole.CursorLeft = 0;
        int savedLineNum = TargetConsole.CursorTop;
        TargetConsole.CursorTop = current;
        TargetConsole.WriteColor($"{OptionPrefix}{Options[current - first].Text}", OptionsTextColor, OptionsBackgroundColor);
        TargetConsole.CursorTop = savedLineNum;
    }

    private int highlight(int first, int selectedIndex)
    {
        if (selectedIndex == -1) { return -1; }
        TargetConsole.CursorLeft = 0;
        int savedLineNum = TargetConsole.CursorTop;
        TargetConsole.CursorTop = first + selectedIndex;
        TargetConsole.WriteColor($"{OptionPrefix}{Options[selectedIndex].Text}", OptionsTextHighlight, OptionsBackgroundHighlight);
        TargetConsole.CursorTop = savedLineNum;
        return first + selectedIndex;
    }

}

public struct OptionsHelperItem<T> : IEquatable<OptionsHelperItem<T>>, IComparable<OptionsHelperItem<T>> where T: IEquatable<T>, new()
{
    public OptionsHelperItem() { }

    public OptionsHelperItem(string text, T value)
    {
        Text = text; 
        Value = value;
    }

    /// <summary>
    /// This is the displayed text for the option in the list
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// This is the value you care about.  Might and integer or 
    /// GUID if all you want is an ID, or it might be an object 
    /// you actually want as a result of the selection.
    /// </summary>
    public T Value { get; set; } = new();

    /// <summary>
    /// Let's you tell a list how to sort this item relative 
    /// to the others.  If you leave them all the same, the 
    /// next thing is looks at is alphabetical by the Text.  
    /// This means you can also mix the sorting to group 
    /// options by number and let alphabetical deal with 
    /// sorting within the groups.
    /// </summary>
    public int SortValue { get; set; } = 0;

    public int CompareTo(OptionsHelperItem<T> other)
    {
        // easy scenarios first...
        if (SortValue < other.SortValue) { return -1; }
        if (SortValue > other.SortValue) { return 1; }

        // sort value is equal, so we'll look at the display text as a last resort
        return Text.CompareTo(other.Text);
    }

    /// <summary>
    /// Doesn't care about sort order, but compares both the 
    /// IntValue and Text for equality.
    /// </summary>
    public bool Equals(OptionsHelperItem<T> other)
    {
        return Value.Equals(other.Value) && Text.Equals(other.Text, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) { return false; }

        if (obj is OptionsHelperItem<T> ohiObj) { return Equals(ohiObj); }

        if (obj is T tObj) { return Value.Equals(tObj); }

        if (obj is string strObj) { return Text.Equals(strObj, StringComparison.OrdinalIgnoreCase); }

        return false;
    }

    public override int GetHashCode() { return Value.GetHashCode(); }

    /// <summary>
    /// Returns the text property
    /// </summary>
    public override string ToString()
    {
        return Text;
    }


    public static bool operator ==(OptionsHelperItem<T> a, OptionsHelperItem<T> b) { return a.Equals(b); }
    public static bool operator !=(OptionsHelperItem<T> a, OptionsHelperItem<T> b) { return !a.Equals(b); }

    public static bool operator <(OptionsHelperItem<T> a, OptionsHelperItem<T> b) { return a.CompareTo(b) < 0; }
    public static bool operator >(OptionsHelperItem<T> a, OptionsHelperItem<T> b) { return a.CompareTo(b) > 0; }

    public static bool operator <=(OptionsHelperItem<T> a, OptionsHelperItem<T> b) { return a.CompareTo(b) <= 0; }
    public static bool operator >=(OptionsHelperItem<T> a, OptionsHelperItem<T> b) { return a.CompareTo(b) >= 0; }




}
