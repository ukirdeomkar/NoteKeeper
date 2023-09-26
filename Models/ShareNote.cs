using System.ComponentModel.DataAnnotations;

namespace NoteKeeper.Models
{
    public class ShareNote
    {
        public Guid? UniqueLink { get; set; }
        public int Sharing { get; set; }
        public int SharedPermission { get; set; } // "view", "edit", "delete"

        [EmailAddress]
        public string? Email { get; set; }


        public static readonly int notShared = 0;
        public static readonly int viewNote = 1;
        public static readonly int editNote = 2;
        public static readonly int deleteNote = 3;

        public static readonly int sharedWithOtherUsers = 1;
        public static readonly int sharedAnonymously = 2;

    }
}
