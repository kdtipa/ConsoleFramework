using ConsoleFramework;
using ConsoleFramework.Commands;
using ConsoleFramework.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.commands;

public class cmdFramedText : IConsoleCommand
{
    public CommandName Names { get; } = new CommandName("FrameText", "FT", "F");

    public string ShortHelp { get; } = "Just gives an example block of text framed with double lines.";

    public string LongHelp { get; } = "Just gives an example block of text framed with double lines.";

    public ConsoleCommandReturn? Run(IConsole cnsl, params string[] args)
    {
        return Run(cnsl, args.ToList());
    }

    public ConsoleCommandReturn? Run(IConsole cnsl, List<string> args)
    {
        foreach (string line in CharHelperLines.FrameText(exampleText, cnsl.WindowWidth - 5, true, true))
        {
            cnsl.WriteColorLine(line, ConsoleColor.Yellow);
        }

        return null;
    }


    private string exampleText = "George Lucas is an amazing story teller when he has creative " 
        + "people around him who are willing to push back on his ideas to help refine them.  " 
        + "The original trilogy even shows the progression where by the end with Return of the "
        + "Jedi, no one pushed back on the Rube Goldberg rescue plan and no one pushed back on "
        + "Ewoks replacing Wookiees.  By the Prequels, no one pushed back at all, we got poorly "
        + "crafted backbone of setting and events where Jedi are military commanders somehow and "
        + "they're willing to use living things that can suffer as soldiers.  It would have made "
        + "more sense anyway if the Separatists were the cloners who based their economy on having "
        + "access to cheap but non-droid labor that can fill roles droids struggle with like "
        + "nannies, teachers, clerks, doctors that people can feel comfortable with, and yes..." 
        + "sex-workers.  The separatists are then the ones that can stand up a clone army and the "
        + "good guys are the ones that have to fall back on easily produced droids.  But this is "
        + "just scratching the surface of how it SHOULD have been.  Yet even with the flaws, Star "
        + "Wars is probably the most influential fiction that exists.  Lucas obviously succeeded.  "
        + "It just would have been nice if he had crafted the work a little more carefully and "
        + "recognized that he needed help creatively.";
}
