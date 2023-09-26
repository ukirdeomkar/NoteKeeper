using NoteKeeper.Models;

namespace NoteKeeper.Dtos
{
    public class ShareNoteOtherUserDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }

        public Guid NoteId { get; set; }
        public Note? Note { get; set; }
    }
}
