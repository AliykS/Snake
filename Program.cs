using Snake;
using System;
using System.Diagnostics;
using static System.Console;
namespace SnakeGame
{
    class Program
    {

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
        static void Main()
        {
            SetWindowSize(ScreenWight, ScreenHeght);
            SetBufferSize(ScreenWight, ScreenHeght);
            CursorVisible = false;
            while (true)
            {
                StartGame();
                Thread.Sleep(1000);
                ReadKey();
            }
        }

        static void StartGame()
        {
            Clear();
            DrawBorder();
            Direction currentMovement = Direction.Right;
            var Snake = new Snake(10, 5, HeadColor, BodyColor);
            Pixel food = GenFood(Snake);
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
                if (Snake.Head.X == food.X && Snake.Head.Y == food.Y)
                {
                    Snake.Move(currentMovement, true);
                    food = GenFood(Snake);
                    food.Draw();
                    score++;
                }
                else
                {
                    Snake.Move(currentMovement);
                }
                if (Snake.Head.X == MapWight - 1
                    || Snake.Head.X == 0
                    || Snake.Head.Y == MapWight - 1
                    || Snake.Head.Y == 0
                    || Snake.Body.Any(b => b.X == Snake.Head.X && b.Y == Snake.Head.Y))
                    break;
            }
            Snake.Clear();
            SetCursorPosition(ScreenWight / 3, ScreenHeght / 2);
            WriteLine($"Игра Окончена. Вы проиграли, ваш счет {score}");
        }
        static Pixel GenFood(Snake snake)
        {
            Pixel food;
            do
            {
                food = new Pixel(Random.Next(1, MapWight - 2), Random.Next(1, MapHeght - 2), FoodColor);
            } while (snake.Head.X == food.X && snake.Head.Y == food.Y
                || snake.Body.Any(b => b.X == food.X && b.Y == food.Y));
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