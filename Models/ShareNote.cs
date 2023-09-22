namespace NoteKeeper.Models
{
    public class ShareNote
    {
        public int Id { get; set; }
        public int NoteId { get; set; }
        public string UniqueLink { get; set; }
        public string Permission { get; set; } // "view", "edit", "delete"

        public Note? Note { get; set; }

        public static readonly int notShared = 0;
        public static readonly int viewNote = 1;
        public static readonly int editNote = 2;
        public static readonly int deleteNote = 3;

    }
}
