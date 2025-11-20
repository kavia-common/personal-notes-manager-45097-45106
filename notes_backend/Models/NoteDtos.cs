using System;
using System.ComponentModel.DataAnnotations;
using NotesBackend.Models;

namespace NotesBackend.Dtos
{
    /// <summary>
    /// Request body for creating a new note.
    /// </summary>
    public class CreateNoteRequest
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request body for updating an existing note.
    /// </summary>
    public class UpdateNoteRequest
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;
    }

    /// <summary>
    /// Response body for note data.
    /// </summary>
    public class NoteResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // PUBLIC_INTERFACE
        public static NoteResponse From(Note note)
        {
            /** Maps Note domain model to API response. */
            return new NoteResponse
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt
            };
        }
    }
}
