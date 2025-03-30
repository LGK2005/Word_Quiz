using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using WordQuizApp.Models;

namespace WordQuizApp
{
    public partial class QuizWindow : Window
    {
        private Test test;
        private List<WordPair> randomizedWordPairs;
        private List<string> userAnswers;
        private int currentQuestionIndex = 0;
        private DateTime startTime;
        private DispatcherTimer timer;
        
        public QuizWindow(Test test)
        {
            InitializeComponent();
            
            this.test = test;
            
            // Randomize the word pairs
            randomizedWordPairs = test.WordPairs.OrderBy(x => Guid.NewGuid()).ToList();
            userAnswers = new List<string>(new string[randomizedWordPairs.Count]);
            
            // Set up the UI
            txtTestName.Text = test.Name;
            txtTotalQuestions.Text = randomizedWordPairs.Count.ToString();
            
            // Start the timer
            startTime = DateTime.Now;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
            
            // Display the first question
            DisplayCurrentQuestion();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsed = DateTime.Now - startTime;
            txtTimer.Text = $"Time: {elapsed.Minutes:00}:{elapsed.Seconds:00}";
        }

        private void DisplayCurrentQuestion()
        {
            txtCurrentQuestion.Text = (currentQuestionIndex + 1).ToString();
            txtWord.Text = randomizedWordPairs[currentQuestionIndex].Word;
            
            // Set the answer if it exists
            txtAnswer.Text = userAnswers[currentQuestionIndex] ?? "";
            
            // Update button states
            btnPrevious.IsEnabled = currentQuestionIndex > 0;
            btnNext.IsEnabled = currentQuestionIndex < randomizedWordPairs.Count - 1;
        }

        private void SaveCurrentAnswer()
        {
            userAnswers[currentQuestionIndex] = txtAnswer.Text.Trim();
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrentAnswer();
            currentQuestionIndex--;
            DisplayCurrentQuestion();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrentAnswer();
            currentQuestionIndex++;
            DisplayCurrentQuestion();
        }

        private void btnQuit_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to quit? Your progress will be lost.", 
                "Confirm Quit", MessageBoxButton.YesNo, MessageBoxImage.Question);
                
            if (result == MessageBoxResult.Yes)
            {
                timer.Stop();
                Close();
            }
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            // Save the current answer
            SaveCurrentAnswer();
            
            // Check if all questions have been answered
            if (userAnswers.Any(a => string.IsNullOrEmpty(a)))
            {
                MessageBoxResult Result = MessageBox.Show("You haven't answered all questions. Do you want to finish anyway?", 
                    "Confirm Finish", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    
                if (Result == MessageBoxResult.No)
                {
                    return;
                }
            }
            
            // Stop the timer
            timer.Stop();
            TimeSpan timeTaken = DateTime.Now - startTime;
            
            // Calculate results
            int correctAnswers = 0;
            List<QuizAnswer> quizAnswers = new List<QuizAnswer>();
            
            for (int i = 0; i < randomizedWordPairs.Count; i++)
            {
                string userAnswer = userAnswers[i] ?? "";
                bool isCorrect = userAnswer.Trim().Equals(randomizedWordPairs[i].Meaning.Trim(), 
                    StringComparison.OrdinalIgnoreCase);
                
                if (isCorrect)
                {
                    correctAnswers++;
                }
                
                quizAnswers.Add(new QuizAnswer
                {
                    Word = randomizedWordPairs[i].Word,
                    CorrectMeaning = randomizedWordPairs[i].Meaning,
                    UserAnswer = userAnswer,
                    IsCorrect = isCorrect
                });
            }
            
            // Save the result to the file system
            QuizResult result = new QuizResult
            {
                TestId = test.Id,
                TestName = test.Name,
                CompletedDate = DateTime.Now,
                TotalQuestions = randomizedWordPairs.Count,
                CorrectAnswers = correctAnswers,
                TimeTaken = timeTaken
            };
            
            try
            {
                FileStorageHelper.SaveQuizResult(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving quiz result: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            // Show the results window
            ResultsWindow resultsWindow = new ResultsWindow(result, quizAnswers);
            resultsWindow.Owner = this;
            resultsWindow.ShowDialog();
            
            // Close this window
            Close();
        }
    }

    public class QuizAnswer
    {
        public string Word { get; set; }
        public string CorrectMeaning { get; set; }
        public string UserAnswer { get; set; }
        public bool IsCorrect { get; set; }
    }
}

