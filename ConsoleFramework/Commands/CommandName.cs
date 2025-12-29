using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleFramework.Helpers;

namespace ConsoleFramework.Commands;

/// <summary>
/// Let's you define a list of distinct strings that all represent a given 
/// command.  For example, "exit", "x", "quit", and "q" could be stored and 
/// when a user types something, you can use the IsMatch method to see if 
/// what they typed is a match for one of the strings you set.  Also comes 
/// with a series of methods like Equals, ToString, and ==/!=
/// </summary>
public struct CommandName : IEquatable<CommandName>, IComparable<CommandName>
{
    public CommandName() { }

    public CommandName(params string[] names)
    {
        LoadAliases(names);
    }

    private List<string> _aliases = new();
    public IEnumerable<string> Aliases 
    {
        get
        {
            foreach (var alias in _aliases) { yield return alias; }
        }
    }

    public void LoadAliases(params string[] aliases)
    {
        _aliases.Clear();

        foreach (var alias in aliases)
        {
            string workStr = GetCleanAlias(alias);
            if (!_aliases.Any(a => string.Equals(a, workStr, StringComparison.OrdinalIgnoreCase)))
            {
                _aliases.Add(workStr);
            }
        }
    }

    private string GetCleanAlias(string sourceAlias)
    {
        char[] leftTrimChars = { ' ', '\t', '-', '_' };
        return sourceAlias.TrimStart(leftTrimChars).Trim();
    }

    public bool IsMatch(string findString)
    {
        string compareStr = GetCleanAlias(findString);
        return _aliases.Any(a => string.Equals(a, findString, StringComparison.OrdinalIgnoreCase));
    }

    public bool Equals(CommandName other)
    {
        List<string> compareList = new();

        foreach (var alias in _aliases) { compareList.Add(alias); }

        foreach (var comp in other.Aliases)
        {
            if (!compareList.Remove(comp, true)) { return false; }
        }

        return compareList.Count == 0;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) { return false; }

        if (obj is CommandName cmdName) { return Equals(cmdName); }

        if (obj is string[] strArr) { return Equals(new CommandName(strArr)); }

        if (obj is List<string> strList) { return Equals(new CommandName(strList.ToArray())); }

        return false;
    }

    public override int GetHashCode()
    {
        if (_aliases.Count == 0) { return 0; }

        return _aliases[0].GetHashCode();
    }

    public override string ToString()
    {
        return string.Join(", ", _aliases);
    }

    public int CompareTo(CommandName other)
    {
        if (Equals(other)) { return 0; }

        int bothLen = _aliases.Count < other._aliases.Count ? _aliases.Count : other._aliases.Count;
        if (bothLen == 0) { return 0; }

        for (int i = 0; i < bothLen; i++)
        {
            int strCmp = string.Compare(_aliases[i], other._aliases[i], true);
            if (strCmp != 0) { return strCmp; }
        }

        if (_aliases.Count < other._aliases.Count) { return -1; }
        else if (other._aliases.Count < _aliases.Count) { return 1; }
        return 0; // shouldn't be possible to get here because we already checked for equality, but compiler needs it
    }

    public static bool operator ==(CommandName a, CommandName b) { return a.Equals(b); }
    public static bool operator !=(CommandName a, CommandName b) { return !a.Equals(b); }



}
