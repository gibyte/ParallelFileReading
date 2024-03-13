
using System.Diagnostics;

string pathFolder = "EditFile";
EnsureFolderExists(pathFolder);

string pathFile1 = pathFolder + "\\file1.txt";
string pathFile2 = pathFolder + "\\file2.txt";
string pathFile3 = pathFolder + "\\file3.txt";

string content1 = GenerateRandomContent(10000);
string content2 = GenerateRandomContent(10000);
string content3 = GenerateRandomContent(10000);

CreateFileWithContent(pathFile1, content1);
CreateFileWithContent(pathFile2, content2);
CreateFileWithContent(pathFile3, content3);

// 1. Прочитать 3 файла параллельно и вычислить количество пробелов в них (через Task).

Stopwatch stopwatch = Stopwatch.StartNew();
var fileTasks = new Task<int>[] {
            CountSpacesAsync(pathFile1),
            CountSpacesAsync(pathFile2),
            CountSpacesAsync(pathFile3)
        };
stopwatch.Stop(); // Останавливаем таймер

await Task.WhenAll(fileTasks);
int totalSpaces = fileTasks.Sum(task => task.Result);
Console.WriteLine($"Total spaces in 3 files: {totalSpaces}");
Console.WriteLine($"Time taken: {stopwatch.ElapsedMilliseconds} ms");
Console.WriteLine();
// 2. Написать функцию, принимающую в качестве аргумента путь к папке.
// Из этой папки параллельно прочитать все файлы и вычислить количество пробелов в них.

Stopwatch readFolderStopwatch = Stopwatch.StartNew();
int totalSpacesInFolder = await ReadFilesInFolderAndCountSpacesAsync(pathFolder, "txt");
readFolderStopwatch.Stop();
Console.WriteLine($"Total spaces in folder: {totalSpacesInFolder}");
Console.WriteLine($"Time taken to read folder: {readFolderStopwatch.ElapsedMilliseconds} ms");


static void CreateFileWithContent(string filePath, string content)
{
    File.WriteAllText(filePath, content);
    Console.WriteLine($"File created: {filePath}");
}
static string GenerateRandomContent(int length)
{
    Random random = new Random();
    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 ";
    var randomContent = new string(Enumerable.Repeat(chars, length)
        .Select(s => s[random.Next(s.Length)]).ToArray());
    Console.WriteLine();
    Console.WriteLine(randomContent);
    Console.WriteLine();
    return randomContent;
}

static async Task<int> CountSpacesAsync(string fileName)
{
    string text = await File.ReadAllTextAsync(fileName);
    return text.Count(char.IsWhiteSpace);
}

static void EnsureFolderExists(string pathFolder)
{
    if (!Directory.Exists(pathFolder)) Directory.CreateDirectory(pathFolder);
}

static async Task<int> ReadFilesInFolderAndCountSpacesAsync(string folderPath, string fileExtension = null)
{
    string[] files;

    if (fileExtension != null)
    {
        files = Directory.GetFiles(folderPath, $"*.{fileExtension}");
    }
    else
    {
        files = Directory.GetFiles(folderPath);
    }

    var tasks = files.Select(file => CountSpacesAsync(file)).ToArray();

    await Task.WhenAll(tasks);

    return tasks.Sum(t => t.Result);
}