
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
        public DbSet<ChatMessageRecords> messageRecords { get; set; }
        public DbSet<chatSummary> chatSummaries { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<DreamSymbols>().HasIndex(x => x.Name);
            builder.Entity<ChatMessageRecords>().HasIndex(x => x.Id);
            builder.Entity<DreamInterprete>().HasIndex(x => x.DreamId);
            builder.Entity<Dreams>().HasOne(d => d.User).WithMany().HasForeignKey(d => d.UserId).HasPrincipalKey(u => u.Id);
        }
    }

    public class UserValidate : IUserValidator<User>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
        {
            var errors = new List<IdentityError>();
            if (user.UserName.Length < 6)
                errors.Add(new() { Code = "username lenght", Description = "user name must be longer than 5 characters" });
            if(errors.Any())
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            return Task.FromResult(IdentityResult.Success);
        }
    }
    public class PasswordValidate : IPasswordValidator<User>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user, string? password)
        {
            var errors = new List<IdentityError>();
            if(user.UserName == password)
            {
                errors.Add(new() { Code = "user and password same", Description = "username and password must be different" });
            }
            if (errors.Any())
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            return Task.FromResult(IdentityResult.Success);
        }
    }
}
