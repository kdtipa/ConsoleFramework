using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleFramework.Commands;

public struct CommandParameter : IEquatable<CommandParameter>, IComparable<CommandParameter>
{
    /// <summary>
    /// Creates an empty default CommandParameter.
    /// </summary>
    public CommandParameter() { }

    /// <summary>
    /// Creates an instance of a CommandParameter with whatever data you 
    /// provide for initialization.  That first parameter is important 
    /// because if you don't provide any others, it'll try to parse it 
    /// for those other values.  If the parse doesn't work, it will just 
    /// store that in the name.
    /// </summary>
    /// <param name="name">
    /// A string that is either empty, the name of the parameter, or a 
    /// string that can be parsed to fill the properties of this object.
    /// </param>
    /// <param name="isRequired">
    /// Tells us if this is a required parameter or optional
    /// </param>
    /// <param name="order">
    /// Tells us the order you want the parameter to be entered in, but 
    /// required parameters ALWAYS come before optional ones because the 
    /// command can't know if earlier ordered optional parameters have 
    /// been left out or not.
    /// </param>
    /// <param name="description">
    /// The description of the parameter that explains to the user what 
    /// the parameter really represents.
    /// </param>
    public CommandParameter(string name, bool? isRequired = null, int? order = null, string? description = null)
    {
        Name = name;
        IsRequired = isRequired ?? false;
        Order = order ?? 0;
        Description = string.IsNullOrWhiteSpace(description) ? string.Empty : description;
    }

    private string _name = string.Empty;
    /// <summary>
    /// Optional name to display to help convey the meaning of this parameter
    /// </summary>
    public string Name
    {
        get { return _name; }
        set
        {
            _name = value;
            HasName = !string.IsNullOrWhiteSpace(value);
        }
    }
    public bool HasName { get; private set; } = false;


    /// <summary>
    /// Defaults to false.  If sorting a list of parameters, required ones MUST 
    /// come before optional ones, so regardless of the Order property, if you 
    /// set this to true, it will come before any listed as optional.
    /// </summary>
    public bool IsRequired { get; set; } = false;

    /// <summary>
    /// Tells the sort for the parameter list what order the parameters come in 
    /// when entering them on the command line.  First priority for order though 
    /// is whether the parameter is required or not.  Required parameters always 
    /// come before optional ones.
    /// </summary>
    public int Order { get; set; } = 0;

    private string _description = string.Empty;
    /// <summary>
    /// A string meant to tell the user what this parameter is for.  For example 
    /// if you have a command that searches a directory, the first command might 
    /// be one that tells the command what directory to start in and this description 
    /// would say that.
    /// </summary>
    public string Description
    {
        get { return _description; }
        set
        {
            _description = value;
            HasDescription = !string.IsNullOrWhiteSpace(value);
        }
    }
    public bool HasDescription { get; private set; }

    public bool Equals(CommandParameter other)
    {
        return Order == other.Order 
            && IsRequired == other.IsRequired 
            && string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase)
            && string.Equals(Description, other.Description, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) { return false; }

        if (obj is CommandParameter cpObj) { return Equals(cpObj); }

        return false;
    }

    public override int GetHashCode()
    {
        var hashStr = new StringBuilder();
        hashStr.Append(Name);
        hashStr.Append(Description);
        return hashStr.ToString().GetHashCode();
    }

    /// <summary>
    /// Default ToString that gets you "[name] = [required], [description]", 
    /// but leaves out name and/or description if it doesn't have them.
    /// </summary>
    public override string ToString()
    {
        var result = new StringBuilder();

        if (HasName) { result.Append(Name); } // only add the name if there is one

        if (HasName && HasDescription) { result.Append(" ="); } // put in an equal sign if it has both a name and description

        if (HasName) { result.Append(' '); } // if there's no name, required or optional is first so doesn't need a space
        result.Append(IsRequired ? "Required" : "Optional");

        if (HasDescription)
        {
            result.Append(", ");
            result.Append(Description);
        }
        
        return result.ToString();
    }

    /// <summary>
    /// This version of ToString is meant to help with 
    /// lining up lists of parameters in a help message 
    /// so that the equal signs are all in the same 
    /// column.  Find the longest name in your list, 
    /// pass that value to this, and the name will be 
    /// padded with spaces before the equal sign so the 
    /// equal signs line up in the fixed width Console.
    /// </summary>
    public string ToString(int nameWidth)
    {
        var result = new StringBuilder();
        int pad = nameWidth;
        if (pad <= 0) { pad = 0; }

        result.Append(Name.PadRight(pad, ' ')); // doesn't care if there's a name or not

        if (HasDescription) { result.Append(" = "); }
        result.Append(IsRequired ? "Required" : "Optional");

        if (HasDescription)
        {
            result.Append(", ");
            result.Append(Description);
        }

        return result.ToString();
    }

    public int CompareTo(CommandParameter other)
    {
        // if this is required and the other isn't, this comes first
        if (IsRequired && !other.IsRequired) { return -1; }

        // if the other is required and this isn't, they go first
        if (!IsRequired && other.IsRequired) { return 1; }

        // now we know their level of requirement is the same, so we'll trying comparing Order
        if (Order < other.Order) { return -1; }
        if (Order > other.Order) { return 1; }

        // now we know their orders are the same... so we could try to alphabetize or something, 
        // but the order the user added the parameter to the list now can count for something.
        return 0;
    }

    public static bool operator ==(CommandParameter left, CommandParameter right) { return left.Equals(right); }
    public static bool operator !=(CommandParameter left, CommandParameter right) { return !left.Equals(right); }
}
