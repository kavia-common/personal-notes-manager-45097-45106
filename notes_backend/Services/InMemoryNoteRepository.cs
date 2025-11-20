using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NotesBackend.Models;

namespace NotesBackend.Services
{
    /// <summary>
    /// Thread-safe in-memory note storage. Suitable for demos and ephemeral sessions.
    /// </summary>
    public class InMemoryNoteRepository : INoteRepository
    {
        private readonly ConcurrentDictionary<Guid, Note> _store = new();

        public Task<Note> CreateAsync(Note note)
        {
            _store[note.Id] = note;
            return Task.FromResult(note);
        }

        public Task<IReadOnlyList<Note>> GetAllAsync()
        {
            var list = _store.Values
                .OrderByDescending(n => n.UpdatedAt)
                .ToList()
                .AsReadOnly();
            return Task.FromResult((IReadOnlyList<Note>)list);
        }

        public Task<Note?> GetByIdAsync(Guid id)
        {
            _store.TryGetValue(id, out var note);
            return Task.FromResult(note);
        }

        public Task<bool> UpdateAsync(Note note)
        {
            if (!_store.ContainsKey(note.Id))
                return Task.FromResult(false);
            _store[note.Id] = note;
            return Task.FromResult(true);
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            return Task.FromResult(_store.TryRemove(id, out _));
        }
    }
}
