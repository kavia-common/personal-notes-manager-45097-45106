using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NotesBackend.Models;

namespace NotesBackend.Services
{
    /// <summary>
    /// Abstraction for note storage operations.
    /// </summary>
    public interface INoteRepository
    {
        // PUBLIC_INTERFACE
        Task<Note> CreateAsync(Note note);
        // PUBLIC_INTERFACE
        Task<IReadOnlyList<Note>> GetAllAsync();
        // PUBLIC_INTERFACE
        Task<Note?> GetByIdAsync(Guid id);
        // PUBLIC_INTERFACE
        Task<bool> UpdateAsync(Note note);
        // PUBLIC_INTERFACE
        Task<bool> DeleteAsync(Guid id);
    }
}
