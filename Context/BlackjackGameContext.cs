using BlackjackGame.Model;
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

        public virtual DbSet<GameResult> GameResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var ugrEntity = modelBuilder
                .Entity<UserGameResult>();
            ugrEntity
                .HasKey(ugr => new {ugr.GameResultId, ugr.UserId});
            ugrEntity
                .HasOne(ugr => ugr.User)
                .WithMany(u => u.UserGameResults)
                .HasForeignKey(gameRes => gameRes.UserId);
            ugrEntity
                .HasOne(ugr => ugr.GameResult)
                .WithMany(gameRes => gameRes.UserGameResults)
                .HasForeignKey(gameRes => gameRes.GameResultId);
        }
        
        
        
    }
}