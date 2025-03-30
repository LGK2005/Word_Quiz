using System;
using System.Windows;

namespace WordQuizApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnCreateTest_Click(object sender, RoutedEventArgs e)
        {
            CreateTestWindow createTestWindow = new CreateTestWindow();
            createTestWindow.Owner = this;
            createTestWindow.ShowDialog();
        }

        private void btnTakeTest_Click(object sender, RoutedEventArgs e)
        {
            SelectTestWindow selectTestWindow = new SelectTestWindow();
            selectTestWindow.Owner = this;
            
            if (selectTestWindow.ShowDialog() == true && selectTestWindow.SelectedTest != null)
            {
                QuizWindow quizWindow = new QuizWindow(selectTestWindow.SelectedTest);
                quizWindow.Owner = this;
                quizWindow.ShowDialog();
            }
        }

        private void btnViewResults_Click(object sender, RoutedEventArgs e)
        {
            ResultsHistoryWindow resultsWindow = new ResultsHistoryWindow();
            resultsWindow.Owner = this;
            resultsWindow.ShowDialog();
        }
    }
}

