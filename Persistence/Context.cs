using Microsoft.EntityFrameworkCore;
using UserManagement.Entities;

namespace UserManagement.Persistence
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Password> Passwords => Set<Password>();
    }
}
