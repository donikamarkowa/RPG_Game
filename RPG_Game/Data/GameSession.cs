using System.ComponentModel.DataAnnotations;

namespace RPG_Game.Data
{
    public class GameSession
    {
        [Key]
        public int Id { get; set; }
        public int HeroId { get; set; }
        public virtual Entity Hero { get; set; } = null!;
        public int MonstersKilled { get; set; }
        public DateTime GameStartedAt { get; set; }

    }
}
