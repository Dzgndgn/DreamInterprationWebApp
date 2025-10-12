
using DreamAI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DreamAI.Context
{
    public class DreamDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public DreamDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> users { get; set; }
        public DbSet<DreamInterprete> dreamInterpretes { get; set; }
        public DbSet<DreamSymbols> dreamSymbols { get; set; }
        public DbSet<Dreams> dreams { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<DreamSymbols>().HasIndex(x => x.Name);
            builder.Entity<DreamInterprete>().HasIndex(x => x.DreamId);
           
        }
    }
}
