namespace RPG_Game.Heroes
{
    public class Warrior : Player
    {
        public Warrior()
        {
            Strenght = 3;
            Agility = 3;
            Intelligence = 0;
            Range = 1;
            Symbol = '@';
            Setup();
        }
    }
}
