namespace RPG_Game.Heroes
{
    public class Archer : Player
    {
        public Archer()
        {
            Strenght = 2;
            Agility = 4;
            Intelligence = 0;
            Range = 2;
            Symbol = '#';
            Setup();
        }
    }
}
