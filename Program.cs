using Npgsql;
using Snake;
using System;
using System.Diagnostics;
using System.Linq;
using static System.Console;

namespace SnakeGame
{
    class Program
    {
        static void ShowMenu()
        {
            Console.WriteLine("Здравствуйте!");
            Console.WriteLine("Добро пожаловать в игру Змейка!");
            Console.WriteLine("1. Запустить игру");
            Console.WriteLine("2. Посмотреть правила игры");
            Console.WriteLine("3. Посмотреть игроков и их счёт");
            Console.WriteLine("4. Зарегистрироваться");
            Console.WriteLine("0. Выход");
        }
        static void Main()
        {
            ShowMenu();

            DB db = new DB();

            string currentPlayer = "";

            bool check = true;

            while (check)
            {

                Console.Clear();
                ShowMenu();

                Console.WriteLine();
                Console.WriteLine("Выберите что Вы хотите сделать:");

                int numb = int.Parse(Console.ReadLine());

                switch (numb)
                {
                    case 1:
                        {
                            

                            int score = StartGame();

                            db.UpdateScore(currentPlayer, score);

                            break;
                        }

                    case 2:
                        {
                            Console.WriteLine("Управление:");
                            Console.WriteLine("W - вверх");
                            Console.WriteLine("A - влево");
                            Console.WriteLine("S - вниз");
                            Console.WriteLine("D - вправо");
                            Console.WriteLine("Не врезайтесь в стены и в себя.");

                            break;
                        }

                    case 3:
                        {
                            db.GetPlayers();
                            break;
                        }

                    case 4:
                        {
                            Console.WriteLine("Введите своё имя:");

                            string name = Console.ReadLine();

                            if (name.Length > 0)
                            {
                                db.AddPlayer(name);

                                currentPlayer = name;

                                Console.WriteLine("Игрок успешно зарегистрирован!");
                            }
                            else
                            {
                                Console.WriteLine("Имя не может быть пустым!");
                            }

                            break;
                        }

                    case 0:
                        {
                            check = false;
                            break;
                        }

                    default:
                        {
                            Console.WriteLine("Неверный пункт меню.");
                            break;
                        }
                }
            }
        }

        private const int MapWight = 30;
        private const int MapHeght = 20;
        private const int ScreenWight = MapWight * 2;
        private const int ScreenHeght = MapHeght * 2;
        private const int FrameMs = 200;

        private const ConsoleColor Border = ConsoleColor.Yellow;
        private const ConsoleColor HeadColor = ConsoleColor.Blue;
        private const ConsoleColor BodyColor = ConsoleColor.DarkBlue;
        private const ConsoleColor FoodColor = ConsoleColor.Red;

        private static readonly Random Random = new Random();

        static int StartGame()
        {
            SetWindowSize(ScreenWight, ScreenHeght);
            SetBufferSize(ScreenWight, ScreenHeght);
            CursorVisible = false;

            Clear();

            DrawBorder();

            Direction currentMovement = Direction.Right;

            var snake = new Snake(10, 5, HeadColor, BodyColor);

            Pixel food = GenFood(snake);
            food.Draw();
            

            int score = 0;

            Stopwatch sw = new Stopwatch();

            while (true)
            {
                sw.Restart();

                Direction oldMovement = currentMovement;

                while (sw.ElapsedMilliseconds <= FrameMs)
                {
                    if (oldMovement == currentMovement)
                    {
                        currentMovement = ReadMovement(currentMovement);
                    }
                }

                bool willEat = false;

                switch (currentMovement)
                {
                    case Direction.Right:
                        willEat = snake.Head.X + 1 == food.X
                                  && snake.Head.Y == food.Y;
                        break;

                    case Direction.Left:
                        willEat = snake.Head.X - 1 == food.X
                                  && snake.Head.Y == food.Y;
                        break;

                    case Direction.Up:
                        willEat = snake.Head.X == food.X
                                  && snake.Head.Y - 1 == food.Y;
                        break;

                    case Direction.Down:
                        willEat = snake.Head.X == food.X
                                  && snake.Head.Y + 1 == food.Y;
                        break;
                }
                snake.Move(currentMovement, willEat);

                if (willEat)
                {
                    score++;

                    food = GenFood(snake);
                    
                    
                    food.Draw();
                }

                if (snake.Head.X == MapWight - 1
                    || snake.Head.X == 0
                    || snake.Head.Y == MapHeght - 1
                    || snake.Head.Y == 0
                    || snake.Body.Any(b => b.X == snake.Head.X && b.Y == snake.Head.Y))
                {
                    break;
                }
            }

            snake.Clear();

            SetCursorPosition(ScreenWight / 3, ScreenHeght / 2);

            WriteLine($"Игра окончена. Ваш счёт: {score}");
            Console.ReadKey();
            Console.Clear();
            return score;
            
        }

        static Pixel GenFood(Snake snake)
        {
            Pixel food;

            int count = 0;

            do
            {
                count++;

                food = FoodFactory.CreateFood(
                    MapWight,
                    MapHeght,
                    FoodColor);
            }
            while (
                (snake.Head.X == food.X && snake.Head.Y == food.Y)
                || snake.Body.Any(b => b.X == food.X && b.Y == food.Y)
            );

            

            return food;
        }
        static Direction ReadMovement(Direction currentDirection)
        {
            if (!KeyAvailable)
                return currentDirection;

            ConsoleKey key = ReadKey(true).Key;

            currentDirection = key switch
            {
                ConsoleKey.W when currentDirection != Direction.Down => Direction.Up,
                ConsoleKey.A when currentDirection != Direction.Right => Direction.Left,
                ConsoleKey.S when currentDirection != Direction.Up => Direction.Down,
                ConsoleKey.D when currentDirection != Direction.Left => Direction.Right,
                _ => currentDirection
            };

            return currentDirection;
        }

        static void DrawBorder()
        {
            for (int i = 0; i < MapWight; i++)
            {
                new Pixel(i, 0, Border).Draw();
                new Pixel(i, MapHeght - 1, Border).Draw();
            }

            for (int i = 0; i < MapHeght; i++)
            {
                new Pixel(0, i, Border).Draw();
                new Pixel(MapWight - 1, i, Border).Draw();
            }
        }
    }

}
