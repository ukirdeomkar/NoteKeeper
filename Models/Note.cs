using System.ComponentModel.DataAnnotations;

namespace NoteKeeper.Models
{
    public class Note
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public User User { get; set; }
        public int UserId{ get; set; }
    }
}
