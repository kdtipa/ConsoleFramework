using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleFramework;

public abstract class AbstractConsole : IConsole
{
    #region Properties

    /// <inheritdoc />
    public virtual ConsoleColor BackgroundColor { get { return Console.BackgroundColor; } set { Console.BackgroundColor = value; } }

    /// <inheritdoc />
    public virtual int BufferHeight { get { return Console.BufferHeight; } set { Console.BufferHeight = value; } }

    /// <inheritdoc />
    public virtual int BufferWidth { get { return Console.BufferWidth; } set { Console.BufferWidth = value; } }


    /// <inheritdoc />
    public virtual bool CapsLock => Console.CapsLock;


    /// <inheritdoc />
    public virtual int CursorLeft { get { return Console.CursorLeft; } set { Console.CursorLeft = value; } }

    /// <inheritdoc />
    public virtual int CursorSize { get { return Console.CursorSize; } set { Console.CursorSize = value; } }

    /// <inheritdoc />
    public virtual int CursorTop { get { return Console.CursorTop; } set { Console.CursorTop = value; } }

    /// <inheritdoc />
    public virtual bool CursorVisible { get { return Console.CursorVisible; } set { Console.CursorVisible = value; } }


    /// <inheritdoc />
    public virtual TextWriter Error => Console.Error;


    /// <inheritdoc />
    public virtual ConsoleColor ForegroundColor { get { return Console.ForegroundColor; } set { Console.ForegroundColor = value; } }


    /// <inheritdoc />
    public virtual TextReader In => Console.In;


    /// <inheritdoc />
    public virtual Encoding InputEncoding { get { return Console.InputEncoding; } set { Console.InputEncoding = value; } }


    /// <inheritdoc />
    public virtual bool IsErrorRedirected => Console.IsErrorRedirected;


    /// <inheritdoc />
    public virtual bool IsInputRedirected => Console.IsInputRedirected;


    /// <inheritdoc />
    public virtual bool IsOutputRedirected => Console.IsOutputRedirected;


    /// <inheritdoc />
    public virtual bool KeyAvailable => Console.KeyAvailable;


    /// <inheritdoc />
    public virtual int LargestWindowHeight => Console.LargestWindowHeight;


    /// <inheritdoc />
    public virtual int LargestWindowWidth => Console.LargestWindowWidth;


    /// <inheritdoc />
    public virtual bool NumberLock => Console.NumberLock;


    /// <inheritdoc />
    public virtual TextWriter Out => Console.Out;


    /// <inheritdoc />
    public virtual Encoding OutputEncoding { get { return Console.OutputEncoding; } set { Console.OutputEncoding = value; } }

    /// <inheritdoc />
    public virtual string Title { get { return Console.Title; } set { Console.Title = value; } }

    /// <inheritdoc />
    public virtual bool TreatControlCAsInput { get { return Console.TreatControlCAsInput; } set { Console.TreatControlCAsInput = value; } }

    /// <inheritdoc />
    public virtual int WindowHeight { get { return Console.WindowHeight; } set { Console.WindowHeight = value; } }

    /// <inheritdoc />
    public virtual int WindowLeft { get { return Console.WindowLeft; } set { Console.WindowLeft = value; } }

    /// <inheritdoc />
    public virtual int WindowTop { get { return Console.WindowTop; } set { Console.WindowTop = value; } }

    /// <inheritdoc />
    public virtual int WindowWidth { get { return Console.WindowWidth; } set { Console.WindowWidth = value; } }

    #endregion

    //==============================================================================

    public event ConsoleCancelEventHandler? CancelKeyPress;

    //==============================================================================

    #region Methods

    public virtual void Beep(int? frequency = null, int? duration = null)
    {
        if (frequency is null && duration is null) { Console.Beep(); return; } // default

        Console.Beep(Beep_CleanFrequency(frequency), Beep_CleanDuration(duration));
    }

    private int Beep_CleanFrequency(int? sourceFrequency)
    {
        if (sourceFrequency is null) { return 4000; }
        else if (sourceFrequency <= 37) { return 37; }
        else if (sourceFrequency >= 32767) { return 32767; }
        else { return sourceFrequency.Value; }
    }

    private int Beep_CleanDuration(int? sourceDuration)
    {
        if (sourceDuration is null) { return 500; }
        else if (sourceDuration <= 50) { return 50; }
        else if (sourceDuration >= 5000) { return 5000; }
        else { return sourceDuration.Value; }
    }


    public virtual void Clear()
    {
        Console.Clear();
    }

    public virtual void GetCursorLocation(out int column, out int row)
    {
        var pos = Console.GetCursorPosition();
        column = pos.Left;
        row = pos.Top;
    }

    public virtual (int Left, int Top) GetCursorPosition()
    {
        return Console.GetCursorPosition();
    }

    public virtual void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop, char? sourceFillChar = null, ConsoleColor? sourceForeColor = null, ConsoleColor? sourceBackColor = null)
    {
        // make sure we have useful coordinates and sizes
        if (sourceWidth == 0) { sourceWidth = 1; }
        else if (sourceWidth < 0)
        {
            sourceLeft += sourceWidth;
            sourceWidth *= -1;
        }

        if (sourceLeft < 0) { sourceLeft = 0; }
        else if (sourceLeft + sourceWidth > BufferWidth) { sourceLeft = BufferWidth - sourceWidth; }

        if (sourceHeight == 0) { sourceHeight = 1; }
        else if (sourceHeight < 0)
        {
            sourceTop += sourceHeight;
            sourceHeight *= -1;
        }

        if (sourceTop < 0) { sourceTop = 0; }
        else if (sourceTop + sourceHeight > BufferHeight) { sourceTop = BufferHeight - sourceHeight; }

        if (targetLeft < 0) { targetLeft = 0; }
        else if (targetLeft + sourceWidth > BufferWidth) { targetLeft = BufferWidth - sourceWidth; }

        if (targetTop < 0) { targetTop = 0; }
        else if (targetTop + sourceHeight > BufferHeight) { targetTop = BufferHeight - sourceHeight; }

        // okay, now we have valid coordinates I think.  Might as well check the optional parameters too.
        char fillChar = sourceFillChar ?? ' '; // default to a space
        ConsoleColor savedFG = ForegroundColor;
        ConsoleColor savedBG = BackgroundColor;

        if (sourceForeColor is not null) { ForegroundColor = sourceForeColor.Value; }
        if (sourceBackColor is not null) { BackgroundColor = sourceBackColor.Value; }


        // now run the method...
        Console.MoveBufferArea(sourceLeft, sourceTop, sourceWidth, sourceHeight, targetLeft, targetTop);


        // and put the fore and back colors back to what they were...
        ForegroundColor = savedFG;
        BackgroundColor = savedBG;
    }

    public virtual Stream OpenStandardError(int? bufferSize = null)
    {
        return bufferSize is null ? Console.OpenStandardError() : Console.OpenStandardError(bufferSize.Value);
    }

    public virtual Stream OpenStandardInput(int? bufferSize = null)
    {
        return bufferSize is null ? Console.OpenStandardInput() : Console.OpenStandardInput(bufferSize.Value);
    }

    public virtual Stream OpenStandardOutput(int? bufferSize = null)
    {
        return bufferSize is null ? Console.OpenStandardOutput() : Console.OpenStandardOutput(bufferSize.Value);
    }

    public virtual int Read()
    {
        return Console.Read();
    }

    public virtual ConsoleKeyInfo ReadKey(bool? interrupt = null)
    {
        return interrupt is null ? Console.ReadKey() : Console.ReadKey(interrupt.Value);
    }

    public virtual string? ReadLine()
    {
        return Console.ReadLine();
    }

    public virtual void ResetColor()
    {
        Console.ResetColor();
    }

    public virtual void SetBufferSize(int width, int height)
    {
        Console.SetBufferSize(width, height);
    }

    public virtual void SetCursorPosition(int left, int top)
    {
        Console.SetCursorPosition(left, top);
    }

    public virtual void SetError(TextWriter newError)
    {
        Console.SetError(newError);
    }

    public virtual void SetIn(TextReader newIn)
    {
        Console.SetIn(newIn);
    }

    public virtual void SetOut(TextWriter newOut)
    {
        Console.SetOut(newOut);
    }

    public virtual void SetWindowPosition(int left, int top)
    {
        Console.SetWindowPosition(left, top);
    }

    public virtual void SetWindowSize(int width, int height)
    {
        Console.SetWindowSize(width, height);
    }

    public virtual void Write(string value)
    {
        Console.Write(value);
    }

    public virtual void Write(string format, params object[] args)
    {
        Console.Write(format, args);
    }

    public virtual void Write(char value)
    {
        Console.Write(value);
    }

    public virtual void Write(char[] buffer, int? startIndex = null, int? count = null)
    {
        if (startIndex is null || count is null) { Console.Write(buffer); }
        else { Console.Write(buffer, startIndex.Value, count.Value); }
    }

    public virtual void Write(object? value)
    {
        Console.Write(value);
    }

    public virtual void WriteLine()
    {
        Console.WriteLine();
    }

    public virtual void WriteLine(string value)
    {
        Console.WriteLine(value);
    }

    public virtual void WriteLine(string format, params object[] args)
    {
        Console.WriteLine(format, args);
    }

    public virtual void WriteLine(char value)
    {
        Console.WriteLine(value);
    }

    public virtual void WriteLine(char[] buffer, int? startIndex = null, int? count = null)
    {
        if (startIndex is null || count is null) { Console.WriteLine(buffer); }
        else { Console.WriteLine(buffer, startIndex.Value, count.Value); }
    }

    public virtual void WriteLine(object? value)
    {
        Console.WriteLine(value);
    }

    #endregion
}
