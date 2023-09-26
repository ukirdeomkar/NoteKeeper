namespace NoteKeeper.Models
{
    public class ShareNoteOtherUsers
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }

        public Guid NoteId { get; set; }
        public  Note? Note { get; set; }
    }
}
