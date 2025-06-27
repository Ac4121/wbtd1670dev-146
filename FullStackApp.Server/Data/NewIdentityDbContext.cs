
using FullStackApp.Server.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FullStackApp.Server.Data
{

    public class NewIdentityDbContext : IdentityDbContext<User>
    {
        public NewIdentityDbContext(DbContextOptions<NewIdentityDbContext> options) :
            base(options)
        { }

        public DbSet<FullStackApp.Server.Models.User.User> User { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<FullStackApp.Server.Models.Bookings> Bookings { get; set; } = default!;
        
        public DbSet<FullStackApp.Server.Models.Movies> Movies { get; set; } = default!;

        public DbSet<FullStackApp.Server.Models.SessionTimes> SessionTimes { get; set; } = default!;
    }
}
