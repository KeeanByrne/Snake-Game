using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;

class Program
{
    static string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\keean\\source\\repos\\Snake Game\\Snake Game\\User.mdf\";Integrated Security=True";
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
                    Console.Write("|");
                    Console.SetCursorPosition(screenwidth - 1, i);
                    Console.Write("|");
                }

                Console.ForegroundColor = defaultColor;

                for (int i = 0; i < snakelength; i++)
                {
                    Console.SetCursorPosition(xpos[i], ypos[i]);
                    if (i == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("O");
                        Console.ForegroundColor = defaultColor;
                    }
                    else
                    {
                        Console.Write("o");
                    }
                }

                Console.SetCursorPosition(appleX, appleY);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("A");

                Console.SetCursorPosition(scoreX, scoreY);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"Score: {score}");

                Console.SetCursorPosition(usernameX, usernameY);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"Username: {loggedInUser.Username}");

                ConsoleKeyInfo info = Console.ReadKey();
                switch (info.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (direction != 4)
                        {
                            direction = 3;
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (direction != 3)
                        {
                            direction = 4;
                        }
                        break;
                    case ConsoleKey.LeftArrow:
                        if (direction != 1)
                        {
                            direction = 2;
                        }
                        break;
                    case ConsoleKey.RightArrow:
                        if (direction != 2)
                        {
                            direction = 1;
                        }
                        break;
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

                for (int i = snakelength - 1; i > 0; i--)
                {
                    xpos[i] = xpos[i - 1];
                    ypos[i] = ypos[i - 1];
                }

                if (xpos[0] == 0 || xpos[0] == screenwidth - 1 || ypos[0] == 0 || ypos[0] == screenheight - 1)
                {
                    gameover = 1;
                }

                for (int i = 2; i < snakelength; i++)
                {
                    if (xpos[0] == xpos[i] && ypos[0] == ypos[i])
                    {
                        gameover = 1;
                    }
                }

                if (xpos[0] == appleX && ypos[0] == appleY)
                {
                    snakelength++;
                    score += 10;
                    appleCount++;
                    colorChangeCount++;

                    if (appleCount % 5 == 0)
                    {
                        level++;
                        speed -= 10;
                    }

                    if (score > highScore)
                    {
                        highScore = score;
                    }

                    previousScores.Add(score);

                    Random random = new Random();
                    appleX = random.Next(1, screenwidth - 1);
                    appleY = random.Next(1, screenheight - 1);
                }

                if (gameover == 1)
                {
                    break;
                }

                Thread.Sleep(speed);
            }

            Console.Clear();

            Console.WriteLine("Game Over!");
            Console.WriteLine($"Score: {score}");
            Console.WriteLine($"High Score: {highScore}");

            if (previousScores.Count > 0)
            {
                Console.WriteLine("Previous Scores:");
                foreach (var prevScore in previousScores)
                {
                    Console.WriteLine(prevScore);
                }
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }


static void SetApplePosition(ref int appleX, ref int appleY, int screenWidth, int screenHeight, int[] xpos, int[] ypos)
    {
        Random random = new Random();

        while (true)
        {
            appleX = random.Next(1, screenWidth - 1);
            appleY = random.Next(1, screenHeight - 1);

            bool collision = false;

            for (int i = 0; i < xpos.Length; i++)
            {
                if (appleX == xpos[i] && appleY == ypos[i])
                {
                    collision = true;
                    break;
                }
            }

            if (!collision)
            {
                break;
            }
        }
    }

    static void ShowStartScreen()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Welcome to Snake Game!");
        Console.WriteLine("-----------------------");
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Logged in as: " + loggedInUser.Username);
        Console.WriteLine("Score: " + score);
        Console.WriteLine();
    }

    static void Register()
    {
        Console.Clear();
        Console.WriteLine("Register");
        Console.WriteLine("--------");

        Console.Write("Username: ");
        string username = Console.ReadLine();

        Console.Write("Password: ");
        string password = Console.ReadLine();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "INSERT INTO [User] (Username, Password) VALUES (@Username, @Password)";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", password);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        Console.WriteLine("Registration successful!");
        Console.WriteLine();
    }

    static bool Login()
    {
        Console.Clear();
        Console.WriteLine("Login");
        Console.WriteLine("-----");

        Console.Write("Username: ");
        string username = Console.ReadLine();

        Console.Write("Password: ");
        string password = Console.ReadLine();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "SELECT * FROM [User] WHERE Username = @Username AND Password = @Password";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", password);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int userID = (int)reader["UserID"];
                        string dbUsername = (string)reader["Username"];

                        loggedInUser = new User(userID, dbUsername);
                        Console.WriteLine("Login successful!");
                        Console.WriteLine();
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Invalid username or password.");
                        Console.WriteLine();
                        return false;
                    }
                }
            }
        }
    }

    static int GenerateUserID()
    {
        int userID = 0;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "SELECT MAX(UserID) FROM [User]";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                var result = command.ExecuteScalar();

                if (result != DBNull.Value)
                {
                    userID = (int)result + 1;
                }
                else
                {
                    userID = 1;
                }
            }
        }

        return userID;
    }

}


class User
{
    public int UserID { get; set; }
    public string Username { get; set; }

    public User(int userID, string username)
    {
        UserID = userID;
        Username = username;
    }
}
