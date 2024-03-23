
using Microsoft.EntityFrameworkCore;

namespace NoteKeeper.Models
{

    public class MyDBContext : DbContext 
    {
        public MyDBContext(DbContextOptions options):base(options)
        {

        }
        public virtual DbSet<User> Users { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<ShareNoteOtherUsers> ShareNoteOtherUsers { get; set; }
    }
}
