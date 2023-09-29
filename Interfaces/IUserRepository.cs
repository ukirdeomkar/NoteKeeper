using NoteKeeper.Models;

namespace NoteKeeper.Interfaces
{
    public interface IUserRepository
    {
        User GetUserById(int userId);
    }
}
