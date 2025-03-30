using System;
using System.Collections.Generic;

namespace WordQuizApp.Models
{
    public class Test
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<WordPair> WordPairs { get; set; }

        public Test()
        {
            WordPairs = new List<WordPair>();
            CreatedDate = DateTime.Now;
        }
    }
}

