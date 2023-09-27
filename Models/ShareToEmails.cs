namespace NoteKeeper.Models
{
    public class ShareToEmails
    {
        public Guid NoteId { get; set; }
        public List<string> EmailList { get; set; }
        public ShareNote ShareNote { get; set; }
    }
}
