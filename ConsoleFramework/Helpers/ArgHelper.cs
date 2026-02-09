using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleFramework.Helpers;

public static class ArgHelper
{
    /// <summary>
    /// Because the default parse assumes it should remove double quotes, things like: 
    /// [param="test value"] come back looking the same as: ["param=test value"].  This 
    /// method can't tell what the original value was, so it is going to guess that if 
    /// it has something like [param=test value] it should be [param="test value"].
    /// </summary>
    /// <param name="args">The arguments as parsed by the built in parser</param>
    /// <returns>The argument re-parsed to be something cleaner</returns>
    public static List<ArgPair> ReParseArgs(string[] args)
    {
        List<ArgPair> result = new();

        // first pass separate param=test value into two items like [param=] and [test value]
        List<string> worker = new();
        foreach (string arg in args)
        {
            int spaceIndex = arg.IndexOf(' ');
            int equalsIndex = arg.IndexOf('=');

            if (equalsIndex != -1 && spaceIndex > equalsIndex)
            {
                // the case where a param name is there with equals and something that SHOULD be quoted
                worker.Add(arg.Substring(0, equalsIndex + 1)); // param name with equal sign
                worker.Add(arg.Substring(equalsIndex + 1)); // the stuff that will need to be double quoted
                continue;
            }

            // all other cases
            worker.Add(arg);
        }

        //result.Add(new("first", worker.Count.ToString()));


        // second pass needs to make sure there are double quotes around anything left with spaces
        int workCount = worker.Count;
        for (int i = 0; i < worker.Count; i++)
        {
            if (worker[i].Contains(' ')) { worker[i] = $"\"{worker[i]}\""; }
        }
        //result.Add(new("second", worker.Count.ToString()));

        // third pass can now combine solitary equals items with the previous item
        for (int r = workCount - 1; r >= 1; r--)
        {
            if (worker[r].Length == 1 && worker[r][0] == '=')
            {
                worker[r - 1] += '='; // tack on the equals to the previous item
                worker.RemoveAt(r); // get rid of the equal sign that is alone
            }
        }
        //result.Add(new("third", worker.Count.ToString()));

        // fourth pass is just to be sure there aren't any ===== that should be just =
        workCount = worker.Count; // need to reset this since we might have removed some
        for (int c = 0; c < workCount; c++)
        {
            while (worker[c].EndsWith("=="))
            {
                worker[c] = worker[c].Substring(0, worker[c].Length - 1);
            }
        }
        //result.Add(new("fourth", worker.Count.ToString()));

        //fifth pass gives us our return value
        int p = 0;
        while (p < workCount)
        {
            if (worker[p].EndsWith('='))
            {
                // this is a param name and we should use this and the next item to create a pair
                ArgPair apItem = new() { Name = worker[p].Substring(0, worker[p].Length - 1) };
                p++; // move to the next item
                if (p < workCount)
                {
                    apItem.Value = worker[p];
                    p++; // move past the value we just used
                }
                result.Add(apItem);
            }
            else
            {
                // this is a nameless value
                ArgPair apItem = new ArgPair(worker[p]);
                result.Add(apItem);
                p++;
            }
        }
        //result.Add(new("fifth", result.Count.ToString()));


        return result;
    }
}

public struct ArgPair : IEquatable<ArgPair>
{
    public ArgPair() { }

    public ArgPair(string value)
    {
        Value = value;
    }

    public ArgPair(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public string? Name { get; set; } = null;

    public string Value { get; set; } = string.Empty;

    public bool Equals(ArgPair other)
    {
        return string.Equals(Name, other.Name)
            && string.Equals(Value, other.Value);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null) { return false; }

        if (obj is ArgPair apObj) { return Equals(apObj); }

        if (obj is string strObj) { return string.Equals(Value, strObj); }

        return false;
    }

    public override int GetHashCode()
    {
        return Name is not null ? Name.GetHashCode() : 0;
    }

    public override string ToString()
    {
        var result = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(Name))
        {
            result.Append($"{Name}=");
        }

        result.Append(Value);

        return result.ToString();
    }

    public static bool operator ==(ArgPair left, ArgPair right) { return left.Equals(right); }
    public static bool operator !=(ArgPair left, ArgPair right) { return !left.Equals(right); }
}
