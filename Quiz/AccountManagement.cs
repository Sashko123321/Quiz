using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Quiz;

public static class AccountManagement
{
    private static Logger logger = new Logger("logs.txt");

    public static void EditAccount(string login)
    {
        string filePath = "accounts.txt";

        try
        {
            logger.Info($"Attempting to edit account: {login}");
            List<string> accounts = new List<string>();

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (StreamReader reader = new StreamReader(fileStream, Encoding.UTF8))
            {
                if (fileStream.Length == 0)
                {
                    logger.Warn("Accounts file is empty!");
                    Console.WriteLine("Accounts file is empty!");
                    return;
                }

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        accounts.Add(line);
                    }
                }
            }

            for (int i = 0; i < accounts.Count; i++)
            {
                string[] data = accounts[i].Split(',');
                if (data[0] == login)
                {
                    logger.Info($"Account found for login: {login}");
                    Console.Write("Do you want to change your password? (yes/no): ");
                    string changePassword = Console.ReadLine().ToLower();
                    string newPassword = data[1];

                    if (changePassword == "yes" || changePassword == "y")
                    {
                        Console.Write("Enter new password: ");
                        newPassword = Console.ReadLine();
                        logger.Info("Password changed successfully.");
                    }

                    Console.Write("Do you want to change your birth date? (yes/no): ");
                    string changeDate = Console.ReadLine().ToLower();
                    string newDate = data[2];

                    if (changeDate == "yes" || changeDate == "y")
                    {
                        Console.Write("Enter new birth date (yyyy-MM-dd): ");
                        newDate = Console.ReadLine();
                        logger.Info("Birth date changed successfully.");
                    }

                    accounts[i] = $"{data[0]},{newPassword},{newDate},{data[3]}";

                    using (FileStream writeStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    using (StreamWriter writer = new StreamWriter(writeStream, Encoding.UTF8))
                    {
                        foreach (var account in accounts)
                        {
                            writer.WriteLine(account);
                        }
                    }

                    logger.Info("Account updated successfully.");
                    Console.WriteLine("Account updated successfully!");
                    return;
                }
            }

            logger.Warn($"Account with login {login} not found.");
            Console.WriteLine("Account not found!");
        }
        catch (FileNotFoundException)
        {
            logger.Error("Accounts file not found!");
            Console.WriteLine("Accounts file not found!");
        }
        catch (Exception ex)
        {
            logger.Error($"An error occurred: {ex.Message}");
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
