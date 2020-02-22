using System;
using System.Collections.Generic;
using System.Text;

namespace AWSLambda.AspNetCoreInterop.LocalRouter
{
    public static class ConsoleUtil
    {
        const int colCount = 79;
        public static readonly string LinesDouble = new string('=', colCount);
        public static readonly string LinesSingle = new string('-', colCount);

        public static void WriteProgramTitle(string title)
        {
            string lines = new string('=', colCount);

            int titleLeftPadding = (colCount - title.Length) / 2 + title.Length;

            Console.WriteLine(lines);
            Console.WriteLine(title.PadLeft(titleLeftPadding));
            Console.WriteLine(lines);
        }
    }
}
