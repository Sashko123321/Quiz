using System;
using System.IO;
using System.Text;

namespace Quiz;

internal class Identification
{
    private static string path = "accounts.txt";
    private static Logger logger = new Logger("logs.txt");


    public static (bool, bool) Authentication(string login, string password)
    {
        Console.OutputEncoding = Encoding.UTF8;

        try
        {
            logger.Info($"Attempting to authenticate user: {login}");
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (StreamReader reader = new StreamReader(fileStream, Encoding.UTF8))
            {
                if (fileStream.Length == 0)
                {
                    logger.Warn("Account file is empty.");
                    Console.WriteLine("Account file is empty.");
                    return (false, false);
                }

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] data = line.Split(',');

                    if (data.Length == 4)
                    {
                        string storedLogin = data[0].Trim();
                        string storedPassword = data[1].Trim();
                        bool isAdmin = data[3].Trim().Equals("true", StringComparison.OrdinalIgnoreCase);

                        if (storedLogin == login && storedPassword == password)
                        {
                            logger.Info("Authentication successful.");
                            Console.WriteLine("Authentication successful!");
                            return (true, isAdmin);
                        }
                    }
                }
            }
        }
        catch (FileNotFoundException)
        {
            logger.Error("Account file not found.");
            Console.WriteLine("Account file not found.");
        }
        catch (Exception ex)
        {
            logger.Error($"An error occurred: {ex.Message}");
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        logger.Warn("Invalid login or password.");
        Console.WriteLine("Invalid login or password");
        return (false, false);
    }

    public static bool Registration(string login, string password, DateTime date)
    {
        Console.OutputEncoding = Encoding.UTF8;

        try
        {
            logger.Info($"Attempting to register user: {login}");
            using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read))
            using (StreamReader reader = new StreamReader(fileStream, Encoding.UTF8))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] data = line.Split(',');

                    if (data.Length > 0 && data[0].Trim().Equals(login, StringComparison.OrdinalIgnoreCase))
                    {
                        logger.Warn($"Registration failed: Login '{login}' already exists.");
                        Console.WriteLine("This login already exists. Please choose a different login.");
                        return false;
                    }
                }
            }

            using (FileStream fileStream = new FileStream(path, FileMode.Append, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(fileStream, Encoding.UTF8))
            {
                writer.WriteLine($"{login},{password},{date:yyyy-MM-dd},false");
            }

            logger.Info($"Registration successful for user: {login}");
            Console.WriteLine("Registration successful!");
            return true;
        }
        catch (Exception ex)
        {
            logger.Error($"An error occurred during registration: {ex.Message}");
            Console.WriteLine($"An error occurred: {ex.Message}");
            return false;
        }
    }
}
