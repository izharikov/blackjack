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

        
    }
}