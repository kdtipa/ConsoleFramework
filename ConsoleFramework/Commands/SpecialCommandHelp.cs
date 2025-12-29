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

    public AutoSortList<CommandParameter> Parameters { get; } = new();

    public ConsoleCommandReturn? Run(IConsole cnsl, params string[] args)
    {
        foreach (string arg in args)
        {
            if (Names.IsMatch(arg))
            {
                this.ShowLongHelp(cnsl);
                return null;
            }
        }

        return new ConsoleCommandReturn() { IsHelpRequest = true };
    }
}
