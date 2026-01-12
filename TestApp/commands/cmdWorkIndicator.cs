using ConsoleFramework;
using ConsoleFramework.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestApp.commands;

public class cmdWorkIndicator : IConsoleCommand
{
    public CommandName Names { get; } = new CommandName("WorkIndicator", "WI", "W");

    public string ShortHelp { get; } = "Shows an example of a work indicator that lasts for 5 seconds by default.";

    public string LongHelp { get; } = "Shows an example of a work indicator that lasts for 5 seconds by default.";

    public ConsoleCommandReturn? Run(IConsole cnsl, params string[] args)
    {
        return Run(cnsl, args.ToList());
    }

    public ConsoleCommandReturn? Run(IConsole cnsl, List<string> args)
    {
        int runTime = 5;
        foreach (string arg in args)
        {
            if (int.TryParse(arg, out int parsedVal) && parsedVal > 0 && parsedVal <= 20)
            {
                runTime = parsedVal;
                break;
            }
        }

        // make sure we're on our own line...
        cnsl.WriteLine();
        cnsl.CursorVisible = false;

        char[] cycleChars = { '-', '\\', '|', '/' };
        int currentIndex = 0;
        var StopTime = DateTime.Now.AddSeconds(runTime);


        while (DateTime.Now <= StopTime)
        {
            cnsl.Write(cycleChars[currentIndex]);
            currentIndex++;
            if (currentIndex > cycleChars.Length - 1) { currentIndex = 0; }

            Thread.Sleep(500);

            cnsl.CursorLeft--;
        }

        cnsl.WriteLine("complete");
        cnsl.CursorVisible = true;

        return null;
    }
}
