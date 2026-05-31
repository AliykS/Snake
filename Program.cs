
using System;
using System.Diagnostics;
using static System.Console;
using Npgsql;
using System.Reflection.Metadata.Ecma335;
using System.Linq;
using System.Threading;

namespace SnakeGame
{
    class Program
    {
        static void Main()
        {

            Console.WriteLine("Здравствуйте!");
            Console.WriteLine("Добро пожаловать в игру Змейка!");
            Console.WriteLine("1. Запустить игру");
            Console.WriteLine("2. Посмотреть правила игры");
            Console.WriteLine("3. Посмотреть игроков и их счёт");
            Console.WriteLine("0. Выход");

            bool check = true;

            while (check)
            {
                Console.WriteLine("Выберите что Вы хотите сделать");
                int numb = int.Parse(Console.ReadLine());

                switch (numb)
                {
                    case 1:
                        StartGame();
                        break;

                    case 0:
                        check = false;
                        break;
                }
            }
        }

        private const int MapWight = 30;
        private const int MapHeght = 20;
        private const int ScreenWight = MapWight * 3;
        private const int ScreenHeght = MapHeght * 3;
        private const int FrameMs = 200;

        private const ConsoleColor Border = ConsoleColor.Gray;
        private const ConsoleColor HeadColor = ConsoleColor.Blue;
        private const ConsoleColor BodyColor = ConsoleColor.DarkBlue;
        private const ConsoleColor FoodColor = ConsoleColor.Red;

        private static readonly Random Random = new Random();

        static void StartGame()
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

                if (snake.Head.X == food.X && snake.Head.Y == food.Y)
                {
                    snake.Move(currentMovement, true);

                    food = GenFood(snake);
                    food.Draw();

                    score++;
                }
                else
                {
                    snake.Move(currentMovement);
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
        }
        static Pixel GenFood(Snake snake)
        {
            Pixel food;

            do
            {
                food = new Pixel(
                    Random.Next(1, MapWight - 2),
                    Random.Next(1, MapHeght - 2),
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