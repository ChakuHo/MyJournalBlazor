using SQLite;
using System;

namespace MyJournalBlazor.Models
{
    public class JournalEntry
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;

        // MOODS
        public string Mood { get; set; } = "Neutral"; // Primary
        public string SecondaryMoods { get; set; } // Comma-separated (e.g. "Tired,Excited")

        // TAGS
        public string Tags { get; set; } // Comma-separated (e.g. "Work,Fitness")

        public DateTime CreatedAt { get; set; }
    }
}