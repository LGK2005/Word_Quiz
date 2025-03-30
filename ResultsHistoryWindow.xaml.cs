using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WordQuizApp.Models;

namespace WordQuizApp
{
    public partial class ResultsHistoryWindow : Window
    {
        public ResultsHistoryWindow()
        {
            InitializeComponent();
            LoadResults();
        }

        private void LoadResults()
        {
            try
            {
                List<QuizResult> results = FileStorageHelper.GetQuizResults();
                
                // Create display objects
                var displayResults = results.Select(r => new
                {
                    r.TestId,
                    r.TestName,
                    r.CompletedDate,
                    r.TotalQuestions,
                    r.CorrectAnswers,
                    r.TimeTaken,
                    ScoreDisplay = $"{r.CorrectAnswers}/{r.TotalQuestions} ({r.ScorePercentage:F1}%)",
                    TimeTakenDisplay = FormatTimeTaken(r.TimeTaken)
                }).ToList();
                
                dgResults.ItemsSource = displayResults;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading results: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string FormatTimeTaken(TimeSpan timeTaken)
        {
            string result = "";
            
            if (timeTaken.Hours > 0)
            {
                result += $"{timeTaken.Hours}h ";
            }
            
            if (timeTaken.Minutes > 0 || timeTaken.Hours > 0)
            {
                result += $"{timeTaken.Minutes}m ";
            }
            
            result += $"{timeTaken.Seconds}s";
            
            return result;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

