using SQLite;
using MyJournalBlazor.Models;

namespace MyJournalBlazor.Services
{
    public class DatabaseService: IDatabaseService
    {
        private SQLiteAsyncConnection _db;

        async Task Init()
        {
            if (_db != null) return;

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "MyJournal.db");
            _db = new SQLiteAsyncConnection(dbPath);
            await _db.CreateTableAsync<JournalEntry>();
        }

        public async Task SaveEntryAsync(JournalEntry entry)
        {
            await Init();

            // Checking if we already have an ID (Update) or if it's new (Insert)
            if (entry.Id != 0)
            {
                await _db.UpdateAsync(entry);
            }
            else
            {
                // Checking if an entry for this DATE already exists to prevent duplicates
                var existing = await GetEntryByDateAsync(entry.Date);
                if (existing != null)
                {
                    entry.Id = existing.Id; // Take the old ID
                    await _db.UpdateAsync(entry); // Update instead of Insert
                }
                else
                {
                    await _db.InsertAsync(entry);
                }
            }
        }

        public async Task<List<JournalEntry>> GetEntriesAsync()
        {
            await Init();
            return await _db.Table<JournalEntry>().OrderByDescending(x => x.Date).ToListAsync();
        }

        public async Task DeleteEntryAsync(JournalEntry entry)
        {
            await Init();
            await _db.DeleteAsync(entry);
        }

  
        public async Task<JournalEntry> GetEntryByDateAsync(DateTime date)
        {
            await Init();
            // Calculate the start and end of the selected day
            var startOfDay = date.Date;
            var endOfDay = date.Date.AddDays(1).AddTicks(-1);

            // Finding those entries that falls between 00:00:00 and 23:59:59 of that day
            return await _db.Table<JournalEntry>()
                            .Where(e => e.Date >= startOfDay && e.Date <= endOfDay)
                            .FirstOrDefaultAsync();
        }
    }
}