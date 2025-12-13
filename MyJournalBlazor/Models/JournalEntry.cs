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
        public DateTime Date { get; set; } = DateTime.Now; // Default to today
        public string Mood { get; set; } = "Neutral";
        public DateTime CreatedAt { get; set; }
    }
}