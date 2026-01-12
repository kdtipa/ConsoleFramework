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

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("test");
        Console.ForegroundColor = ConsoleColor.Green;

        cnsl.WriteColorLine("Welcome to the test app for the ConsoleFramework", ConsoleColor.Blue);
        cnsl.WriteColorLine($"arguments received from command line: {args.Length}", ConsoleColor.DarkBlue);

        string mainPrompt = ">> ";
        bool keepGoing = true;
        cmdExit exit = new();

        List<IConsoleCommand> cmds = new()
        {
            new cmdExit(), 
            new cmdOptions(),
            new cmdLogin()
        };

        while (keepGoing)
        {
            var userInput = cnsl.ReadLine(mainPrompt, ConsoleColor.DarkGray, ConsoleColor.White);

            if (string.IsNullOrWhiteSpace(userInput)) { continue; }

            ConsoleCommandHelper.BreakUpInput(userInput, out var userCmd, out var userArgs);

            if (string.IsNullOrEmpty(userCmd)) { continue; }

            var matchingCmd = cmds.Where(cmd => cmd.IsMatch(userCmd)).FirstOrDefault();

            if (matchingCmd is null) { cnsl.WriteColorLine($"No command matches for [{userCmd}]\n", ConsoleColor.Yellow); continue; }

            if (userArgs is null) { userArgs = new List<string>(); }
            var cmdResult = matchingCmd.Run(cnsl, userArgs);

            if (cmdResult is not null)
            {
                if (cmdResult.Value.IsExitRequest) { keepGoing = false; continue; }
            }
        }


    }
}
