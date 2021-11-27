// See https://aka.ms/new-console-template for more information

// Using
using FotoSorter;
using Spectre.Console;
using BarChartItem = FotoSorter.BarChartItem;

// Initial variables
List<string> images = new List<string>();
List<string> selectedFileTypes = new List<string>();
string rootPath;
string destPath;
Rule rule;
List<BarChartItem> fileTypes = new List<BarChartItem>();
bool renameFileOnSort = true;
bool showPassLogs = true;

#region Typed inputs

// Input
rule = new Rule("[silver]Input[/]");
rule.Alignment = Justify.Left;
rule.Style = Style.Parse("gray");
AnsiConsole.Write(rule);
rootPath = AnsiConsole.Ask<string>("What is the [green]input[/] folder?");
AnsiConsole.WriteLine();

// Output
rule = new Rule("[silver]Output[/]");
rule.Alignment = Justify.Left;
rule.Style = Style.Parse("gray");
AnsiConsole.Write(rule);
destPath = AnsiConsole.Ask<string>("What is the [green]output[/] folder?");
AnsiConsole.WriteLine();

// Sort all file filetypes
rule = new Rule("[silver]Image types[/]");
rule.Alignment = Justify.Left;
rule.Style = Style.Parse("gray");
AnsiConsole.Write(rule);

if (AnsiConsole.Confirm("Specify [green]image types[/] to sort?", false))
{
    IEnumerable<string> fileTypeChoicesJPG = new string[] { ".jpg", ".jpeg", ".jpe", ".jif", ".jfif", ".jfi" };
    IEnumerable<string> fileTypeChoicesTIFF = new string[] { ".tiff", ".tif" };
    IEnumerable<string> fileTypeChoicesRAW = new string[] { ".raw", ".arw", ".cr2", ".nrw", ".k25" };
    IEnumerable<string> fileTypeChoicesBMP = new string[] { ".bmp", ".dib" };
    IEnumerable<string> fileTypeChoicesHEIF = new string[] { ".heif", ".heic" };
    IEnumerable<string> fileTypeChoicesINDD = new string[] { ".ind", ".indd", ".indt" };
    IEnumerable<string> fileTypeChoicesJPEG2000 = new string[] { ".jp2", ".j2k", ".jpf", ".jpx", ".jpm", ".mj2" };
    IEnumerable<string> fileTypeChoicesSVG = new string[] { ".svg", ".svgz" };

    AnsiConsole.WriteLine();
    rule = new Rule("[silver]Select image types[/]");
    rule.Alignment = Justify.Left;
    rule.Style = Style.Parse("gray");
    AnsiConsole.Write(rule);

    selectedFileTypes = AnsiConsole.Prompt(
        new MultiSelectionPrompt<string>()
            .Title("What [green]type of images[/] do you want to sort?")
            .PageSize(15)
            .MoreChoicesText("[grey](Move up and down to reveal more fruits)[/]")
            .InstructionsText(
                "[grey](Press [blue]<space>[/] to toggle an image type, " +
                "[green]<enter>[/] to accept)[/]")
            .AddChoiceGroup("JPG", fileTypeChoicesJPG)
            .AddChoices(new[] { ".png", ".gif", ".webp" })
            .AddChoiceGroup("TIFF", fileTypeChoicesTIFF)
            .AddChoices(new[] { ".psd" })
            .AddChoiceGroup("RAW", fileTypeChoicesRAW)
            .AddChoiceGroup("BMP", fileTypeChoicesBMP)
            .AddChoiceGroup("HEIF", fileTypeChoicesHEIF)
            .AddChoiceGroup("INDD", fileTypeChoicesINDD)
            .AddChoiceGroup("JPEG 2000", fileTypeChoicesJPEG2000)
            .AddChoiceGroup("SVG", fileTypeChoicesSVG)
            .AddChoices(new[] { ".ai", ".eps", ".pdf", ".ico" }
        )
    );
    AnsiConsole.WriteLine();
}
else
{
    AnsiConsole.WriteLine("Sorting all image types");
    AnsiConsole.WriteLine();
    selectedFileTypes.AddRange(new string[] { 
        ".jpg", ".jpeg", ".jpe", ".jif", ".jfif", 
        ".jfi", ".png", ".gif", ".webp", ".tiff", 
        ".tif", ".psd", ".raw", ".arw", ".cr2", 
        ".nrw", ".k25", ".bmp", ".dib", ".heif", 
        ".heic", ".ind", ".indd", ".indt", ".jp2", 
        ".j2k", ".jpf", ".jpx", ".jpm", ".mj2", 
        ".svg", ".svgz", ".ai", ".eps", ".pdf", 
        ".ico" 
    });
}

foreach (string selectedFileType in selectedFileTypes)
{
    fileTypes.Add(new BarChartItem($"*{selectedFileType}", 0, Color.Orange1));
}

// Renameing
rule = new Rule("[silver]Rename images[/]");
rule.Alignment = Justify.Left;
rule.Style = Style.Parse("gray");
AnsiConsole.Write(rule);

if (!AnsiConsole.Confirm("[green]Rename[/] images after sorting?")) 
{
    renameFileOnSort = false;
}

AnsiConsole.WriteLine();

// Logging
rule = new Rule("[silver]Ignore success[/]");
rule.Alignment = Justify.Left;
rule.Style = Style.Parse("gray");
AnsiConsole.Write(rule);

if (!AnsiConsole.Confirm("[green]Ignore[/] success messages and only show fails?"))
{
    showPassLogs = false;
}

#endregion

// Init output
Output output = new Output(showPassLogs);

// Fill list
checkFilesInDirectory(rootPath);

// Copy files
foreach (string path in images)
{
  List<string> pathSplit = path.Split(@"\").ToList();
  pathSplit.Reverse();
  string imageName = pathSplit[0];

  Directory.CreateDirectory(destPath);

  if (File.Exists(Path.Combine(destPath, imageName)))
  {
        output.Skip("File exists " + Path.Combine(destPath, imageName));
  }
  else
  {
    try
    {
      File.Copy(path, Path.Combine(destPath, imageName), true);
            output.Pass("Copied " + path);
    }
    catch (Exception)
    {
      AnsiConsole.MarkupLine("[red](fail)[/] Failed copying " + path);
            output.Fail("Couldn't copy " + path);
    }
  }

}

DirectoryInfo destinationDirectoryInfo = new DirectoryInfo(destPath);

foreach (FileInfo imageInfo in destinationDirectoryInfo.GetFiles())
{
  string newPath = Path.Combine(
      destinationDirectoryInfo.FullName,
      imageInfo.LastWriteTime.Year.ToString(),
      getMonthFullName(imageInfo.LastWriteTime.Month)
  );

  Directory.CreateDirectory(
      Path.Combine(destinationDirectoryInfo.FullName,
      imageInfo.LastWriteTime.Year.ToString(),
      getMonthFullName(imageInfo.LastWriteTime.Month))
  );

  if (File.Exists(Path.Combine(newPath, imageInfo.Name)))
  {
        output.Skip("Already sorted " + Path.Combine(newPath, imageInfo.Name));
  }
  else
  {
    try
    {
      File.Copy(imageInfo.FullName, Path.Combine(newPath, imageInfo.Name));
            output.Pass("Sorted " + imageInfo.FullName);
      File.Delete(imageInfo.FullName);
    }
    catch (Exception)
        {
            output.Pass("Failed sorting " + imageInfo.FullName);
        }
  }
}

destinationDirectoryInfo = new DirectoryInfo(destPath);

if(renameFileOnSort)
{
    renameFilesInDirectory(destPath);
}

foreach (BarChartItem fileTypesCount in fileTypes)
{
    output.AddChartItem(fileTypesCount);
}

output.Chart(Console.WindowWidth);

// Functions
void checkFilesInDirectory(string directory)
{
  DirectoryInfo directoryInfo = new DirectoryInfo(directory);

    List<BarChartItem> fileTypesLooper = new List<BarChartItem>();

    foreach (BarChartItem fileType in fileTypes)
    {
        fileTypesLooper.Add(fileType); 
    }

    foreach (BarChartItem fileType in fileTypesLooper)
  {
    int count = 0;

    foreach (FileInfo file in directoryInfo.GetFiles(fileType.Label))
    {
      images.Add(file.FullName);
            count++;
    }

    int index = fileTypes.IndexOf(fileType);

    BarChartItem barChartItem = fileTypes[index];
    barChartItem.Value += count;

    fileTypes[index] = barChartItem;        
  }

  foreach (DirectoryInfo subDirectory in directoryInfo.GetDirectories())
  {
    checkFilesInDirectory(subDirectory.FullName);
  }
}

void renameFilesInDirectory(string directory)
{
  DirectoryInfo directoryInfo = new DirectoryInfo(directory);

  int count = 0;

  foreach (FileInfo imageInfo in directoryInfo.GetFiles())
  {
    string name = "IMG-";

    for (int i = 0; i < 4 - count.ToString().Length; i++)
    {
      name += "0";
    }

    name += count.ToString() + imageInfo.Extension;

    count++;

    if (File.Exists(Path.Combine(directoryInfo.FullName, name)))
    {
            output.Skip("Already renamed " + Path.Combine(directoryInfo.FullName, name));
        }
    else
    {
      try
      {
        File.Move(imageInfo.FullName, Path.Combine(directoryInfo.FullName, name));
                output.Pass("Renamed " + imageInfo.FullName);
            }
      catch (Exception)
            {
                output.Pass("Failed renaming " + imageInfo.FullName);
            }
    }
  }

  foreach (DirectoryInfo subDirectory in directoryInfo.GetDirectories())
  {
    renameFilesInDirectory(subDirectory.FullName);
  }
}

string getMonthFullName(int monthNumber)
{
  switch (monthNumber)
  {
    case 1:
      return "01-Januari";
    case 2:
      return "02-February";
    case 3:
      return "03-March";
    case 4:
      return "04-April";
    case 5:
      return "05-May";
    case 6:
      return "06-June";
    case 7:
      return "07-Juli";
    case 8:
      return "08-August";
    case 9:
      return "09-September";
    case 10:
      return "10-October";
    case 11:
      return "11-November";
    case 12:
      return "12-December";
    default:
      return "00_invalid-month";
  }
}

