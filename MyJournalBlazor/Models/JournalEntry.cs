using SQLite;

namespace MyJournalBlazor.Models
{
    public class JournalEntry
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public DateTime Date { get; set; } = DateTime.Today;

        // Primary mood
        public string Mood { get; set; } = "Neutral";

        // Comma-separated values
        public string SecondaryMoods { get; set; } = "";
        public string Tags { get; set; } = "";

        // System timestamps
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}