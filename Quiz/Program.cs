using System;
using System.IO;
using System.Text;

namespace Quiz
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            string path = "logs.txt";
            Logger logger = new Logger(path);

            logger.Info("Program RUN");

            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("=========> Identification <=========");
                Console.WriteLine("1. Authentication");
                Console.WriteLine("2. Registration");
                Console.WriteLine("0. Exit");
                Console.WriteLine("====================================");

                string input = Console.ReadLine();
                Console.Clear();
                switch (input)
                {
                    case "1":
                        {
                            logger.Info("User requested authentication");
                            Console.Write("Enter login: ");
                            string login = Console.ReadLine();
                            Console.Write("Enter password: ");
                            string password = Console.ReadLine();

                            var (isAuthenticated, isAdmin) = Identification.Authentication(login, password);

                            if (isAuthenticated)
                            {
                                logger.Info($"User {login} successfully logged in");
                                Console.WriteLine("You have successfully logged in.");
                                NormalMenu(isAdmin, login, logger);
                                Console.ReadKey();
                            }
                            else
                            {
                                logger.Warn($"Failed login attempt for user {login}");
                                Console.WriteLine("Press any key to return to the menu...");
                                Console.ReadKey();
                            }
                        }
                        break;

                    case "2":
                        {
                            logger.Info("User requested registration");
                            Console.Write("Enter new login: ");
                            string login = Console.ReadLine();
                            Console.Write("Enter new password: ");
                            string password = Console.ReadLine();
                            Console.Write("Enter data of birsday: ");
                            string data = Console.ReadLine();

                            Identification.Registration(login, password, DateTime.Parse(data));
                            logger.Info($"New user registered with login: {login}");
                        }
                        break;

                    case "0":
                        logger.Info("User exited the program");
                        exit = true;
                        break;

                    default:
                        Console.WriteLine("Please try again.");
                        break;
                }
            }
        }

        static void NormalMenu(bool isAdmin, string login, Logger logger)
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("============> Menu <============");
                Console.WriteLine("1. Start new quiz");
                Console.WriteLine("2. View results");
                Console.WriteLine("3. View quiz questions");
                Console.WriteLine("4. Change settings");

                if (isAdmin)
                {
                    Console.WriteLine("=========> Admin Menu <=========");
                    Console.WriteLine("5. Add quiz");
                    Console.WriteLine("6. Edit quiz");
                    Console.WriteLine("7. Delete quiz");
                }

                Console.WriteLine("================================");
                Console.WriteLine("0. Exit");

                string input = Console.ReadLine();
                Console.Clear();
                switch (input)
                {
                    case "1":
                        logger.Info($"User {login} started new quiz");
                        QuizManagement.ListFilesInDirectory("quizs_files", login, false);
                        break;
                    case "2":
                        logger.Info($"User {login} viewed past results");
                        Results.ViewPastResults();
                        break;
                    case "3":
                        logger.Info($"User {login} viewed quiz questions");
                        QuizManagement.ListFilesInDirectory("quizs_files", login, true);
                        break;
                    case "4":
                        logger.Info($"User {login} edited account settings");
                        AccountManagement.EditAccount(login);
                        break;
                    case "5":
                        if (isAdmin)
                        {
                            logger.Info("Admin added new quiz");
                            QuizManagement.AddQuiz();
                        }
                        break;
                    case "6":
                        if (isAdmin)
                        {
                            logger.Info("Admin edited a quiz");
                            QuizManagement.EditQuiz();
                        }
                        break;
                    case "7":
                        if (isAdmin)
                        {
                            logger.Info("Admin deleted a quiz");
                            QuizManagement.DeleteQuiz();
                        }
                        break;
                    case "0":
                        logger.Info($"User {login} exited the menu");
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Please try again.");
                        break;
                }
            }
        }
    }
}

