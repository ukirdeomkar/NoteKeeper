using NoteKeeper.Models;

namespace NoteKeeper.Dtos
{
    public class ShareNoteDto
    {

        public string? UniqueLink { get; set; }
        public int Permission { get; set; } // "view", "edit", "delete"


    }
}
