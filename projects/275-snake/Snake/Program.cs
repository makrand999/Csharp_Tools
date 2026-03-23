namespace Snake;

class Program
{
    private const int Width = 30;
    private const int Height = 15;
    private static readonly List<Point> Snake = new();
    private static Point Food;
    private static int dx = 1, dy = 0;
    private static int Score;
    private static bool GameOver;
    private static readonly Random Random = new();

    static void Main()
    {
        Console.WriteLine("🐍 Snake Game");
        Console.WriteLine("=============\n");
        Console.WriteLine("Controls: W/A/S/D or Arrow Keys");
        Console.WriteLine("Press any key to start...\n");
        Console.ReadKey();

        Snake.Add(new Point(Width / 2, Height / 2));
        Snake.Add(new Point(Width / 2 - 1, Height / 2));
        Snake.Add(new Point(Width / 2 - 2, Height / 2));

        PlaceFood();
        Console.TreatControlCAsInput = true;

        var inputTask = Task.Run(HandleInput);
        
        while (!GameOver)
        {
            Move();
            Draw();
            Thread.Sleep(150);
        }

        Console.WriteLine($"\n💀 Game Over! Final Score: {Score}");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    private static void HandleInput()
    {
        while (!GameOver)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        if (dy != 1) { dx = 0; dy = -1; }
                        break;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        if (dy != -1) { dx = 0; dy = 1; }
                        break;
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                        if (dx != 1) { dx = -1; dy = 0; }
                        break;
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                        if (dx != -1) { dx = 1; dy = 0; }
                        break;
                    case ConsoleKey.C:
                        GameOver = true;
                        break;
                }
            }
        }
    }

    private static void Move()
    {
        var head = Snake[0];
        int newX = head.X + dx;
        int newY = head.Y + dy;

        if (newX < 0 || newX >= Width || newY < 0 || newY >= Height)
        {
            GameOver = true;
            return;
        }

        if (Snake.Any(s => s.X == newX && s.Y == newY))
        {
            GameOver = true;
            return;
        }

        Snake.Insert(0, new Point(newX, newY));

        if (newX == Food.X && newY == Food.Y)
        {
            Score += 10;
            PlaceFood();
        }
        else
        {
            Snake.RemoveAt(Snake.Count - 1);
        }
    }

    private static void PlaceFood()
    {
        do
        {
            Food = new Point(Random.Next(Width), Random.Next(Height));
        } while (Snake.Any(s => s.X == Food.X && s.Y == Food.Y));
    }

    private static void Draw()
    {
        Console.SetCursorPosition(0, 0);
        var buffer = new System.Text.StringBuilder();

        buffer.AppendLine("┌" + new string('─', Width) + "┐");

        for (int y = 0; y < Height; y++)
        {
            buffer.Append("│");
            for (int x = 0; x < Width; x++)
            {
                if (Snake.Any(s => s.X == x && s.Y == y))
                    buffer.Append(Snake[0].X == x && Snake[0].Y == y ? "█" : "▓");
                else if (Food.X == x && Food.Y == y)
                    buffer.Append("●");
                else
                    buffer.Append(" ");
            }
            buffer.AppendLine("│");
        }

        buffer.AppendLine("└" + new string('─', Width) + "┘");
        buffer.AppendLine($"Score: {Score} | Length: {Snake.Count}");
        buffer.AppendLine("Controls: W/A/S/D | Press C to quit");

        Console.Write(buffer.ToString());
    }
}

class Point
{
    public int X { get; set; }
    public int Y { get; set; }
    public Point(int x, int y) { X = x; Y = y; }
}
