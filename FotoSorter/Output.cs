using Spectre.Console;
using System.Text;

namespace FotoSorter
{
    internal class Output
    {
        private List<BarChartItem> barChartItems;
        private bool ignorePass;
        private bool ignoreSkip;
        private List<string> logPassed;
        private List<string> logSkipped;
        private List<string> logFailed;

        public Output(bool ignorePass = false, bool ignoreSkip = false)
        {
            barChartItems = new List<BarChartItem>();
            this.ignorePass = ignorePass;
            this.ignoreSkip = ignoreSkip;
            logPassed = new List<string>();
            logSkipped = new List<string>();
            logFailed = new List<string>();
        }

        public void Pass(string text)
        {
            if(!ignorePass)
            {
                AnsiConsole.MarkupLine("[green](pass)[/] " + text);
            }

            logPassed.Add(text.Trim());
        }

        public void Skip(string text)
        {
            if(!ignoreSkip)
            {
                AnsiConsole.MarkupLine("[orange3](skip)[/] " + text);
            }

            logSkipped.Add(text.Trim());
        }

        public void Fail(string text)
        {
            AnsiConsole.MarkupLine("[red](fail)[/] " + text);

            logFailed.Add(text.Trim());
        }

        public void AddChartItem(BarChartItem item)
        {
            if(item.Value > 0)
            {
                barChartItems.Add(item);
            }
        }

        public void Chart(int width)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new BarChart()
                .Width(width)
                .Label("[bold underline]Amount per file type[/]")
                .AddItems(barChartItems));
        }

        public void ExportLog(string path)
        {
            string file = Path.Combine(path, "sort.log");

            // Check if file already exists. If yes, delete it.     
            if (File.Exists(file))
            {
                File.Delete(file);
            }

            // Create a new file     
            using (FileStream fs = File.Create(file))
            {
                byte[] text;
                
                text = new UTF8Encoding(true).GetBytes($"Image Sorter log {DateTime.Now.ToLongTimeString()}\n");
                fs.Write(text, 0, text.Length);

                text = new UTF8Encoding(true).GetBytes("\nFails:\n");
                fs.Write(text, 0, text.Length);

                foreach (string log in logFailed)
                {
                    text = new UTF8Encoding(true).GetBytes(log + "\n");
                    fs.Write(text, 0, text.Length);
                }

                text = new UTF8Encoding(true).GetBytes("\nSkips:\n");
                fs.Write(text, 0, text.Length);

                foreach (string log in logSkipped)
                {
                    text = new UTF8Encoding(true).GetBytes(log + "\n");
                    fs.Write(text, 0, text.Length);
                }

                text = new UTF8Encoding(true).GetBytes("\nPasses:\n");
                fs.Write(text, 0, text.Length);

                foreach (string log in logPassed)
                {
                    text = new UTF8Encoding(true).GetBytes(log + "\n");
                    fs.Write(text, 0, text.Length);
                }
            }
        }
    }

    public sealed class BarChartItem : IBarChartItem
    {
        public string Label { get; set; }
        public double Value { get; set; }
        public Color? Color { get; set; }

        public BarChartItem(string label, double value, Color? color = null)
        {
            Label = label;
            Value = value;
            Color = color;
        }
    }
}
