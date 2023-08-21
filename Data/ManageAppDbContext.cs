using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WebChatApp.Data.Entities;

namespace WebChatApp.Data
{
    public class ManageAppDbContext : IdentityDbContext<ManageUser>
    {
        public ManageAppDbContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>().Property(x => x.Id).HasMaxLength(50).IsRequired(true);
            builder.Entity<ManageUser>().Property(x => x.Id).HasMaxLength(50).IsRequired(true);

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ManageUser> ManageUsers { get; set; }
    }
}
