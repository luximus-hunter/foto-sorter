using Spectre.Console;

namespace FotoSorter
{
    internal class Output
    {
        private List<BarChartItem> barChartItems;
        private bool ignorePass;
        private bool ignoreSkip;

        public Output(bool ignorePass = false, bool ignoreSkip = false)
        {
            barChartItems = new List<BarChartItem>();
            this.ignorePass = ignorePass;
            this.ignoreSkip = ignoreSkip;
        }

        public void Pass(string text)
        {
            if(!ignorePass)
            {
                AnsiConsole.MarkupLine("[green](pass)[/] " + text);
            }
        }

        public void Skip(string text)
        {
            if(!ignoreSkip)
            {
                AnsiConsole.MarkupLine("[orange3](skip)[/] " + text);
            }
        }

        public void Fail(string text)
        {
            AnsiConsole.MarkupLine("[red](fail)[/] " + text);
        }

        public void AddChartItem(BarChartItem item)
        {
            barChartItems.Add(item);
        }

        public void Chart(int width)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new BarChart()
                .Width(width)
                .Label("[bold underline]Amount per file type[/]")
                .AddItems(barChartItems));
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
