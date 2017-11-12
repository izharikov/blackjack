using BlackjackGame.Models;
using Microsoft.EntityFrameworkCore;

namespace BlackjackGame.Context
{
    public class BlackjackGameContext : DbContext
    {
        public BlackjackGameContext(DbContextOptions<BlackjackGameContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Inventory> Inventory { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(
                        @"Server=localhost,1401;Database=TestDB;User Id=SA;Password=kind6iVy;Trusted_Connection=False;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
//            modelBuilder.Entity<User>()
//                .Ignore(c => c.AccessFailedCount)
//                .Ignore(c => c.ConcurrencyStamp)
//                .Ignore(c => c.Email)
//                .Ignore(c => c.EmailConfirmed)
//                .Ignore(c => c.LockoutEnabled)
//                .Ignore(c => c.LockoutEnd)
//                .Ignore(c => c.NormalizedEmail)
//                .Ignore(c => c.NormalizedUserName)
//                .Ignore(c => c.PasswordHash)
//                .Ignore(c => c.PhoneNumber)
//                .Ignore(c => c.PhoneNumberConfirmed)
//                .Ignore(c => c.SecurityStamp)
//                .Ignore(c => c.TwoFactorEnabled);
        }
        
        
    }
}