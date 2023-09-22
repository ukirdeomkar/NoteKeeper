namespace NoteKeeper.Models
{
    public class ShareNote
    {
        public Guid? UniqueLink { get; set; }
        public int SharedPermission { get; set; } // "view", "edit", "delete"


        public static readonly int notShared = 0;
        public static readonly int viewNote = 1;
        public static readonly int editNote = 2;
        public static readonly int deleteNote = 3;

    }
}
