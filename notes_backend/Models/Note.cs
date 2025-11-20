using System;
using System.ComponentModel.DataAnnotations;

namespace NotesBackend.Models
{
    /// <summary>
    /// Domain model representing a personal note.
    /// </summary>
    public class Note
    {
        /// <summary>
        /// Unique identifier of the note.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Title of the note.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Content of the note.
        /// </summary>
        [Required]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Creation timestamp (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Last update timestamp (UTC).
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
