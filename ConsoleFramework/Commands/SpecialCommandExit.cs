using ConsoleFramework.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleFramework.Commands;

internal class SpecialCommandExit : IConsoleCommand
{
    public CommandName Names { get; } = new CommandName("Exit", "X", "Quit", "Q");

    public string ShortHelp { get; } = "Run this command to exit the console application and go back to the command line.";

    public string LongHelp { get; } = "This command tells the console application that you are done and that you want to close the app.  No special arguments are required";

    public ConsoleCommandReturn? Run(IConsole cnsl, params string[] args)
    {
        return new ConsoleCommandReturn() { IsExitRequest = true };
    }

    public ConsoleCommandReturn? Run(IConsole cnsl, List<string> args)
    {
        return new ConsoleCommandReturn() { IsExitRequest = true };
    }
}
