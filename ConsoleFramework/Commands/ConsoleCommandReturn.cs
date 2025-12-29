using ConsoleFramework.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleFramework.Commands;

public struct ConsoleCommandReturn : IEquatable<ConsoleCommandReturn>
{
    public ConsoleCommandReturn() { }

    public ConsoleCommandReturn(
        bool ranSuccessfully, 
        bool? resultOfCommandIsGood = null, 
        int? resultValue = null, 
        params string[] messages)
    {
        RanSuccessfully = ranSuccessfully;
        ResultOfCommandIsGood = resultOfCommandIsGood is not null ? resultOfCommandIsGood.Value : true;
        ResultValue = resultValue is not null ? resultValue.Value : 0;

        foreach (string msg in messages) { Messages.Add(msg); }
    }

    /// <summary>
    /// Did the code run without horrifying exceptions?  If you set 
    /// this to false, ResultOfCommandIsGood will also come back 
    /// false since the code failing to run can't result in "good".
    /// </summary>
    public bool RanSuccessfully { get; set; } = true;

    private bool _resultOfCommandIsGood = true;
    /// <summary>
    /// If the command is one where the main code was asking 
    /// for a true or false response, this contains the result.  
    /// For example, if it's a login command, and the login is 
    /// successful, this can return true.  If RanSuccessfully is 
    /// false, this property cannot be true;
    /// </summary>
    public bool ResultOfCommandIsGood
    {
        get { return _resultOfCommandIsGood && RanSuccessfully; }
        set { _resultOfCommandIsGood = value; }
    }


    /// <summary>
    /// It is common for commands to return a numeric code.  
    /// This property is just meant for those cases where 
    /// the caller expects a number.
    /// </summary>
    public int ResultValue { get; set; } = 0;


    public bool IsHelpRequest { get; set; } = false;

    public bool IsExitRequest { get; set; } = false;



    /// <summary>
    /// Any messages the calling code might need.
    /// </summary>
    public List<string> Messages { get; set; } = new();

    public bool Equals(ConsoleCommandReturn other)
    {
        return RanSuccessfully == other.RanSuccessfully 
            && ResultOfCommandIsGood == other.ResultOfCommandIsGood 
            && ResultValue == other.ResultValue 
            && Messages.EqualByValue(other.Messages);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) { return false; }

        if (obj is ConsoleCommandReturn ccrObj) { return Equals(ccrObj); }

        return false;
    }

    public override int GetHashCode()
    {
        // if everything is a good result, return the unix idea of a good result: 0
        if (RanSuccessfully && ResultOfCommandIsGood && ResultValue == 0) { return 0; }

        // if the user went to the trouble of setting the result value, use that
        if (ResultValue != 0) { return ResultValue; }

        // If the code didn't run, we'll use minimum value to suggest it
        if (RanSuccessfully == false) { return int.MinValue; }

        // this last one is tough.  We know ResultValue is 0 and the code ran without exception, 
        // so since we didn't exit the method on the first test, we know that the result of the 
        // command is bad (instead of good).  We could return -1 as is common practice, but users 
        // might use that as their normal ResultValue.  So, we're going to do minimum value +1.
        return int.MinValue + 1;

    }

    public override string ToString()
    {
        var str = new StringBuilder();

        if (RanSuccessfully) { str.AppendLine("Ran Successfully"); }
        else { str.AppendLine("FAILED to run"); }

        if (ResultOfCommandIsGood) { str.AppendLine("Command Result is Good"); }
        else { str.AppendLine("Command Result is Bad"); }

        str.AppendLine($"Result Value = {ResultValue}");

        if (Messages.Count > 0)
        {
            str.AppendLine("Messages...");
            foreach (string msg in Messages)
            {
                str.AppendLine($" - {msg}");
            }
        }

        return str.ToString();
    }


    public static bool operator ==(ConsoleCommandReturn a, ConsoleCommandReturn b) { return a.Equals(b); }
    public static bool operator !=(ConsoleCommandReturn a, ConsoleCommandReturn b) { return !a.Equals(b); }

}
