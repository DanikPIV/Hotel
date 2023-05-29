using Hotel.hotel;
using System.Data.Entity;

namespace Hotel
{
    internal class AppContext : DbContext
    {

        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Status_client> Status_clients { get; set; }
        public DbSet<Type_of_food> Type_of_foods { get; set; }

        public AppContext() : base("DefaultConnection") { }

    }
}
