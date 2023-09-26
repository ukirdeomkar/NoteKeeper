using NoteKeeper.Models;

namespace NoteKeeper.Dtos
{
    public class ShareNoteDto
    {

        public Guid? UniqueLink { get; set; }
        public int Sharing { get; set; }
        public int Permission { get; set; } // "view", "edit", "delete"


    }
}
