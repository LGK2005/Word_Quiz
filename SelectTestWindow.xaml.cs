using System;
using System.Collections.Generic;
using System.Windows;
using WordQuizApp.Models;

namespace WordQuizApp
{
    public partial class SelectTestWindow : Window
    {
        public Test SelectedTest { get; private set; }
        
        public SelectTestWindow()
        {
            InitializeComponent();
            LoadTests();
        }

        private void LoadTests()
        {
            try
            {
                List<Test> tests = FileStorageHelper.GetAllTests();
                lvTests.ItemsSource = tests;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tests: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            Test selectedTest = lvTests.SelectedItem as Test;
            
            if (selectedTest == null)
            {
                MessageBox.Show("Please select a test to take.", "No Selection", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            try
            {
                // Load the complete test with word pairs
                SelectedTest = FileStorageHelper.GetTestById(selectedTest.Id);
                
                if (SelectedTest.WordPairs.Count == 0)
                {
                    MessageBox.Show("This test has no word pairs. Please select another test or add word pairs to this test.", 
                        "Empty Test", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading test details: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

