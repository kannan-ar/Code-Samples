namespace SalesSimulator
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    public class ConsoleLogger
    {
        private BlockingCollection<string> messages = new BlockingCollection<string>();
        private const int tableWidth = 73;
        private void PrintLine()
        {
            Console.WriteLine(new string('-', tableWidth));
        }

        private void PrintRow(params string[] columns)
        {
            int width = (tableWidth - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
                row += AlignCentre(column, width) + "|";
            }

            Console.WriteLine(row);
        }

        private string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
        }

        private void Log()
        {
            PrintLine();
            PrintRow("Column 1", "Column 2", "Column 3", "Column 4");
            PrintLine();
            PrintRow("", "", "", "");
            PrintRow("", "", "", "");
            PrintLine();
        }

        private void OnStart()
        {
            foreach(var message in messages.GetConsumingEnumerable(CancellationToken.None))
            {
                Console.WriteLine(message);
            }
        }
        public ConsoleLogger()
        {
            var thread = new Thread(new ThreadStart(OnStart));
            thread.IsBackground = true;
            thread.Start();
        }

        public void Add(string message)
        {
            messages.Add(message);
        }
    }
}