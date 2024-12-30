using Microsoft.EntityFrameworkCore;

namespace RPG_Game.Data
{
    public class GameDbContext : DbContext
    {
        public DbSet<Entity> Heroes { get; set; }
        public DbSet<GameSession> GameSessions { get; set; }

        public GameDbContext(DbContextOptions<GameDbContext> options)
          : base(options)
        {
        }

        public GameDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=DESKTOP-CTSCET9\\SQLEXPRESS01;Database=RPG_Game;Trusted_Connection=True;TrustServerCertificate=True");
            }
        }



    }
}
