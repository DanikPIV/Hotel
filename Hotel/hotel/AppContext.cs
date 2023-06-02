using Hotel;
using System.Data.Entity;

namespace Hotel
{
    internal class AppContext : DbContext
    {

        public DbSet<User> Users { get; set; }

        public AppContext() : base("DefaultConnection") { }

    }
}
