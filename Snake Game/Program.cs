using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

class Program
{
    static void Main()
    {
        int screenwidth = 34; // Increased by 2 to account for borders
        int screenheight = 18; // Increased by 2 to account for borders
        Console.SetWindowSize(screenwidth, screenheight);
        Console.SetBufferSize(screenwidth, screenheight);

        Random randomnummer = new Random();
        int score = 5;
        int gameover = 0;
        int maxlevel = 500;
        int speed = 100;
        int[] xpos = new int[50];
        int[] ypos = new int[50];
        int appleX = randomnummer.Next(1, screenwidth - 1); // Adjusted to avoid border
        int appleY = randomnummer.Next(1, screenheight - 1); // Adjusted to avoid border
        int level = 1;
        int snakelength = 5;
        int direction = 1; // 1 = right, 2 = left, 3 = up, 4 = down

        for (int i = 0; i < snakelength; i++)
        {
            xpos[i] = 5;
            ypos[i] = 5;
        }

        while (true)
        {
            Console.Clear();

            if (xpos[0] == appleX && ypos[0] == appleY)
            {
                score++;
                snakelength++;
                if (score == maxlevel)
                {
                    Console.WriteLine("You win!");
                    break;
                }
                appleX = randomnummer.Next(1, screenwidth - 1); // Adjusted to avoid border
                appleY = randomnummer.Next(1, screenheight - 1); // Adjusted to avoid border
            }

            for (int i = snakelength - 1; i > 0; i--)
            {
                xpos[i] = xpos[i - 1];
                ypos[i] = ypos[i - 1];
            }

            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.W:
                        direction = 3;
                        break;
                    case ConsoleKey.S:
                        direction = 4;
                        break;
                    case ConsoleKey.A:
                        direction = 2;
                        break;
                    case ConsoleKey.D:
                        direction = 1;
                        break;
                }
            }

            if (direction == 1)
            {
                xpos[0]++;
            }
            if (direction == 2)
            {
                xpos[0]--;
            }
            if (direction == 3)
            {
                ypos[0]--;
            }
            if (direction == 4)
            {
                ypos[0]++;
            }

            for (int i = 0; i < snakelength; i++)
            {
                if (i == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.SetCursorPosition(xpos[i], ypos[i]);
                    Console.Write("0");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.SetCursorPosition(xpos[i], ypos[i]);
                    Console.Write("o");
                }
            }

            if (xpos[0] >= screenwidth - 1 || xpos[0] <= 0 || ypos[0] >= screenheight - 1 || ypos[0] <= 0)
            {
                gameover = 1;
            }

            for (int i = 1; i < snakelength; i++)
            {
                if (xpos[0] == xpos[i] && ypos[0] == ypos[i])
                {
                    gameover = 1;
                }
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(appleX, appleY);
            Console.Write("@");

            if (gameover == 1)
            {
                Console.Clear();
                Console.WriteLine("Game over!");
                Console.WriteLine("Press R to restart or any other key to exit.");

                var restartKey = Console.ReadKey(true);
                if (restartKey.Key == ConsoleKey.R)
                {
                    gameover = 0;
                    score = 5;
                    snakelength = 5;
                    direction = 1;
                    xpos = new int[50];
                    ypos = new int[50];
                    appleX = randomnummer.Next(1, screenwidth - 1); // Adjusted to avoid border
                    appleY = randomnummer.Next(1, screenheight - 1); // Adjusted to avoid border
                    for (int i = 0; i < snakelength; i++)
                    {
                        xpos[i] = 5;
                        ypos[i] = 5;
                    }
                }
                else
                {
                    break;
                }
            }

            Thread.Sleep(speed);
        }
    }
}