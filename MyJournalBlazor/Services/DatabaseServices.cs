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

            // Normalize date (remove time) so "one entry per day" is strict
            entry.Date = entry.Date.Date;

            var now = DateTime.Now;

            // If the entry already has an ID, it's definitely an update
            if (entry.Id != 0)
            {
                entry.UpdatedAt = now;

                // CreatedAt should not change on updates (but ensuring that it exists)
                if (entry.CreatedAt == default)
                    entry.CreatedAt = now;

                await _db.UpdateAsync(entry);
                return;
            }

            // If ID == 0, check if an entry for this day already exists
            var existing = await GetEntryByDateAsync(entry.Date);

            if (existing != null)
            {
                // Update existing row instead of inserting a duplicate
                entry.Id = existing.Id;
                entry.CreatedAt = existing.CreatedAt;  // preserve original create time
                entry.UpdatedAt = now;

                await _db.UpdateAsync(entry);
            }
            else
            {
                // Brand new entry
                entry.CreatedAt = now;
                entry.UpdatedAt = now;

                await _db.InsertAsync(entry);
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