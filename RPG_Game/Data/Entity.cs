using System.ComponentModel.DataAnnotations;

namespace RPG_Game.Data
{
    public class Entity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Strength { get; set; }
        public int Agility { get; set; }
        public int Intelligence { get; set; }
        public int Range { get; set; }
        public char Symbol { get; set; }
        public int Health { get; set; }
        public int Mana { get; set; }
        public int Damage { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
