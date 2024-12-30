using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RPG_Game.Data;

namespace RPG_Game
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddDbContext<GameDbContext>(options =>
                    options.UseSqlServer("Server=DESKTOP-CTSCET9\\SQLEXPRESS01;Database=RPG_Game;Trusted_Connection=True;TrustServerCertificate=True"))
                .BuildServiceProvider();

            var dbContext = serviceProvider.GetRequiredService<GameDbContext>();


            Player player = null!;

            Game game = new Game(player);

            game.Start();
        }

    }
}
