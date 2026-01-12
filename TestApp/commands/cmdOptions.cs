using ConsoleFramework;
using ConsoleFramework.Commands;
using ConsoleFramework.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.commands;

public class cmdOptions : IConsoleCommand
{
    public CommandName Names { get; } = new CommandName("Options", "Option", "Opt", "O");

    public string ShortHelp { get; } = "Run this command to open up the test for our options menu in a console.";

    public string LongHelp { get; } = "Run this command to open up the test for our options menu in a console.";

    public ConsoleCommandReturn? Run(IConsole cnsl, params string[] args)
    {
        if (args.Length == 1)
        {
            ConsoleCommandHelper.BreakUpInput(args[0], out var uc, out var ua);
            var newList = new List<string>();
            if (!string.IsNullOrEmpty(uc)) { newList.Add(uc); }
            if (ua is not null && ua.Count > 0) { newList.AddRange(ua); }
            return Run(cnsl, newList);
        }

        if (args.Length > 1) { return Run(cnsl, args.ToList()); }

        return Run(cnsl, new List<string>());
    }

    public ConsoleCommandReturn? Run(IConsole cnsl, List<string> args)
    {
        OptionsHelper<DateTime> dateTimeOptions = new(cnsl, "available dates...", true);
        dateTimeOptions.InputPrompt = "= ";
        dateTimeOptions.InputPromptColor = ConsoleColor.White;
        dateTimeOptions.OptionPrefix = " • ";
        dateTimeOptions.OptionsTextColor = ConsoleColor.Green;
        dateTimeOptions.OptionsBackgroundColor = ConsoleColor.DarkGray;
        dateTimeOptions.OptionsTextHighlight = ConsoleColor.DarkGray;
        dateTimeOptions.OptionsBackgroundHighlight = ConsoleColor.Yellow;

        DateTime worker = DateTime.Now;

        for (int incr = 1; incr <= 5; incr++)
        {
            worker = worker.AddDays(4);
            dateTimeOptions.Options.Add(new OptionsHelperItem<DateTime>(worker.ToString("ddd, yyyy-MM-dd"), worker));
        }

        if (dateTimeOptions.RunOptionList(out var result))
        {
            cnsl.WriteColorLine($"HOORAY!  We got a selection of {result.Value.Text}!", ConsoleColor.Yellow);
        }
        else
        {
            cnsl.WriteColorLine($"Boo... no selection was made", ConsoleColor.DarkRed);
        }

        return new ConsoleCommandReturn(true);
    }
}
