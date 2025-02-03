using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Quiz;

public static class Results
{
    private static Logger logger = new Logger("logs.txt");

    public static void SaveResult(string quizName, int totalPoints, string login)
    {
        string resultFilePath = "result.txt";
        string resultContent = $"{login}: {quizName} - {totalPoints} points - {DateTime.Now}\n";

        try
        {
            logger.Info($"Saving result for quiz: {quizName}, user: {login}, points: {totalPoints}");
            using (FileStream fileStream = new FileStream(resultFilePath, FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    streamWriter.Write(resultContent);
                }
            }
            logger.Info("Result saved successfully.");
        }
        catch (Exception ex)
        {
            logger.Error($"Error saving result: {ex.Message}");
            Console.WriteLine($"Error saving result: {ex.Message}");
        }
    }

    public static void ViewSortedResults(string quizName)
    {
        string resultFilePath = "result.txt";

        try
        {
            logger.Info($"Fetching sorted results for quiz: {quizName}");
            using (FileStream fileStream = new FileStream(resultFilePath, FileMode.Open, FileAccess.Read))
            using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                if (fileStream.Length == 0)
                {
                    logger.Warn("No results found.");
                    Console.WriteLine("No results found.");
                    return;
                }

                var results = new List<string>();
                while (!streamReader.EndOfStream)
                {
                    results.Add(streamReader.ReadLine());
                }

                var filteredResults = results.Where(r => r.Contains(quizName)).ToList();

                var sortedResults = filteredResults
                    .Select(r =>
                    {
                        var parts = r.Split('-');
                        if (parts.Length < 2) return null;

                        var scorePart = parts[1].Trim().Split(' ')[0];
                        if (!int.TryParse(scorePart, out int points)) return null;

                        return new { Result = r, Points = points };
                    })
                    .Where(r => r != null)
                    .OrderByDescending(r => r.Points)
                    .Select(r => r.Result)
                    .ToList();

                if (sortedResults.Any())
                {
                    Console.WriteLine($"Results for {quizName}:");
                    foreach (var result in sortedResults)
                    {
                        Console.WriteLine(result);
                    }
                }
                else
                {
                    logger.Warn($"No results found for quiz: {quizName}");
                    Console.WriteLine($"No results found for quiz: {quizName}");
                }
            }
        }
        catch (FileNotFoundException)
        {
            logger.Error("Result file not found.");
            Console.WriteLine("No results found.");
        }
        catch (Exception ex)
        {
            logger.Error($"Error reading results: {ex.Message}");
            Console.WriteLine($"Error reading results: {ex.Message}");
        }

        Console.ReadLine();
    }

    public static void ViewPastResults()
    {
        string resultFilePath = "result.txt";

        try
        {
            logger.Info("Fetching past quiz results.");
            using (FileStream fileStream = new FileStream(resultFilePath, FileMode.Open, FileAccess.Read))
            using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                if (fileStream.Length == 0)
                {
                    logger.Warn("No past results found.");
                    Console.WriteLine("No past results found.");
                    return;
                }

                Console.WriteLine("Past quiz results:");
                while (!streamReader.EndOfStream)
                {
                    Console.WriteLine(streamReader.ReadLine());
                }
            }
        }
        catch (FileNotFoundException)
        {
            logger.Error("Past results file not found.");
            Console.WriteLine("No past results found.");
        }
        catch (Exception ex)
        {
            logger.Error($"Error reading past results: {ex.Message}");
            Console.WriteLine($"Error reading past results: {ex.Message}");
        }

        Console.ReadLine();
    }
}
