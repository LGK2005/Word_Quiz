using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WordQuizApp.Models;
using System.Windows;

namespace WordQuizApp
{
    public static class FileStorageHelper
    {
        private static readonly string BasePath = @"D:\C# Save\WordQuizApp";
        private static readonly string TestsFolderPath = Path.Combine(BasePath, "Tests");
        private static readonly string ResultsFolderPath = Path.Combine(BasePath, "Results");

        public static void InitializeStorage()
        {
            // Create base directory if it doesn't exist
            if (!Directory.Exists(BasePath))
            {
                Directory.CreateDirectory(BasePath);
            }

            // Create Tests directory if it doesn't exist
            if (!Directory.Exists(TestsFolderPath))
            {
                Directory.CreateDirectory(TestsFolderPath);
            }

            // Create Results directory if it doesn't exist
            if (!Directory.Exists(ResultsFolderPath))
            {
                Directory.CreateDirectory(ResultsFolderPath);
            }
        }

        public static int SaveTest(Test test)
        {
            // Generate a unique ID for the test if it doesn't have one
            if (test.Id <= 0)
            {
                test.Id = GenerateNewTestId();
            }

            // Create the file path
            string fileName = $"{test.Id}_{SanitizeFileName(test.Name)}.txt";
            string filePath = Path.Combine(TestsFolderPath, fileName);

            // Create the file content
            StringBuilder content = new StringBuilder();

            // First line: Test metadata (ID, Name, CreatedDate)
            content.AppendLine($"TEST:{test.Id}_{test.Name}_{test.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss")}");

            // Remaining lines: Word pairs
            foreach (var wordPair in test.WordPairs)
            {
                content.AppendLine($"{wordPair.Word} {wordPair.Meaning}");
            }

            // Write to file
            File.WriteAllText(filePath, content.ToString(), Encoding.UTF8);

            return test.Id;
        }

        public static List<Test> GetAllTests()
        {
            List<Test> tests = new List<Test>();

            if (!Directory.Exists(TestsFolderPath))
            {
                MessageBox.Show($"Tests folder not found at: {TestsFolderPath}", "Folder Not Found",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return tests;
            }

            // Get all .txt files in the Tests directory
            string[] testFiles = Directory.GetFiles(TestsFolderPath, "*.txt");

            if (testFiles.Length == 0)
            {
                MessageBox.Show($"No test files found in: {TestsFolderPath}", "No Tests Found",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }

            foreach (string filePath in testFiles)
            {
                try
                {
                    // Read the first line to get test metadata
                    string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);

                    if (lines.Length > 0 && lines[0].StartsWith("TEST:"))
                    {
                        string[] testParts = lines[0].Substring(5).Split('_');

                        if (testParts.Length >= 2)
                        {
                            Test test = new Test
                            {
                                Id = int.Parse(testParts[0]),
                                Name = testParts[1],
                                CreatedDate = DateTime.Parse(testParts[2])
                            };

                            tests.Add(test);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log error or handle as needed
                    MessageBox.Show($"Error reading test file {filePath}: {ex.Message}", "File Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            // Sort by created date (newest first)
            return tests.OrderByDescending(t => t.CreatedDate).ToList();
        }

        public static Test GetTestById(int testId)
        {
            if (!Directory.Exists(TestsFolderPath))
            {
                return null;
            }

            // Find the file that starts with the test ID
            string[] testFiles = Directory.GetFiles(TestsFolderPath, $"{testId}_*.txt");

            if (testFiles.Length == 0)
            {
                return null;
            }

            string filePath = testFiles[0];

            try
            {
                // Read all lines from the file
                string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);

                if (lines.Length > 0 && lines[0].StartsWith("TEST:"))
                {
                    string[] testParts = lines[0].Substring(5).Split('_');

                    if (testParts.Length >= 2)
                    {
                        Test test = new Test
                        {
                            Id = int.Parse(testParts[0]),
                            Name = testParts[1],
                            CreatedDate = DateTime.Parse(testParts[2])
                        };

                        // Read word pairs (starting from line 1)
                        for (int i = 1; i < lines.Length; i++)
                        {
                            string line = lines[i].Trim();

                            if (!string.IsNullOrEmpty(line))
                            {
                                int firstSpaceIndex = line.IndexOf(' ');

                                if (firstSpaceIndex > 0 && firstSpaceIndex < line.Length - 1)
                                {
                                    string word = line.Substring(0, firstSpaceIndex).Trim();
                                    string meaning = line.Substring(firstSpaceIndex + 1).Trim();

                                    WordPair wordPair = new WordPair
                                    {
                                        Id = i,
                                        TestId = test.Id,
                                        Word = word,
                                        Meaning = meaning
                                    };

                                    test.WordPairs.Add(wordPair);
                                }
                            }
                        }

                        return test;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error or handle as needed
                Console.WriteLine($"Error reading test file {filePath}: {ex.Message}");
            }

            return null;
        }

        public static void SaveQuizResult(QuizResult result)
        {
            // Generate a unique ID for the result
            result.Id = GenerateNewResultId();

            // Create the file path
            string fileName = $"{result.Id}_{result.TestId}_{SanitizeFileName(result.TestName)}_{result.CompletedDate.ToString("yyyyMMdd_HHmmss")}.txt";
            string filePath = Path.Combine(ResultsFolderPath, fileName);

            // Create the file content
            StringBuilder content = new StringBuilder();

            // Result metadata
            content.AppendLine($"RESULT:{result.Id}_{result.TestId}_{result.TestName}_{result.CompletedDate.ToString("yyyy-MM-dd HH:mm:ss")}_{result.TotalQuestions}_{result.CorrectAnswers}_{(int)result.TimeTaken.TotalSeconds}");

            // Write to file
            File.WriteAllText(filePath, content.ToString(), Encoding.UTF8);
        }

        public static List<QuizResult> GetQuizResults()
        {
            List<QuizResult> results = new List<QuizResult>();

            if (!Directory.Exists(ResultsFolderPath))
            {
                return results;
            }

            // Get all .txt files in the Results directory
            string[] resultFiles = Directory.GetFiles(ResultsFolderPath, "*.txt");

            foreach (string filePath in resultFiles)
            {
                try
                {
                    // Read the first line to get result metadata
                    string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);

                    if (lines.Length > 0 && lines[0].StartsWith("RESULT:"))
                    {
                        string[] resultParts = lines[0].Substring(7).Split('_');

                        if (resultParts.Length >= 2)
                        {
                            QuizResult result = new QuizResult
                            {
                                Id = int.Parse(resultParts[0]),
                                TestId = int.Parse(resultParts[1]),
                                TestName = resultParts[2],
                                CompletedDate = DateTime.Parse(resultParts[3]),
                                TotalQuestions = int.Parse(resultParts[4]),
                                CorrectAnswers = int.Parse(resultParts[5]),
                                TimeTaken = TimeSpan.FromSeconds(int.Parse(resultParts[6]))
                            };

                            results.Add(result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log error or handle as needed
                    Console.WriteLine($"Error reading result file {filePath}: {ex.Message}");
                }
            }

            // Sort by completed date (newest first)
            return results.OrderByDescending(r => r.CompletedDate).ToList();
        }

        private static int GenerateNewTestId()
        {
            // Get the highest existing test ID and increment by 1
            List<Test> tests = GetAllTests();

            if (tests.Count == 0)
            {
                return 1;
            }

            return tests.Max(t => t.Id) + 1;
        }

        private static int GenerateNewResultId()
        {
            // Get the highest existing result ID and increment by 1
            List<QuizResult> results = GetQuizResults();

            if (results.Count == 0)
            {
                return 1;
            }

            return results.Max(r => r.Id) + 1;
        }

        private static string SanitizeFileName(string fileName)
        {
            // Remove invalid file name characters
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '?');
            }

            return fileName;
        }
    }
}

