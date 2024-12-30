using RPG_Game.Data;
using RPG_Game.Enums;
using RPG_Game.Heroes;

namespace RPG_Game
{
    public class Game
    {
        private const int MatrixSize = 10;
        private Player player;
        private Dictionary<(int X, int Y), Monster> monsterPositions = new Dictionary<(int, int), Monster>();
        private GameScreen currentScreen = GameScreen.MainMenu;
        private int playerX = 1, playerY = 1; // Start position of the player
        private int monstersKilled;

        public int MonstersKilled
        {
            get { return monstersKilled; }
            private set { monstersKilled = value; }
        }

        public Game(Player player)
        {
            this.player = player;
        }

        // Start the game
        public void Start()
        {
            while (currentScreen != GameScreen.Exit)
            {
                switch (currentScreen)
                {
                    case GameScreen.MainMenu:
                        MainMenuScreen();
                        break;

                    case GameScreen.CharacterSelect:
                        CharacterSelectScreen();
                        break;

                    case GameScreen.InGame:
                        PlayGame();
                        break;
                }
            }

            Console.WriteLine("Thanks for playing!");
        }

        // See the main menu
        private void MainMenuScreen()
        {
            Console.Clear();
            Console.WriteLine("Welcome!");
            Console.WriteLine("Press any key to play.");
            Console.ReadKey();

            currentScreen = GameScreen.CharacterSelect;
        }

        // Choose character
        private void CharacterSelectScreen()
        {
            Console.Clear();
            Console.WriteLine("Choose character type:");
            Console.WriteLine("Options:");
            Console.WriteLine("1) Warrior ");
            Console.WriteLine("2) Archer ");
            Console.WriteLine("3) Mage ");
            Console.WriteLine("Your pick: ");

            string choice;
            bool validChoice = false;

            while (!validChoice)
            {
                choice = Console.ReadLine()!;
                switch (choice)
                {
                    case "1":
                        player = new Warrior();
                        validChoice = true;
                        break;
                    case "2":
                        player = new Archer();
                        validChoice = true;
                        break;
                    case "3":
                        player = new Mage();
                        validChoice = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }

            player.Setup();
            CustomizeCharacterStats();
            currentScreen = GameScreen.InGame;
        }

        // Custome the points of chosen character
        private void CustomizeCharacterStats()
        {
            Console.Clear();

            int remainingPoints = 3;
            Console.WriteLine("Would you like to buff up your stats before starting?");
            char response;
            while (true)
            {
                Console.WriteLine("Response (Y\\N): ");
                response = Console.ReadKey().KeyChar;
                if (char.ToUpper(response) == 'Y' || char.ToUpper(response) == 'N')
                {
                    break;  // We have valid response
                }
                else
                {
                    Console.WriteLine("\nInvalid input. Please enter 'Y' for yes or 'N' for no.");
                }
            }

            if (char.ToUpper(response) == 'Y')
            {
                while (remainingPoints > 0)
                {
                    Console.Clear();
                    Console.WriteLine($"Remaining Points: {remainingPoints}");
                    Console.WriteLine($"Current points: Strength: {player.Strenght}, Agility: {player.Agility}, Intelligence: {player.Intelligence}");

                    // Add Strength
                    Console.Write("Add to Strength: ");
                    int strengthPoints = GetValidPoints(remainingPoints);
                    player.Strenght += strengthPoints;
                    remainingPoints -= strengthPoints;

                    // If there are points left, allow adding to Agility or Intelligence
                    if (remainingPoints > 0)
                    {
                        Console.Write("Add to Agility: ");
                        int agilityPoints = GetValidPoints(remainingPoints);
                        player.Agility += agilityPoints;
                        remainingPoints -= agilityPoints;
                    }

                    if (remainingPoints > 0)
                    {
                        Console.Write("Add to Intelligence: ");
                        int intelligencePoints = GetValidPoints(remainingPoints);
                        player.Intelligence += intelligencePoints;
                        remainingPoints -= intelligencePoints;
                    }
                }

                player.Setup();
                SaveHeroToDatabase();
            }
        }

        // Check if the points are valid
        private int GetValidPoints(int remainingPoints)
        {
            int points;
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out points) && points >= 0 && points <= remainingPoints)
                {
                    return points;
                }
                else
                {
                    Console.WriteLine($"Invalid points. Please enter a number between 0 and {remainingPoints}.");
                }
            }
        }

        // Start playing
        private void PlayGame()
        {
            SpawnMonster();  

            bool isPlaying = true;

            while (isPlaying)
            {
                DrawGrid();

                // Choose action
                ActionType action = GetPlayerAction();

                if (action == ActionType.Move) // Move
                {
                    MovePlayerAction();
                    SpawnMonster();
                }
                else if (action == ActionType.Attack) // Attack
                {
                    Attack();
                }

                // Move monsters if the player is moved or attack
                MoveMonsters();


                // Check player's health
                if (player.Health <= 0)
                {
                    Console.WriteLine("Game Over!");
                    isPlaying = false;
                    currentScreen = GameScreen.Exit;
                }
            }
        }

        // Choose between move and attack
        private ActionType GetPlayerAction()
        {
            ActionType action;
            bool isValidAction = false;

            do
            {
                Console.WriteLine("Choose action: 1) Move  2) Attack");
                string input = Console.ReadLine()!;
              
                if (Enum.TryParse(input, out action) && Enum.IsDefined(typeof(ActionType), action))
                {
                    isValidAction = true;
                }
                else
                {
                    Console.WriteLine("Invalid action! Try again.");
                }
            } while (!isValidAction);

            return action;
        }

        // Choose direction of the move
        private void MovePlayerAction()
        {
            Console.WriteLine("Enter direction: W, A, S, D, E, X, Q, Z");
            char input = Console.ReadKey().KeyChar;

            if (!MovePlayer(input))
            {
                Console.WriteLine("\nInvalid move! Try again.");
            }
        }

        // Draw the field
        private void DrawGrid()
        {
            Console.Clear();

            Console.WriteLine($"Health: {player.Health} | Mana: {player.Mana}");

            for (int i = 0; i < MatrixSize; i++)
            {
                for (int j = 0; j < MatrixSize; j++)
                {
                    if (i == playerX && j == playerY)
                    {
                        Console.Write(player.Symbol); 
                    }
                    else if (monsterPositions.ContainsKey((i, j)))
                    {
                        Console.Write(monsterPositions[(i, j)].Symbol); 
                    }
                    else
                    {
                        Console.Write('▒'); 
                    }
                }
                Console.WriteLine();
            }

            // See monsters on the field
            Console.WriteLine("Monsters:");
            foreach (var monster in monsterPositions)
            {
                Console.WriteLine($" - Monster at ({monster.Key.X}, {monster.Key.Y}) with {monster.Value.Health} health.");
            }
            Console.WriteLine("Options: W (Move Up), A (Move left), S (Move down), D (Move right), E (Move diagonally up & right), X (Move diagonally down & right) Q (Move diagonally up & left), Z (Move diagonally down & left)");
        }

        // Move player by choosen direction
        private bool MovePlayer(char input)
        {
            int newX = playerX, newY = playerY;

            switch (char.ToUpper(input))
            {
                case 'W': newX--; break; // Move up
                case 'A': newY--; break; // Move left
                case 'S': newX++; break; // Move down
                case 'D': newY++; break; // Move right
                case 'E': newX--; newY++; break; // Move diagonally up & right
                case 'X': newX++; newY++; break; // Move diagonally down & right
                case 'Q': newX--; newY--; break; // Move diagonally up & left
                case 'Z': newX++; newY--; break; // Move diagonally down & left
                default:
                    Console.WriteLine("Invalid move!");
                    return false; // Quit if the move is invalid
            }

            // Check if the position is valid
            if (newX >= 0 && newX < MatrixSize && newY >= 0 && newY < MatrixSize)
            {
                playerX = newX;
                playerY = newY;

                return true;
            }
            else
            {
                Console.WriteLine("\nInvalid move! Out of bounds!");
                return false;
            }
        }

        // Attack the monster
        private void Attack()
        {
            var targets = monsterPositions
                .Where(monster => Math.Abs(monster.Key.X - playerX) <= player.Range &&
                                  Math.Abs(monster.Key.Y - playerY) <= player.Range
                                  // && monster.Value.Health <= player.Damage
                                  )
                .ToList();

            if (!targets.Any())
            {
                Console.WriteLine("No available targets in your range.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Available targets:");
            for (int i = 0; i < targets.Count; i++)
            {
                var monster = targets[i];
                Console.WriteLine($"{i + 1}) target with remaining blood {monster.Value.Health}");
            }

            Console.Write("Which one to attack: ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= targets.Count)
            {
                var selectedMonster = targets[choice - 1];

                player.Health -= Math.Abs(selectedMonster.Value.Health);

                // Remove health of the chosen monster
                if (player.Damage >= selectedMonster.Value.Health)
                {
                    selectedMonster.Value.Health = 0;                   
                }



                // Check if the monster is defeated
                if (selectedMonster.Value.Health <= 0)
                {
                    MonstersKilled++;
                    selectedMonster.Value.Health = 0;
                    Console.WriteLine($"Monster at ({selectedMonster.Key.X}, {selectedMonster.Key.Y}) was defeated!");
                    monsterPositions.Remove(selectedMonster.Key);
                }
                else
                {
                    Console.WriteLine($"Monster at ({selectedMonster.Key.X}, {selectedMonster.Key.Y}) now has {selectedMonster.Value.Health} HP.");
                }

                // Check if the monster is still alive and has a higher damage than the player
                if (selectedMonster.Value.Health > 0 && selectedMonster.Value.Damage > player.Damage)
                {
                    // Monster attacks the player
                    player.Health -= selectedMonster.Value.Damage;
                    Console.WriteLine($"Monster attacks back! Your health is now {player.Health}");

                    // Check if the player is dead after the monster's attack
                    if (player.Health <= 0)
                    {
                        Console.WriteLine("Game Over! Your health has reached zero.");
                        currentScreen = GameScreen.Exit;
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("No counterattack from monster.");
                }

                CheckWin();
            }
            else
            {
                Console.WriteLine("\nInvalid choice!");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        // Check if the player wins
        private void CheckWin()
        {
            if (monsterPositions.Count == 0)
            {
                Console.WriteLine("Congratulations! You've defeated all monsters!");
                SaveGameToDatabase();
                currentScreen = GameScreen.Exit;
            }
        }

        // Moving monsters
        private void MoveMonsters()
        {
            var updatedMonsterPositions = new Dictionary<(int X, int Y), Monster>();

            foreach (var monster in monsterPositions)
            {
                int monsterX = monster.Key.X;
                int monsterY = monster.Key.Y;

                // Check if the mosnter is in range of the hero
                if (Math.Abs(monsterX - playerX) <= 1 && Math.Abs(monsterY - playerY) <= 1)
                {
                    // Monster attacks the player if it has higher damage
                    if (monster.Value.Damage > player.Damage)
                    {
                        player.Health -= monster.Value.Damage;
                        Console.WriteLine($"Monster at ({monsterX}, {monsterY}) attacks! Player's health is now {player.Health}");

                        if (player.Health <= 0)
                        {
                            Console.WriteLine("Game Over!");
                            SaveGameToDatabase();
                            Environment.Exit(0); // Exit the game
                        }
                    }
                }
                else
                {
                    if (monsterX < playerX) monsterX++;
                    else if (monsterX > playerX) monsterX--;

                    if (monsterY < playerY) monsterY++;
                    else if (monsterY > playerY) monsterY--;

                    // Check if new position of the monster is not occupied by player
                    if ((monsterX != playerX || monsterY != playerY) && !monsterPositions.ContainsKey((monsterX, monsterY)))
                    {
                        updatedMonsterPositions[(monsterX, monsterY)] = monster.Value;
                    }
                }
            }

            monsterPositions = updatedMonsterPositions;
        }

        // Add monsters on the field
        private void SpawnMonster()
        {
            Random random = new Random();
            int x, y;

            do
            {
                x = random.Next(0, MatrixSize);
                y = random.Next(0, MatrixSize);
            } while ((x == playerX && y == playerY) || monsterPositions.ContainsKey((x, y)));

            Monster newMonster = new Monster();
            monsterPositions[(x, y)] = newMonster;

            //Console.WriteLine($"A monster has spawned at ({x}, {y}) with {newMonster.Health} HP.");

            DrawGrid();
        }

        private void SaveHeroToDatabase()
        {
            using (var context = new GameDbContext())
            {

                var hero = new Entity
                {
                    Name = player.GetType().Name,
                    Strength = player.Strenght,
                    Agility = player.Agility,
                    Intelligence = player.Intelligence,
                    CreatedAt = DateTime.Now
                };

                context.Heroes.Add(hero);
                context.SaveChanges();

                player.Id = hero.Id;
            }
        }

        private void SaveGameToDatabase()
        {
            using (var context = new GameDbContext())
            {
                var game = new GameSession
                {
                    HeroId = player.Id, 
                    MonstersKilled = player.MonstersKilled, 
                    GameStartedAt = DateTime.Now
                };

                context.GameSessions.Add(game);
                context.SaveChanges();
            }
        }

    }
}
