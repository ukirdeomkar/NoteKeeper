
using Microsoft.EntityFrameworkCore;

namespace NoteKeeper.Models
{
    public class MyDBContext : DbContext
    {
        public MyDBContext(DbContextOptions options):base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Note> Notes { get; set; }
    }
}
