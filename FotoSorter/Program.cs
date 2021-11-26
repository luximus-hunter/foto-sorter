// See https://aka.ms/new-console-template for more information

// Using
using FotoSorter;
using Spectre.Console;
using BarChartItem = FotoSorter.BarChartItem;

// Initial variables
List<string> images = new List<string>();
string rootPath;
string destPath;
Output output = new Output();

List<BarChartItem> fileTypes = new List<BarChartItem>();
fileTypes.Add(new BarChartItem("*.jpg", 0, Color.Orange1));
fileTypes.Add(new BarChartItem("*.jpeg", 0, Color.Purple));
fileTypes.Add(new BarChartItem("*.bmp", 0, Color.Red));
fileTypes.Add(new BarChartItem("*.png", 0, Color.Turquoise2));
fileTypes.Add(new BarChartItem("*.gif", 0, Color.Green));
fileTypes.Add(new BarChartItem("*.ico", 0, Color.CadetBlue_1));

// Get in and output
Console.WriteLine("Root folder to check:");
rootPath = Console.ReadLine();

Console.WriteLine("Destination folder:");
destPath = Console.ReadLine();

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

renameFilesInDirectory(destPath);

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

