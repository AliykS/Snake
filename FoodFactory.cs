using SnakeGame;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snake;
public class FoodFactory
{
    private static Random random = new Random();
    
    public static Pixel CreateFood(
        int mapWidth,
        int mapHeight,
        ConsoleColor color)
    {
        return new Pixel(
            random.Next(2, mapWidth - 3),
            random.Next(2, mapHeight - 3),
            color);
    }
}

