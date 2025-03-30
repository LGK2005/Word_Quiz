using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using WordQuizApp.Models;

namespace WordQuizApp
{
    public partial class ResultsWindow : Window
    {
        private QuizResult quizResult;
        private List<QuizAnswer> quizAnswers;
        
        public ResultsWindow(QuizResult result, List<QuizAnswer> answers)
        {
            InitializeComponent();
            
            quizResult = result;
            quizAnswers = answers;
            
            // Set up the UI
            txtTestName.Text = result.TestName;
            txtCompletionDate.Text = result.CompletedDate.ToString("MM/dd/yyyy hh:mm tt");
            txtScore.Text = $"{result.CorrectAnswers}/{result.TotalQuestions} ({result.ScorePercentage:F1}%)";
            
            // Format time taken
            string timeTaken = "";
            if (result.TimeTaken.Hours > 0)
            {
                timeTaken += $"{result.TimeTaken.Hours} hour{(result.TimeTaken.Hours > 1 ? "s" : "")} ";
            }
            if (result.TimeTaken.Minutes > 0)
            {
                timeTaken += $"{result.TimeTaken.Minutes} minute{(result.TimeTaken.Minutes > 1 ? "s" : "")} ";
            }
            timeTaken += $"{result.TimeTaken.Seconds} second{(result.TimeTaken.Seconds > 1 ? "s" : "")}";
            txtTimeTaken.Text = timeTaken;
            
            // Set feedback based on score
            double scorePercentage = result.ScorePercentage;
            if (scorePercentage >= 90)
            {
                txtFeedback.Text = "Excellent! You've mastered these words!";
            }
            else if (scorePercentage >= 75)
            {
                txtFeedback.Text = "Great job! You're doing well with these words.";
            }
            else if (scorePercentage >= 60)
            {
                txtFeedback.Text = "Good effort! Keep practicing to improve.";
            }
            else
            {
                txtFeedback.Text = "Keep practicing! You'll get better with time.";
            }
            
            // Set up the results grid
            dgResults.ItemsSource = quizAnswers;
        }

        private void btnTryAgain_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Load the test again
                Test test = FileStorageHelper.GetTestById(quizResult.TestId);
                
                if (test != null)
                {
                    // Open a new quiz window
                    QuizWindow quizWindow = new QuizWindow(test);
                    quizWindow.Owner = this.Owner;
                    
                    // Close this window
                    Close();
                    
                    // Show the quiz window
                    quizWindow.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Could not load the test. It may have been deleted.", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading test: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class BoolToResultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isCorrect)
            {
                return isCorrect ? "Correct" : "Incorrect";
            }
            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isCorrect)
            {
                return isCorrect ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
            }
            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

