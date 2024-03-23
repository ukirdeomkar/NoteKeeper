using NoteKeeper.Interfaces;
using NoteKeeper.Models;

namespace NoteKeeper.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly MyDBContext _context;

        public UserRepository(MyDBContext context)
        {
            _context = context;
        }

        public User GetUserById(int id)
        {
            return _context.Users.SingleOrDefault(u => u.Id == id);
        }
        public void CreateUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }
        public User GetUserByEmail(string email)
        {
            return _context.Users.SingleOrDefault(u => u.Email == email);
        }
    }
}
