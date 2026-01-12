using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleFramework.Commands;

public static class ConsoleCommandHelper
{
    public static void BreakUpInput(string? userInput, out string? userCommand, out List<string>? arguments)
    {
        userCommand = null;
        arguments = null;

        if (string.IsNullOrWhiteSpace(userInput)) { return; }

        string cleanSource = userInput.Trim();

        var workList = new List<string>();
        var worker = new StringBuilder();
        bool inDoubleQuotes = false;
        bool inSingleQuotes = false;

        foreach (char c in cleanSource)
        {
            if (inDoubleQuotes)
            {
                if (c == '"')
                {
                    worker.Append(c);
                    workList.Add(worker.ToString());
                    worker.Clear();
                    inDoubleQuotes = false;
                }
                else { worker.Append(c); }

                continue;
            }

            if (inSingleQuotes)
            {
                if (c == '\'')
                {
                    worker.Append(c);
                    workList.Add(worker.ToString());
                    worker.Clear();
                    inSingleQuotes = false;
                }
                else { worker.Append(c); }

                continue;
            }

            if (c == ' ')
            {
                // we're not inside quotes, so we break up the words
                if (worker.Length > 0)
                {
                    workList.Add(worker.ToString());
                    worker.Clear();
                }

                continue;
            }

            if (c == '"')
            {
                // count this as a word break
                if (worker.Length > 0)
                {
                    workList.Add(worker.ToString());
                    worker.Clear();
                }

                worker.Append(c);
                inDoubleQuotes = true;
                continue;
            }

            if (c == '\'')
            {
                // count this as a word break
                if (worker.Length > 0)
                {
                    workList.Add(worker.ToString());
                    worker.Clear();
                }

                worker.Append(c);
                inSingleQuotes = true;
                continue;
            }

            worker.Append(c);
        }

        // there might still be something in the worker
        if (worker.Length > 0) { workList.Add(worker.ToString()); }

        // transfer results to out parameters
        int resultCount = workList.Count;

        if (resultCount > 0) { userCommand = workList[0]; }

        if (resultCount > 1)
        {
            arguments = new List<string>();
            for (int i = 1; i < resultCount; i++)
            {
                arguments.Add(workList[i]);
            }
        }
        

    }





}
