using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public Guid UniqueId { get; set; }


        [ForeignKey("UserId")]
        public int UserId{ get; set; }
        public User? User { get; set; }
    }
}
