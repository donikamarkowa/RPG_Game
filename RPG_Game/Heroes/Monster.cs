namespace RPG_Game.Heroes
{
    public class Monster : Player
    {
        public Monster()
        {
            Strenght = new Random().Next(1, 4);
            Agility = new Random().Next(1, 4);
            Intelligence = new Random().Next(1, 4);
            Range = 1;
            Symbol = '!';
            Setup();
        }
    }
}
