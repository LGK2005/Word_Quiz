using System;

namespace WordQuizApp.Models
{
    public class WordPair
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public string Word { get; set; }
        public string Meaning { get; set; }

        public WordPair()
        {
        }

        public WordPair(string word, string meaning)
        {
            Word = word;
            Meaning = meaning;
        }
    }
}

