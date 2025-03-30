using System;

namespace WordQuizApp.Models
{
    public class QuizResult
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public string TestName { get; set; }
        public DateTime CompletedDate { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public TimeSpan TimeTaken { get; set; }
        public double ScorePercentage => TotalQuestions > 0 ? (double)CorrectAnswers / TotalQuestions * 100 : 0;

        public QuizResult()
        {
            CompletedDate = DateTime.Now;
        }
    }
}

