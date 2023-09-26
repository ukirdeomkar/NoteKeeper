using NoteKeeper.Models;
using System.ComponentModel.DataAnnotations;

namespace NoteKeeper.Dtos
{
    public class NoteDto
    {
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public int Sharing{ get; set; }

        public DateTime DateAdded { get; set; }

        public int UserId { get; set; }
        public int? Permission { get; set; }





    }
}
