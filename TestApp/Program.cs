using System;
using ConsoleFramework;
using ConsoleFramework.Helpers;

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

        OptionsHelper<DateTime> dateTimeOptions = new(cnsl, "available dates...", true);
        dateTimeOptions.InputPrompt = mainPrompt;
        dateTimeOptions.InputPromptColor = ConsoleColor.White;
        dateTimeOptions.OptionPrefix = " - ";
        dateTimeOptions.OptionsTextColor = ConsoleColor.Yellow;
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

        cnsl.ReadLine();
    }
}
