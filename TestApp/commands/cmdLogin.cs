using ConsoleFramework;
using ConsoleFramework.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.commands;

public class cmdLogin : IConsoleCommand
{
    public CommandName Names { get; } = new CommandName("Login", "L", "Auth", "A");

    public string ShortHelp { get; } = "A sample login interface using the fancy masked read line methods from this library.";

    public string LongHelp { get; } = "A sample login interface using the fancy masked read line methods from this library.";

    public ConsoleCommandReturn? Run(IConsole cnsl, params string[] args)
    {
        return Run(cnsl, new List<string>());
    }

    public ConsoleCommandReturn? Run(IConsole cnsl, List<string> args)
    {
        var userName = cnsl.ReadLine("username: ", ConsoleColor.DarkGray, ConsoleColor.White);
        var password = cnsl.ReadLine(InputMaskingOptions.Hide, '*', "password: ", ConsoleColor.DarkGray, ConsoleColor.White);

        cnsl.WriteLine($"Got user name [{userName}] and password [{password}]");
        return null;
    }
}
