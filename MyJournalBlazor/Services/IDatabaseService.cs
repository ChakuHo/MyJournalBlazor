using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyJournalBlazor.Models;

namespace MyJournalBlazor.Services
{
    public interface IDatabaseService
    {
        Task SaveEntryAsync(JournalEntry entry);
        Task<List<JournalEntry>> GetEntriesAsync();
        Task<JournalEntry> GetEntryByDateAsync(DateTime date);
        Task DeleteEntryAsync(JournalEntry entry);
    }
}
