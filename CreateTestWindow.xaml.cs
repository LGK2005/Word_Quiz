using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using WordQuizApp.Models;

namespace WordQuizApp
{
    public partial class CreateTestWindow : Window
    {
        private ObservableCollection<WordPair> wordPairs;
        
        public CreateTestWindow()
        {
            InitializeComponent();
            
            wordPairs = new ObservableCollection<WordPair>();
            dgWordPairs.ItemsSource = wordPairs;
        }

        private void btnAddPair_Click(object sender, RoutedEventArgs e)
        {
            string word = txtWord.Text.Trim();
            string meaning = txtMeaning.Text.Trim();
            
            if (string.IsNullOrEmpty(word) || string.IsNullOrEmpty(meaning))
            {
                MessageBox.Show("Please enter both a word and its meaning.", "Missing Information", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            wordPairs.Add(new WordPair(word, meaning));
            
            // Clear input fields
            txtWord.Clear();
            txtMeaning.Clear();
            txtWord.Focus();
        }

        private void btnRemovePair_Click(object sender, RoutedEventArgs e)
        {
            WordPair selectedPair = (sender as Button).DataContext as WordPair;
            
            if (selectedPair != null)
            {
                wordPairs.Remove(selectedPair);
            }
        }

        private void btnImportAll_Click(object sender, RoutedEventArgs e)
        {
            string bulkText = txtBulkImport.Text.Trim();
            
            if (string.IsNullOrEmpty(bulkText))
            {
                MessageBox.Show("Please enter word-meaning pairs to import.", "Missing Information", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            // Split the text into lines
            string[] lines = bulkText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int successCount = 0;
            int failCount = 0;
            
            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();
                if (string.IsNullOrEmpty(trimmedLine))
                    continue;
                
                // Find the first space that separates word and meaning
                int firstSpaceIndex = trimmedLine.IndexOf(' ');
                
                if (firstSpaceIndex > 0 && firstSpaceIndex < trimmedLine.Length - 1)
                {
                    string word = trimmedLine.Substring(0, firstSpaceIndex).Trim();
                    string meaning = trimmedLine.Substring(firstSpaceIndex + 1).Trim();
                    
                    if (!string.IsNullOrEmpty(word) && !string.IsNullOrEmpty(meaning))
                    {
                        wordPairs.Add(new WordPair(word, meaning));
                        successCount++;
                    }
                    else
                    {
                        failCount++;
                    }
                }
                else
                {
                    failCount++;
                }
            }
            
            // Clear the bulk import text box
            txtBulkImport.Clear();
            
            // Show results
            if (successCount > 0)
            {
                MessageBox.Show($"Successfully imported {successCount} word-meaning pairs." + 
                    (failCount > 0 ? $"\n{failCount} lines could not be processed." : ""), 
                    "Import Complete", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("No valid word-meaning pairs were found. Please check the format and try again.", 
                    "Import Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string testName = txtTestName.Text.Trim();
            
            if (string.IsNullOrEmpty(testName))
            {
                MessageBox.Show("Please enter a name for the test.", "Missing Information", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            if (wordPairs.Count < 1)
            {
                MessageBox.Show("Please add at least one word pair to the test.", "Missing Information", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            try
            {
                // Create and save the test
                Test test = new Test
                {
                    Name = testName,
                    CreatedDate = DateTime.Now
                };
                
                foreach (var pair in wordPairs)
                {
                    test.WordPairs.Add(pair);
                }
                
                int testId = FileStorageHelper.SaveTest(test);
                
                MessageBox.Show($"Test '{testName}' has been saved successfully!", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving test: {ex.Message}", "Error", 
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

