using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

class Program
{
    static string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\keean\\source\\repos\\Snake Game\\Snake Game\\User.mdf\";Integrated Security=True";
    static List<User> users = new List<User>();
    static User loggedInUser = null;
    static int score = 0;

    // Import the necessary Win32 API functions
    [DllImport("kernel32.dll", ExactSpelling = true)]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    private const int SW_MAXIMIZE = 3; // Window maximize constant

    public static void Main()
    {
        int screenwidth = 40 + 2; // Increased by 2 to account for borders
        int screenheight = 24 + 2; // Increased by 2 to account for borders

        Console.SetWindowSize(screenwidth, screenheight);
        Console.SetBufferSize(screenwidth, screenheight);

        bool loggedIn = false;

        IntPtr consoleWindow = GetConsoleWindow();

        // Maximize the console window
        ShowWindow(consoleWindow, SW_MAXIMIZE);

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
            int score = 0;
            int gameover = 0;

            int speed = 150;
            int[] xpos = new int[50];
            int[] ypos = new int[50];
            int appleX = randomnummer.Next(1, screenwidth - 1); // Adjusted to avoid border
            int appleY = randomnummer.Next(1, screenheight - 1); // Adjusted to avoid border
            int level = 1;
            int snakelength = 5;
            int direction = 1; // 1 = right, 2 = left, 3 = up, 4 = down

            int scoreX = screenwidth - 10;
            int scoreY = 0;

            int highScore = 0;
            List<int> previousScores = new List<int>();

            ConsoleColor borderColor = ConsoleColor.White;
            ConsoleColor defaultColor = ConsoleColor.DarkBlue;

            for (int i = 0; i < snakelength; i++)
            {
                xpos[i] = 5;
                ypos[i] = 5;
            }

            int appleCount = 0; // Variable

            while (true)
            {
                Console.Clear();

                for (int i = 1; i < screenwidth; i++)
                {
                    Console.SetCursorPosition(i, 0);
                    Console.Write("■");
                }

                for (int i = 1; i < screenheight; i++)
                {
                    Console.SetCursorPosition(screenwidth - 1, i);
                    Console.Write("■");
                }

                for (int i = screenwidth - 2; i > 1; i--)
                {
                    Console.SetCursorPosition(i, screenheight - 1);
                    Console.Write("■");
                }

                for (int i = screenheight - 1; i > 0; i--)
                {
                    Console.SetCursorPosition(0, i);
                    Console.Write("■");
                }

                Console.SetCursorPosition(scoreX, scoreY);
                Console.WriteLine("Score: " + score);
                Console.SetCursorPosition(scoreX, scoreY + 1);
                Console.WriteLine("High Score: " + highScore);

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo toets = Console.ReadKey(true);
                    // Use WASD or arrow keys to change direction
                    switch (toets.Key)
                    {
                        case ConsoleKey.UpArrow:
                        case ConsoleKey.W:
                            if (direction != 4)
                            {
                                direction = 3;
                            }
                            break;
                        case ConsoleKey.DownArrow:
                        case ConsoleKey.S:
                            if (direction != 3)
                            {
                                direction = 4;
                            }
                            break;
                        case ConsoleKey.LeftArrow:
                        case ConsoleKey.A:
                            if (direction != 1)
                            {
                                direction = 2;
                            }
                            break;
                        case ConsoleKey.RightArrow:
                        case ConsoleKey.D:
                            if (direction != 2)
                            {
                                direction = 1;
                            }
                            break;
                    }
                }

                switch (direction)
                {
                    case 1:
                        xpos[0]++;
                        break;
                    case 2:
                        xpos[0]--;
                        break;
                    case 3:
                        ypos[0]--;
                        break;
                    case 4:
                        ypos[0]++;
                        break;
                }

                if (xpos[0] == 0 || xpos[0] == screenwidth - 1 || ypos[0] == 0 || ypos[0] == screenheight - 1)
                {
                    gameover = 1;
                    break;
                }

                for (int i = snakelength - 1; i > 0; i--)
                {
                    if (xpos[0] == xpos[i] && ypos[0] == ypos[i])
                    {
                        gameover = 1;
                        break;
                    }
                }

                if (gameover == 1)
                {
                    break;
                }

                if (xpos[0] == appleX && ypos[0] == appleY)
                {
                    snakelength++;
                    score += 10;
                    appleX = randomnummer.Next(1, screenwidth - 1); // Adjusted to avoid border
                    appleY = randomnummer.Next(1, screenheight - 1); // Adjusted to avoid border

                    // Increment appleCount by 1
                    appleCount++;

                    if (appleCount == 5) // Changed condition to 5
                    {
                        appleCount = 0;
                        level++;
                        speed -= 10;
                    }

                    if (score > highScore)
                    {
                        highScore = score;
                    }
                }

                if (previousScores.Count < 3) // Changed condition to 3
                {
                    previousScores.Add(score);
                }
                else
                {
                    previousScores.RemoveAt(0);
                    previousScores.Add(score);
                }

                Console.SetCursorPosition(appleX, appleY);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("■");

                if (xpos[0] > screenwidth - 1)
                {
                    xpos[0] = 1;
                }

                if (xpos[0] < 1)
                {
                    xpos[0] = screenwidth - 1;
                }

                if (ypos[0] > screenheight - 1)
                {
                    ypos[0] = 1;
                }

                if (ypos[0] < 1)
                {
                    ypos[0] = screenheight - 1;
                }

                for (int i = snakelength; i > 0; i--)
                {
                    Console.SetCursorPosition(xpos[i], ypos[i]);
                    Console.ForegroundColor = defaultColor;
                    Console.Write("■");
                }

                if (xpos[0] == appleX && ypos[0] == appleY)
                {
                    xpos[snakelength] = xpos[snakelength - 1];
                    ypos[snakelength] = ypos[snakelength - 1];
                }

                for (int i = snakelength; i > 0; i--)
                {
                    xpos[i] = xpos[i - 1];
                    ypos[i] = ypos[i - 1];
                }

                Console.SetCursorPosition(xpos[0], ypos[0]);
                Console.ForegroundColor = defaultColor;
                Console.Write("■");

                Thread.Sleep(speed);
            }

            ShowGameOverScreen(score, highScore, previousScores);
        }
    }

    static void Register()
    {
        Console.WriteLine("Register");

        Console.Write("Username: ");
        string username = Console.ReadLine();

        Console.Write("Password: ");
        string password = Console.ReadLine();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string query = $"INSERT INTO Users (Username, Password) VALUES ('{username}', '{password}')";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                int result = command.ExecuteNonQuery();

                if (result > 0)
                {
                    Console.WriteLine("Registration successful. Please login to continue.");
                }
                else
                {
                    Console.WriteLine("Registration failed. Please try again.");
                }
            }
        }
    }

    static bool Login()
    {
        Console.WriteLine("Login");

        Console.Write("Username: ");
        string username = Console.ReadLine();

        Console.Write("Password: ");
        string password = Console.ReadLine();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string query = $"SELECT * FROM [User] WHERE Username = '{username}' AND Password = '{password}'";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Console.WriteLine("Login successful. Welcome, " + username + "!");

                        loggedInUser = new User(username, password);
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Login failed. Invalid username or password.");
                        return false;
                    }
                }
            }
        }
    }

    static void ShowStartScreen()
    {
        Console.Clear();
        Console.WriteLine("Welcome to Snake Game!");
        Console.WriteLine();
        Console.WriteLine("Use WASD or arrow keys to control the snake.");
        Console.WriteLine("Eat the red apples to grow and increase your score.");
        Console.WriteLine("Avoid running into the walls or yourself.");
        Console.WriteLine("Every 5 apples, the game level will increase and the snake will move faster.");
        Console.WriteLine();
        Console.WriteLine("Press any key to start...");
        Console.ReadKey();
    }

    static void ShowGameOverScreen(int score, int highScore, List<int> previousScores)
    {
        Console.Clear();
        Console.WriteLine("Game Over!");
        Console.WriteLine();
        Console.WriteLine("Score: " + score);
        Console.WriteLine("High Score: " + highScore);
        Console.WriteLine();
        Console.WriteLine("Previous Scores:");

        foreach (int prevScore in previousScores)
        {
            Console.WriteLine(prevScore);
        }

        Console.WriteLine();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
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