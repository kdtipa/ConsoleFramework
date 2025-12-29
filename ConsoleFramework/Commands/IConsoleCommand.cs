using ConsoleFramework.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleFramework.Commands;

public interface IConsoleCommand
{
    public CommandName Names { get; }

    public string ShortHelp { get; }

    public string LongHelp { get; }

    public AutoSortList<CommandParameter> Parameters { get; }

    /// <summary>
    /// This is what gets called to execute your command.  Pass in any relevant 
    /// arguments and parse them in the method's code.  Then do whatever this 
    /// command is supposed to do.
    /// </summary>
    /// <param name="cnsl">
    /// A reference to your IConsole object for output and for any user interactions 
    /// that might be necessary.
    /// </param>
    /// <param name="args">The list of data your command needs in order to run</param>
    /// <returns>
    /// The ConsoleCommandReturn class has a set of properties that should allow you 
    /// to convey whatever you need to the calling code.  If you don't need anything, 
    /// you can just return null.
    /// </returns>
    public ConsoleCommandReturn? Run(IConsole cnsl, params string[] args);
}
