using ConsoleFramework.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleFramework.Commands;

public class SpecialCommandHelp : IConsoleCommand
{
    public CommandName Names { get; } = new CommandName("Help", "H", "?");

    public string ShortHelp { get; } = "Run this command to see the help message.";

    public string LongHelp { get; } = "This command will get you the general help message.  To get the specific help for a command, type that command name first followed by a help alias.";


    public ConsoleCommandReturn? Run(IConsole cnsl, params string[] args)
    {
        return new ConsoleCommandReturn() { IsHelpRequest = true };
    }

    public ConsoleCommandReturn? Run(IConsole cnsl, List<string> args)
    {
        return new ConsoleCommandReturn() { IsHelpRequest = true };
    }
}
