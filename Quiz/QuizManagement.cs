using Quiz;
using System.Text;

public static class QuizManagement
{
    private static Logger logger = new Logger("logs.txt");

    public static void StartQuiz(string filePath, string quizName, string login)
    {
        int totalPoints = 0;
        int totalQuestions = 0;

        try
        {
            logger.Info($"Quiz '{quizName}' started by user '{login}'.");

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        totalQuestions++;
                        string[] parts = line.Split('&');
                        if (parts.Length == 2)
                        {
                            string pointsPart = parts[0];
                            string questionAndAnswers = parts[1];

                            string[] questionParts = questionAndAnswers.Split('|');
                            if (questionParts.Length == 2)
                            {
                                string questionText = questionParts[0];
                                string answersPart = questionParts[1];

                                string[] answers = answersPart.Split(',');
                                Console.WriteLine($"Question: {questionText}");
                                for (int i = 0; i < answers.Length; i++)
                                {
                                    string[] answerParts = answers[i].Split('~');
                                    if (answerParts.Length == 2)
                                    {
                                        string answerText = answerParts[0].Trim();
                                        bool isCorrect = answerParts[1].Trim().ToLower() == "true";

                                        Console.WriteLine($"{i + 1}. {answerText}");
                                    }
                                }

                                Console.Write("Enter your answer (1-4): ");
                                int userAnswer;
                                bool isValid = int.TryParse(Console.ReadLine(), out userAnswer) && userAnswer >= 1 && userAnswer <= 4;

                                if (isValid)
                                {
                                    string[] correctAnswerParts = answers[userAnswer - 1].Split('~');
                                    bool isCorrectAnswer = correctAnswerParts[1].Trim().ToLower() == "true";

                                    if (isCorrectAnswer)
                                    {
                                        Console.WriteLine("Correct!");
                                        totalPoints += int.Parse(pointsPart);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Incorrect!");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Invalid input.");
                                }
                                Console.Clear();
                            }
                        }
                    }
                }
            }

            Console.WriteLine($"You scored {totalPoints} out of {totalQuestions}");
            Results.SaveResult(quizName, totalPoints, login);
            Results.ViewSortedResults(quizName);

            logger.Info($"Quiz '{quizName}' completed by user '{login}' with {totalPoints} points.");
        }
        catch (Exception ex)
        {
            logger.Error($"Error starting quiz '{quizName}' for user '{login}': {ex.Message}");
            Console.WriteLine($"Error: {ex.Message}");
        }
    }


    public static void AddQuiz()
    {
        Console.Write("Enter quiz category: ");
        string category = Console.ReadLine();
        string filePath = $"quizs_files/{category}.txt";

        List<string> questions = new List<string>();
        for (int i = 0; i < 2; i++)
        {
            Console.Write($"Enter question {i + 1}: ");
            string question = Console.ReadLine();

            List<string> answers = new List<string>();
            for (int j = 0; j < 4; j++)
            {
                Console.Write($"Enter answer {j + 1}: ");
                string answerText = Console.ReadLine();
                Console.Write($"Is this answer correct? (true/false): ");
                string isCorrect = Console.ReadLine().ToLower();
                answers.Add($"{answerText}~{isCorrect}");
            }

            Console.Write("Enter points: ");
            string points = Console.ReadLine();

            questions.Add($"{points}&{question}|{string.Join(", ", answers)};");
        }

        try
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write))
            using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
            {
                foreach (var question in questions)
                {
                    streamWriter.WriteLine(question);
                }
            }

            Console.WriteLine("Quiz added successfully!");
            logger.Info($"Quiz in category '{category}' added successfully.");
        }
        catch (Exception ex)
        {
            logger.Error($"Error adding quiz in category '{category}': {ex.Message}");
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.ReadKey();
    }

    public static void EditQuiz()
    {
        Console.Write("Enter quiz category to edit: ");
        string category = Console.ReadLine();
        string filePath = $"quizs_files/{category}.txt";

        try
        {
            logger.Info($"Attempt to edit quiz in category '{category}'.");

            string[] questions;

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                if (fileStream.Length == 0)
                {
                    Console.WriteLine("Quiz file is empty!");
                    return;
                }

                questions = streamReader.ReadToEnd().Split(';', StringSplitOptions.RemoveEmptyEntries);
            }

            Console.WriteLine("\nCurrent Questions:");
            for (int i = 0; i < questions.Length; i++)
            {
                string[] parts = questions[i].Split('&');
                if (parts.Length < 2)
                {
                    Console.WriteLine($"{i + 1}. [INVALID FORMAT]");
                    continue;
                }

                string[] questionParts = parts[1].Split('|');
                if (questionParts.Length < 1)
                {
                    Console.WriteLine($"{i + 1}. [INVALID FORMAT]");
                    continue;
                }

                Console.WriteLine($"{i + 1}. {questionParts[0]}");
            }

            Console.Write("Enter question number to edit: ");
            if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > questions.Length)
            {
                Console.WriteLine("Invalid question number.");
                return;
            }
            index--;

            Console.Write("Enter new question text: ");
            string newQuestion = Console.ReadLine();

            List<string> answers = new List<string>();
            for (int j = 0; j < 4; j++)
            {
                Console.Write($"Enter new answer {j + 1}: ");
                string answerText = Console.ReadLine();
                Console.Write($"Is answer {j + 1} correct? (true/false): ");
                string isCorrect = Console.ReadLine().ToLower();
                answers.Add($"{answerText}~{isCorrect}");
            }

            Console.Write("Enter new points: ");
            string points = Console.ReadLine();

            questions[index] = $"{points}&{newQuestion}|{string.Join(", ", answers)};";

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
            {
                foreach (var question in questions)
                {
                    streamWriter.WriteLine(question);
                }
            }

            Console.WriteLine("Question updated successfully!");
            logger.Info($"Quiz in category '{category}' edited successfully.");
        }
        catch (FileNotFoundException)
        {
            logger.Error($"Quiz in category '{category}' not found.");
            Console.WriteLine("Quiz not found!");
        }
        catch (Exception ex)
        {
            logger.Error($"Error editing quiz in category '{category}': {ex.Message}");
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.ReadKey();
    }

    public static void DeleteQuiz()
    {
        Console.Write("Enter quiz category to delete: ");
        string category = Console.ReadLine();
        string filePath = $"quizs_files/{category}.txt";

        try
        {
            logger.Info($"Attempt to delete quiz in category '{category}'.");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Console.WriteLine("Quiz deleted!");
                logger.Info($"Quiz in category '{category}' deleted successfully.");
            }
            else
            {
                Console.WriteLine("Quiz not found!");
                logger.Warn($"Quiz in category '{category}' not found.");
            }
        }
        catch (Exception ex)
        {
            logger.Error($"Error deleting quiz in category '{category}': {ex.Message}");
            Console.WriteLine($"Error deleting quiz: {ex.Message}");
        }
    }

    public static void ListFilesInDirectory(string path, string login, bool viewQuestionsOnly = false)
    {
        if (Directory.Exists(path))
        {
            string[] files = Directory.GetFiles(path);
            Console.WriteLine("Quizs:");
            for (int i = 0; i < files.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {Path.GetFileName(files[i])}");
            }

            Console.Write("Enter a number to select: ");
            int choice;
            if (int.TryParse(Console.ReadLine(), out choice) && choice > 0 && choice <= files.Length)
            {
                Console.Clear();
                string selectedFile = files[choice - 1];
                Console.WriteLine($"You selected: {Path.GetFileName(selectedFile)}");

                if (viewQuestionsOnly)
                {
                    ViewQuizQuestions(selectedFile);
                }
                else
                {
                    StartQuiz(selectedFile, Path.GetFileName(selectedFile), login);
                }

            }
            else
            {
                Console.WriteLine("Invalid choice");
            }
        }
        else
        {
            Console.WriteLine("Directory does not exist");
        }
    }

    public static void ViewQuizQuestions(string filePath)
    {
        try
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                string content = streamReader.ReadToEnd();
                string[] questions = content.Split(';', StringSplitOptions.RemoveEmptyEntries);

                Console.WriteLine("Quiz Questions:");
                int count = 0;
                foreach (var question in questions)
                {
                    count++;
                    string[] parts = question.Split('&');
                    if (parts.Length == 2)
                    {
                        string questionAndAnswers = parts[1];
                        string[] questionParts = questionAndAnswers.Split('|');
                        if (questionParts.Length == 2)
                        {
                            string questionText = questionParts[0];
                            Console.WriteLine($"{count}. {questionText}");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {

            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.ReadLine();
    }
}
