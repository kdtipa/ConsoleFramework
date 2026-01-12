using ConsoleFramework;
using ConsoleFramework.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.commands;

public class cmdHelp : IConsoleCommand
{
    public CommandName Names { get; } = new CommandName("Help", "H", "?");

    public string ShortHelp { get; } = "Show the help menu.";

    public string LongHelp { get; } = "Show the help menu.";

    public ConsoleCommandReturn? Run(IConsole cnsl, params string[] args)
    {
        return Run(cnsl, args.ToList());
    }

    public ConsoleCommandReturn? Run(IConsole cnsl, List<string> args)
    {
        return new ConsoleCommandReturn() { IsHelpRequest = true };
    }
}
