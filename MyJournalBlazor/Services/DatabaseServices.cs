using SQLite;
using MyJournalBlazor.Models;

namespace MyJournalBlazor.Services
{
    public class DatabaseService
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
            if (entry.Id == 0)
                await _db.InsertAsync(entry);
            else
                await _db.UpdateAsync(entry);
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
    }
}