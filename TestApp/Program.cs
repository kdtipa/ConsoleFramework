using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleFramework;
using ConsoleFramework.Commands;
using ConsoleFramework.Helpers;
using TestApp.commands;

namespace TestApp;

internal class Program
{
    static void Main(string[] args)
    {
        ConsoleClass cnsl = new();

        cnsl.WriteColorLine("Welcome to the test app for the ConsoleFramework", ConsoleColor.Blue);
        cnsl.WriteColorLine($"arguments received from command line: {args.Length}", ConsoleColor.DarkBlue);

        string mainPrompt = ">> ";
        bool keepGoing = true;
        cmdExit exit = new();

        List<IConsoleCommand> cmds = new()
        {
            new cmdHelp(),
            new cmdExit(),
            new cmdOptions(),
            new cmdLogin(),
            new cmdWorkIndicator(),
            new cmdFramedText(),
            new cmdInputInt()
        };

        while (keepGoing)
        {
            // get the user's input
            var userInput = cnsl.ReadLine(mainPrompt, ConsoleColor.DarkGray, ConsoleColor.White);

            // just loop back around if they didn't give us anything
            if (string.IsNullOrWhiteSpace(userInput)) { continue; }

            // use a utility method to break it up in case they gave arguments
            ConsoleCommandHelper.BreakUpInput(userInput, out var userCmd, out var userArgs);

            // shouldn't be possible, but just in case, we'll check for empty
            if (string.IsNullOrEmpty(userCmd)) { continue; }

            // now we compare that first chunk of text to the command list to see if there's a match
            var matchingCmd = cmds.Where(cmd => cmd.IsMatch(userCmd)).FirstOrDefault();

            // if there's no match, alert the user and loop back around
            if (matchingCmd is null) { cnsl.WriteColorLine($"No command matches for [{userCmd}]\n", ConsoleColor.Yellow); continue; }

            // run the command
            if (userArgs is null) { userArgs = new List<string>(); }
            var cmdResult = matchingCmd.Run(cnsl, userArgs);

            // deal with results of running the command
            if (cmdResult is not null)
            {
                if (cmdResult.Value.IsExitRequest) { keepGoing = false; continue; }
                else if (cmdResult.Value.IsHelpRequest) { ShowHelp(cnsl, cmds); }
            }
        }


    }

    public static void ShowHelp(IConsole cnsl, List<IConsoleCommand> cmds)
    {
        cnsl.WriteColorLine("Help List...", ConsoleColor.Yellow);

        List<HelpPair> helpPairs = new List<HelpPair>();
        foreach (var cmd in cmds)
        {
            helpPairs.Add(new HelpPair(cmd.Names.ToString(), cmd.ShortHelp));
        }

        int longestAliases = helpPairs.Max(hp => hp.AliasesLength);

        foreach (var helpText in helpPairs)
        {
            cnsl.WriteColor(helpText.Aliases.PadRight(longestAliases, ' '), ConsoleColor.Yellow);
            cnsl.WriteColor(" = ", ConsoleColor.Yellow);
            cnsl.WriteColorLine(helpText.ShortHelp, ConsoleColor.Yellow);
        }

        cnsl.WriteLine();
    }
}

public struct HelpPair
{
    public HelpPair() { }

    public HelpPair(string aliases, string shortHelp)
    {
        Aliases = aliases;
        ShortHelp = shortHelp;
    }

    private string _aliases = string.Empty;
    public string Aliases
    {
        get { return _aliases; } 
        set
        {
            _aliases = value;
            AliasesLength = _aliases.Length;
        }
    }

    public int AliasesLength { get; private set; } = 0;

    public string ShortHelp { get; set; } = string.Empty;
}
