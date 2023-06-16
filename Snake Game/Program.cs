using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;


class Program
{
    static List<User> users = new List<User>();
    static User loggedInUser = null;
    static int score = 0;


    public static void Main()
    {
        int screenwidth = 34; // Increased by 2 to account for borders
        int screenheight = 18; // Increased by 2 to account for borders

       

        Console.SetWindowSize(screenwidth, screenheight);
        Console.SetBufferSize(screenwidth, screenheight);

        bool loggedIn = false;

        while (!loggedIn)
        {
            Console.WriteLine("Please choose an option:");
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Exit");

            Console.Write("Option: ");
            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    Register();
                    break;
                case "2":
                    loggedIn = Login();
                    break;
                case "3":
                    return;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }

            Console.WriteLine();
        }

        while (true)
        {
            if (loggedInUser == null)
            {
                Console.WriteLine("Please login or register to play the game.");
                Console.ReadKey();
                break;
            }

            ShowStartScreen();

            Random randomnummer = new Random();

            int gameover = 0;

            int speed = 150;
            int[] xpos = new int[50];
            int[] ypos = new int[50];
            int appleX = randomnummer.Next(1, screenwidth - 1); // Adjusted to avoid border
            int appleY = randomnummer.Next(1, screenheight - 1); // Adjusted to avoid border
            int level = 1;
            int snakelength = 5;
            int direction = 1; // 1 = right, 2 = left, 3 = up, 4 = down

            int scoreX = screenwidth - 15; // Adjusted to make room for the username
            int scoreY = 0;

            int highScore = 0;
            List<int> previousScores = new List<int>();

            int usernameX = 0; // X position of the username on the screen
            int usernameY = 0; // Y position of the username on the screen

            ConsoleColor borderColor = ConsoleColor.White;
            ConsoleColor defaultColor = ConsoleColor.Green;
            int colorChangeCount = 0; // Variable to track the number of color changes

            for (int i = 0; i < snakelength; i++)
            {
                xpos[i] = 5;
                ypos[i] = 5;
            }

            int appleCount = 0; // Variable to track the number of apples consumed

            

            while (true)
            {
                Console.Clear();

                Console.ForegroundColor = borderColor;
                for (int i = 0; i < screenwidth; i++)
                {
                    Console.SetCursorPosition(i, 0);
                    Console.Write("-");
                    Console.SetCursorPosition(i, screenheight - 1);
                    Console.Write("-");
                }
                for (int i = 0; i < screenheight; i++)
                {
                    Console.SetCursorPosition(0, i);
                    Console.Write(" |");
                    Console.SetCursorPosition(screenwidth - 1, i);
                    Console.Write(" |");
                }

                if (xpos[0] == appleX && ypos[0] == appleY)
                {
                    score++;
                    snakelength++;
                    appleX = randomnummer.Next(1, screenwidth - 1); // Adjusted to avoid border
                    appleY = randomnummer.Next(1, screenheight - 1); // Adjusted to avoid border

                    appleCount++; // Increment apple count
                    colorChangeCount++; // Increment color change count

                    if (appleCount % 5 == 0) // Check if apple count is a multiple of 5
                    {
                        speed -= 10; // Decrease the delay between snake movements to increase speed
                    }

                    if (colorChangeCount % 5 == 0) // Check if color change count is a multiple of 5
                    {
                        defaultColor = GetRandomConsoleColor(); // Change snake color randomly
                    }
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
                        Console.ForegroundColor = defaultColor;
                        Console.SetCursorPosition(xpos[i], ypos[i]);
                        Console.Write("@");
                    }
                    else
                    {
                        Console.ForegroundColor = defaultColor;
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

                // Update username position
                usernameX = 0;
                usernameY = scoreY;

                // Display username
                Console.SetCursorPosition(usernameX, usernameY);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"Username: {loggedInUser.Username}");

                // Update score display
                Console.SetCursorPosition(scoreX, scoreY);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"Score: {score}");

                Console.ForegroundColor = ConsoleColor.Red;
                Console.SetCursorPosition(appleX, appleY);
                Console.Write("@");

                if (gameover == 1)
                {
                    Console.Clear();
                    Console.WriteLine("Game over!");

                    // Update high score and previous scores
                    if (score > highScore)
                    {
                        highScore = score;
                    }

                    if (previousScores.Count < 2)
                    {
                        previousScores.Add(score);
                    }
                    else if (score > previousScores[1])
                    {
                        previousScores[0] = previousScores[1];
                        previousScores[1] = score;
                    }
                    else if (score > previousScores[0])
                    {
                        previousScores[0] = score;
                    }

                    Console.WriteLine($"High Score: {highScore}");
                    Console.WriteLine("Previous Scores:");
                    foreach (int prevScore in previousScores)
                    {
                        Console.WriteLine(prevScore);
                    }

                    Console.WriteLine();
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    break;
                }

                Thread.Sleep(speed);
            }
        }
    }

    static void ShowStartScreen()
    {
        Console.Clear();
        Console.WriteLine("Welcome to Snake Game!");
        Console.WriteLine("-----------------------");
        Console.WriteLine();
    }

    static void Register()
    {
        Console.WriteLine("Please enter a username:");
        string username = Console.ReadLine();
        Console.WriteLine("Please enter a password:");
        string password = Console.ReadLine();

        users.Add(new User(username, password));
        Console.WriteLine("Registration successful. You can now login.");
        Console.WriteLine();
    }

    static bool Login()
    {
        Console.WriteLine("Please enter your username:");
        string username = Console.ReadLine();
        Console.WriteLine("Please enter your password:");
        string password = Console.ReadLine();

        foreach (User user in users)
        {
            if (user.Username == username && user.Password == password)
            {
                loggedInUser = user;
                Console.WriteLine("Login successful!");
                Console.WriteLine();
                return true;
            }
        }

        Console.WriteLine("Invalid username or password. Please try again.");
        Console.WriteLine();
        return false;
    }

    static ConsoleColor GetRandomConsoleColor()
    {
        var consoleColors = Enum.GetValues(typeof(ConsoleColor));
        return (ConsoleColor)consoleColors.GetValue(new Random().Next(consoleColors.Length));
    }
}

class User
{
    public string Username { get; set; }
    public string Password { get; set; }

    public User(string username, string password)
    {
        Username = username;
        Password = password;
    }
}