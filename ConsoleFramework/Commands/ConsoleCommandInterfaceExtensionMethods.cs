using ConsoleFramework.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleFramework.Commands;

public static class ConsoleCommandInterfaceExtensionMethods
{
    public static void ShowLongHelp(this IConsoleCommand cmd, IConsole cnsl)
    {
        var longHelp = new StringBuilder();
        int availableWidth = cnsl.WindowWidth - 1;
        string hzRl = StringHelper.GenerateHorizontalRule('═', availableWidth) + '\n';

        // command aliases
        longHelp.AppendLine(cmd.Names.ToString());
        longHelp.AppendLine(hzRl);

        // long help text
        foreach (string line in StringHelper.WrapLines(cmd.LongHelp, availableWidth))
        {
            longHelp.AppendLine(line);
        }
        longHelp.AppendLine();

        // argument details
        if (cmd.Parameters.Count > 0)
        {
            longHelp.AppendLine("Command Parameters...");
            int longestName = cmd.Parameters.Values.Max(p => p.Name.Length);
            foreach (var param in cmd.Parameters.Values)
            {
                longHelp.AppendLine(param.ToString(longestName));
            }
        }

        // finish up the text and write the long help message
        longHelp.AppendLine(hzRl);

        cnsl.WriteLine(longHelp.ToString());
    }
}
