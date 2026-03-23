namespace Minesweeper;

class Program
{
    private const int Width = 10;
    private const int Height = 8;
    private const int MinesCount = 12;
    private static char[,] board = new char[Width, Height];
    private static char[,] display = new char[Width, Height];
    private static bool[,] revealed = new bool[Width, Height];
    private static bool[,] flagged = new bool[Width, Height];
    private static bool gameOver;
    private static int flagsUsed;
    private static readonly Random Random = new();

    static void Main()
    {
        Console.WriteLine("💣 Minesweeper");
        Console.WriteLine("==============\n");

        InitializeBoard();
        PlaceMines();
        CalculateNumbers();

        while (!gameOver)
        {
            DrawBoard();
            GetMove();
        }

        DrawBoard();
        if (gameOver)
        {
            Console.WriteLine("\n💥 BOOM! You hit a mine!");
        }
        else
        {
            Console.WriteLine("\n🎉 Congratulations! You cleared the field!");
        }
    }

    private static void InitializeBoard()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                board[x, y] = '0';
                display[x, y] = '·';
                revealed[x, y] = false;
                flagged[x, y] = false;
            }
        }
    }

    private static void PlaceMines()
    {
        int placed = 0;
        while (placed < MinesCount)
        {
            int x = Random.Next(Width);
            int y = Random.Next(Height);
            if (board[x, y] != 'X')
            {
                board[x, y] = 'X';
                placed++;
            }
        }
    }

    private static void CalculateNumbers()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (board[x, y] == 'X') continue;

                int count = 0;
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        int nx = x + dx, ny = y + dy;
                        if (nx >= 0 && nx < Width && ny >= 0 && ny < Height && board[nx, ny] == 'X')
                            count++;
                    }
                }
                board[x, y] = count == 0 ? ' ' : count.ToString()[0];
            }
        }
    }

    private static void DrawBoard()
    {
        Console.Clear();
        Console.WriteLine("💣 Minesweeper\n");
        Console.WriteLine($"Mines: {MinesCount} | Flags: {flagsUsed} | Use: [x,y] or F[x,y] to flag\n");

        // Column numbers
        Console.Write("   ");
        for (int x = 0; x < Width; x++) Console.Write($" {x + 1} ");
        Console.WriteLine();

        // Top border
        Console.Write("   ┌");
        for (int x = 0; x < Width; x++) Console.Write("───");
        Console.WriteLine("┐");

        for (int y = 0; y < Height; y++)
        {
            Console.Write($"{y + 1,2}│");
            for (int x = 0; x < Width; x++)
            {
                string c = revealed[x, y] ? board[x, y].ToString() : (flagged[x, y] ? "🚩" : "·");
                Console.Write($" {c} ");
            }
            Console.WriteLine("│");
        }

        // Bottom border
        Console.Write("   └");
        for (int x = 0; x < Width; x++) Console.Write("───");
        Console.WriteLine("┘");
    }

    private static void GetMove()
    {
        Console.Write("\nMove (format: x,y or Fx,y to flag): ");
        string? input = Console.ReadLine();

        if (string.IsNullOrEmpty(input)) return;

        bool flag = input.ToUpper().StartsWith('F');
        string coords = flag ? input[1..] : input;

        var parts = coords.Split(',');
        if (parts.Length != 2 || !int.TryParse(parts[0], out int x) || !int.TryParse(parts[1], out int y))
        {
            Console.WriteLine("Invalid format! Use: x,y");
            return;
        }

        x--; y--; // Convert to 0-based

        if (x < 0 || x >= Width || y < 0 || y >= Height)
        {
            Console.WriteLine("Out of bounds!");
            return;
        }

        if (revealed[x, y])
        {
            Console.WriteLine("Already revealed!");
            return;
        }

        if (flag)
        {
            flagged[x, y] = !flagged[x, y];
            flagsUsed += flagged[x, y] ? 1 : -1;
        }
        else
        {
            if (flagged[x, y])
            {
                Console.WriteLine("Remove flag first!");
                return;
            }
            Reveal(x, y);
        }
    }

    private static void Reveal(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y < Height || revealed[x, y] || flagged[x, y])
            return;

        revealed[x, y] = true;

        if (board[x, y] == 'X')
        {
            gameOver = true;
            // Reveal all mines
            for (int ix = 0; ix < Width; ix++)
                for (int iy = 0; iy < Height; iy++)
                    if (board[ix, iy] == 'X') revealed[ix, iy] = true;
            return;
        }

        // Auto-reveal adjacent if empty
        if (board[x, y] == ' ')
        {
            for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                    Reveal(x + dx, y + dy);
        }
    }
}
