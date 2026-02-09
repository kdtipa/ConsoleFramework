using ConsoleFramework;
using ConsoleFramework.Helpers;
using System;
using System.Collections.Generic;

namespace TestApp_Args;

internal class Program
{
    static void Main(string[] args)
    {
        var cnsl = new ConsoleClass();

        if (args.Length == 0) { cnsl.WriteColorLine("no arguments", ConsoleColor.Yellow); }
        else
        {
            // show the args as parsed by the system
            cnsl.WriteColorLine("Arguments as parsed by default...", ConsoleColor.DarkGreen);
            cnsl.WriteBulletList(ConsoleColor.DarkGreen, " ∙ ", ConsoleColor.DarkGreen, args);
            cnsl.WriteLine();

            // re-parse the args trying to re-insert double quotes and collapse = with too much space around it.
            var reparsedArgs = ArgHelper.ReParseArgs(args);
            cnsl.WriteColorLine("Re-Parsed arguments...", ConsoleColor.Green);
            foreach (var item in reparsedArgs)
            {
                cnsl.WriteColor(" ∙ ", ConsoleColor.Green);
                cnsl.WriteColorLine($"name: [{item.Name}] / val: [{item.Value}]", ConsoleColor.Green);
            }
        }





        _ = cnsl.ReadLine();

    }
}
