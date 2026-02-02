using ConsoleFramework;
using ConsoleFramework.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.commands;

public class cmdInputInt : IConsoleCommand
{
    public CommandName Names { get; } = new CommandName("InputInt", "Int", "II");

    public string ShortHelp { get; } = "A test command for using the integer only input method";

    public string LongHelp { get; } = "A test command for using the integer only input method";

    public ConsoleCommandReturn? Run(IConsole cnsl, params string[] args)
    {
        return Run(cnsl, new List<string>());
    }

    public ConsoleCommandReturn? Run(IConsole cnsl, List<string> args)
    {
        var result = cnsl.ReadInt("Integer: ", ConsoleColor.DarkGray, ConsoleColor.White);
        if (result is null) { cnsl.WriteLine("no value given"); }
        else
        {
            cnsl.WriteLine($"value entered: {result.ToString()}");
        }
        return null;
    }
}
