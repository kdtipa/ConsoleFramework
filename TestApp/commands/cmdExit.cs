using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleFramework;
using ConsoleFramework.Commands;
using ConsoleFramework.Helpers;

namespace TestApp.commands;

public class cmdExit : IConsoleCommand
{
    public CommandName Names { get; } = new CommandName("Exit", "X", "Quit", "Q");

    public string ShortHelp { get; } = "Closes this application.";

    public string LongHelp { get; } = "Ends the application allowing the Console to go back to its normal command line prompt.";

    public ConsoleCommandReturn? Run(IConsole cnsl, params string[] args)
    {
        ConsoleCommandReturn result = new(true, true);
        result.IsExitRequest = true;
        return result;
    }

    public ConsoleCommandReturn? Run(IConsole cnsl, List<string> args)
    {
        ConsoleCommandReturn result = new(true, true);
        result.IsExitRequest = true;
        return result;
    }
}
