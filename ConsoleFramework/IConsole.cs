using System;
using System.IO;
using System.Text;

namespace ConsoleFramework;

public interface IConsole
{
    #region Properties

    /// <summary>
    /// Gets or sets the background color of the console.
    /// </summary>
    public ConsoleColor BackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the height of the buffer area.
    /// </summary>
    public int BufferHeight { get; set; }

    /// <summary>
    /// Gets or sets the width of the buffer area.
    /// </summary>
    public int BufferWidth { get; set; }

    /// <summary>
    /// Gets a value indicating whether the CAPS LOCK keyboard toggle is turned on or turned off.
    /// </summary>
    public bool CapsLock { get; }

    /// <summary>
    /// Gets or sets the column position of the cursor within the buffer area.  
    /// Leftmost column is zero (0).
    /// </summary>
    public int CursorLeft { get; set; }

    /// <summary>
    /// Gets or sets the height of the cursor within a character cell. The size of the cursor 
    /// expressed as a percentage of the height of a character cell. The property value ranges 
    /// from 1 to 100.
    /// </summary>
    public int CursorSize { get; set; }

    /// <summary>
    /// Gets or sets the row position of the cursor within the buffer area. 
    /// Topmost row is zero (0).
    /// </summary>
    public int CursorTop { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the cursor is visible.
    /// </summary>
    public bool CursorVisible { get; set; }

    /// <summary>
    /// Gets the standard error output stream.
    /// </summary>
    public TextWriter Error { get; }

    /// <summary>
    /// Gets or sets the foreground or text color of the console.
    /// </summary>
    public ConsoleColor ForegroundColor { get; set; }

    /// <summary>
    /// Gets the standard input stream.
    /// </summary>
    public TextReader In { get; }

    /// <summary>
    /// Gets or sets the encoding the console uses to read input.
    /// </summary>
    public Encoding InputEncoding { get; set; }

    /// <summary>
    /// Gets a value that indicates whether the error output stream has 
    /// been redirected from the standard error stream.
    /// </summary>
    public bool IsErrorRedirected { get; }

    /// <summary>
    /// Gets a value that indicates whether input has been redirected 
    /// from the standard input stream.
    /// </summary>
    public bool IsInputRedirected { get; }

    /// <summary>
    /// Gets a value that indicates whether output has been redirected 
    /// from the standard output stream.
    /// </summary>
    public bool IsOutputRedirected { get; }

    /// <summary>
    /// Gets a value indicating whether a key press is available in the input stream.
    /// </summary>
    public bool KeyAvailable { get; }

    /// <summary>
    /// Gets the largest possible number of console window rows, based on the current font and screen resolution.
    /// </summary>
    public int LargestWindowHeight { get; }

    /// <summary>
    /// Gets the largest possible number of console window columns, based on the current font and screen resolution.
    /// </summary>
    public int LargestWindowWidth { get; }

    /// <summary>
    /// Gets a value indicating whether the NUM LOCK keyboard toggle is turned on or turned off.
    /// </summary>
    public bool NumberLock { get; }

    /// <summary>
    /// Gets the standard output stream.
    /// </summary>
    public TextWriter Out { get; }

    /// <summary>
    /// Gets or sets the encoding the console uses to write output.
    /// </summary>
    public Encoding OutputEncoding { get; set; }

    /// <summary>
    /// Gets or sets the title to display in the console title bar.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the combination of the Control modifier key and C console key (Ctrl+C) 
    /// is treated as ordinary input or as an interruption that is handled by the operating system.
    /// </summary>
    public bool TreatControlCAsInput { get; set; }

    /// <summary>
    /// Gets or sets the height of the console window area.
    /// </summary>
    public int WindowHeight { get; set; }

    /// <summary>
    /// Gets or sets the leftmost position of the console window area relative to the screen buffer.
    /// </summary>
    public int WindowLeft { get; set; }

    /// <summary>
    /// Gets or sets the top position of the console window area relative to the screen buffer.
    /// </summary>
    public int WindowTop { get; set; }

    /// <summary>
    /// Gets or sets the width of the console window.
    /// </summary>
    public int WindowWidth { get; set; }

    #endregion

    //=================================================================================================================

    #region Methods

    /// <summary>
    /// Plays the sound of a beep through to device speaker.  If both frequency and duration 
    /// are provided, it will attempt to make the beep fit that definition.  If both are null 
    /// the default Beep method will be used.  Specific implementation will determine the 
    /// behavior if one of the two values is given, possibly using an arbitrary default for 
    /// the missing value.
    /// </summary>
    /// <param name="frequency">
    /// A value from 37 to 32767 hertz.  Humans can hear 20 to 20,000 hertz in general but 
    /// ideal hearing range is said to be between 2000 and 5000 hertz.</param>
    /// <param name="duration">Duration of the beep in milliseconds</param>
    public void Beep(int? frequency = null, int? duration = null);

    /// <summary>
    /// Clears the console buffer and corresponding console window of display information.
    /// </summary>
    public void Clear();

    /// <summary>
    /// Gets the column (Left) and row (Top) position of the cursor.  Leftmost and topmost 
    /// column and row is (0, 0).
    /// </summary>
    /// <returns>integer pair representing column and row.</returns>
    public (int Left, int Top) GetCursorPosition();

    /// <summary>
    /// Gets you the position of the cursor in the Console via out parameters
    /// </summary>
    /// <param name="column">Leftmost column is 0</param>
    /// <param name="row">Topmost row is 0</param>
    public void GetCursorLocation(out int column, out int row);

    /// <summary>
    /// Copies a specified source area of the screen buffer to a specified destination area.  
    /// If the last three parameters are left as null, the Console method that does not include 
    /// those parameters will be used.  If all of them are filled, the Console method that does 
    /// include them will be used.  If some are included but not all, the implementation will 
    /// decide what defaults to use.
    /// </summary>
    /// <param name="sourceLeft">The leftmost column of the source area.</param>
    /// <param name="sourceTop">The topmost row of the source area.</param>
    /// <param name="sourceWidth">The number of columns in the source area.</param>
    /// <param name="sourceHeight">The number of rows in the source area.</param>
    /// <param name="targetLeft">The leftmost column of the destination area.</param>
    /// <param name="targetTop">The topmost row of the destination area.</param>
    /// <param name="sourceFillChar"></param>
    /// <param name="sourceForeColor"></param>
    /// <param name="sourceBackColor"></param>
    public void MoveBufferArea(
        int sourceLeft,
        int sourceTop,
        int sourceWidth,
        int sourceHeight,
        int targetLeft,
        int targetTop,
        char? sourceFillChar = null,
        ConsoleColor? sourceForeColor = null,
        ConsoleColor? sourceBackColor = null);

    /// <summary>
    /// Acquires the standard error stream.
    /// </summary>
    /// <param name="bufferSize">
    /// According to Microsoft documentation, this parameter has no effect, but 
    /// its value must be greater than or equal to zero.  Seems a pointless thing 
    /// to do I guess.  Included here for completion.
    /// </param>
    /// <returns>The standard error stream</returns>
    public Stream OpenStandardError(int? bufferSize = null);

    /// <summary>
    /// Acquires the standard input stream.
    /// </summary>
    /// <param name="bufferSize">
    /// According to Microsoft documentation, this parameter has no effect, but 
    /// its value must be greater than or equal to zero.  Seems a pointless thing 
    /// to do I guess.  Included here for completion.
    /// </param>
    /// <returns>The standard input stream</returns>
    public Stream OpenStandardInput(int? bufferSize = null);

    /// <summary>
    /// Acquires the standard output stream.
    /// </summary>
    /// <param name="bufferSize">
    /// According to Microsoft documentation, this parameter has no effect, but 
    /// its value must be greater than or equal to zero.  Seems a pointless thing 
    /// to do I guess.  Included here for completion.
    /// </param>
    /// <returns>The standard output stream</returns>
    public Stream OpenStandardOutput(int? bufferSize = null);

    /// <summary>
    /// Reads the next character from the standard input stream as an integer.
    /// </summary>
    /// <returns>
    /// An integer representation of the next value from the input stream or -1 if none.
    /// </returns>
    public int Read();

    /// <summary>
    /// Obtains the next character or function key pressed by the user.  With a 
    /// true value passed in the parameter, this how methods that handle password 
    /// entry and the like are created.
    /// </summary>
    /// <param name="interrupt">
    /// If you leave the parameter null, it will use the normal Console 
    /// ReadKey.  If you pass in a value, it will use the method that 
    /// accepts a parameter.  If you pass true specifically, the effects 
    /// of the key press won't occur.  You'll have the data involved, 
    /// and are responsible for any updates to output and for how the 
    /// data is used.
    /// </param>
    /// <returns>
    /// A special class with properties to tell you what happened with a 
    /// key press including special keys.
    /// </returns>
    public ConsoleKeyInfo ReadKey(bool? interrupt = null);

    /// <summary>
    /// The most common way to get user input as it returns when the 
    /// enter key is hit and gives you the full line of what the user 
    /// typed into the input stream.
    /// </summary>
    /// <returns>Usually a string of the user's input, but null if nothing else is available.</returns>
    public string? ReadLine();

    /// <summary>
    /// Sets the foreground and background console colors to their defaults.
    /// </summary>
    public void ResetColor();

    /// <summary>
    /// Sets the height and width of the screen buffer area to the specified values.
    /// </summary>
    /// <param name="width">The width of the buffer area measured in columns.</param>
    /// <param name="height">The height of the buffer area measured in rows.</param>
    public void SetBufferSize(int width, int height);

    /// <summary>
    /// Sets the position of the cursor.
    /// </summary>
    /// <param name="left">
    /// The column position of the cursor. ColumnCount are numbered from left to right starting at 0.
    /// </param>
    /// <param name="top">
    /// The row position of the cursor. RowCount are numbered from top to bottom starting at 0.
    /// </param>
    public void SetCursorPosition(int left, int top);

    /// <summary>
    /// Sets the Error property to the specified TextWriter object.
    /// </summary>
    /// <param name="newError">
    /// A stream that is the new standard error output.
    /// </param>
    public void SetError(TextWriter newError);

    /// <summary>
    /// Sets the In property to the specified TextReader object.
    /// </summary>
    /// <param name="newIn">
    /// A stream that is the new standard input.
    /// </param>
    public void SetIn(TextReader newIn);

    /// <summary>
    /// Sets the Out property to target the TextWriter object.
    /// </summary>
    /// <param name="newOut">
    /// A text writer to be used as the new standard output.
    /// </param>
    public void SetOut(TextWriter newOut);

    /// <summary>
    /// Sets the position of the console window relative to the screen buffer.
    /// </summary>
    /// <param name="left">The column position of the upper left corner of the console window.</param>
    /// <param name="top">The row position of the upper left corner of the console window.</param>
    public void SetWindowPosition(int left, int top);

    /// <summary>
    /// Sets the height and width of the console window to the specified values.
    /// </summary>
    /// <param name="width">The width of the console window measured in columns.</param>
    /// <param name="height">The height of the console window measured in rows.</param>
    public void SetWindowSize(int width, int height);



    // Writes ==========================================================================================

    /// <summary>
    /// WriteColor the given string value to the output stream
    /// </summary>
    /// <param name="value"></param>
    public void Write(string value);

    /// <summary>
    /// Covers Writes using a format string along with the array of objects used 
    /// to fill in marker locations in the format string.  Try to be sure the 
    /// number of markers in the format string match the number of _arguments 
    /// because the index of the object matches the marker it will replace.
    /// </summary>
    /// <param name="format">
    /// The string that determines what the framework of the output will look like.  
    /// It contains markers denoted by squiggle brackets containing the index number 
    /// of the value to be used in that spot.  An example might be a string like 
    /// "first name: {0}\nlast name: {1}" and then the args would have two values.
    /// </param>
    /// <param name="args">
    /// This is the set of values to use in replacing the markers in the format 
    /// string.  It allows the set to be defined as an array or other collection, 
    /// or as a comma delimited list of objects right in the function call.
    /// </param>
    public void Write(string format, params object[] args);

    /// <summary>
    /// Writes the specified Unicode character value to the standard output stream.
    /// </summary>
    /// <param name="value">The value to write</param>
    public void Write(char value);

    /// <summary>
    /// Writes an array of characters and optionally allows a starting point 
    /// and length of output to be specified.
    /// </summary>
    /// <param name="buffer">The array of characters</param>
    /// <param name="startIndex">The place to start writing from</param>
    /// <param name="count">The total count of characters to write</param>
    public void Write(char[] buffer, int? startIndex = null, int? count = null);

    /// <summary>
    /// Writes the text representation of the specified object to the standard output stream.
    /// </summary>
    /// <param name="value">The value to write, or null.</param>
    public void Write(object? value);




    // WriteLines ==========================================================================================

    /// <summary>
    /// Just writes the Line Break
    /// </summary>
    public void WriteLine();

    /// <summary>
    /// Writes the given string value to the output stream including a line break
    /// </summary>
    /// <param name="value"></param>
    public void WriteLine(string value);

    /// <summary>
    /// Covers WriteLines using a format string along with the array of objects used 
    /// to fill in marker locations in the format string.  Try to be sure the 
    /// number of markers in the format string match the number of _arguments 
    /// because the index of the object matches the marker it will replace.
    /// </summary>
    /// <param name="format">
    /// The string that determines what the framework of the output will look like.  
    /// It contains markers denoted by squiggle brackets containing the index number 
    /// of the value to be used in that spot.  An example might be a string like 
    /// "first name: {0}\nlast name: {1}" and then the args would have two values.
    /// </param>
    /// <param name="args">
    /// This is the set of values to use in replacing the markers in the format 
    /// string.  It allows the set to be defined as an array or other collection, 
    /// or as a comma delimited list of objects right in the function call.
    /// </param>
    public void WriteLine(string format, params object[] args);

    /// <summary>
    /// WriteColor the specified Unicode character value to the standard output stream including a line break
    /// </summary>
    /// <param name="value">The value to WriteColor</param>
    public void WriteLine(char value);

    /// <summary>
    /// Writes an array of characters and optionally allows a starting point 
    /// and length of output to be specified and ends with a line break.
    /// </summary>
    /// <param name="buffer">The array of characters</param>
    /// <param name="startIndex">The place to start writing from</param>
    /// <param name="count">The total count of characters to WriteColor</param>
    public void WriteLine(char[] buffer, int? startIndex = null, int? count = null);

    /// <summary>
    /// Writes the text representation of the specified object to the 
    /// standard output stream and includes a line break.
    /// </summary>
    /// <param name="value">The value to WriteColorLine, or null.</param>
    public void WriteLine(object? value);


    #endregion

    //=================================================================================================================

    /// <summary>
    /// Occurs when the Control modifier key (Ctrl) and either 
    /// the C console key (C) or the Break key are pressed 
    /// simultaneously (Ctrl+C or Ctrl+Break).
    /// </summary>
    public event ConsoleCancelEventHandler? CancelKeyPress;
}
