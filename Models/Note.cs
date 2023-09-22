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

        public int Permission { get; set; } = 0;// "view", "edit", "delete"


        public static readonly int notShared = 0;
        public static readonly int viewNote = 1;
        public static readonly int editNote = 2;
        public static readonly int deleteNote = 3;
    }
}
