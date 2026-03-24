namespace Sudoku;

class Program
{
    private static int[,] puzzle = new int[9, 9];
    private static int[,] solution = new int[9, 9];
    private static int[,] initial = new int[9, 9];
    private static readonly Random Random = new();

    static void Main()
    {
        Console.WriteLine("🔢 Sudoku");
        Console.WriteLine("=========\n");

        GeneratePuzzle();

        while (!IsSolved())
        {
            DrawBoard();
            GetMove();
        }

        DrawBoard();
        Console.WriteLine("\n🎉 Congratulations! You solved the puzzle!");
    }

    private static void GeneratePuzzle()
    {
        // Start with a valid solved board
        Solve(solution);

        // Copy to puzzle
        for (int i = 0; i < 9; i++)
            for (int j = 0; j < 9; j++)
                puzzle[i, j] = solution[i, j];

        // Remove numbers to create puzzle (remove ~50)
        int toRemove = 50;
        while (toRemove > 0)
        {
            int r = Random.Next(9), c = Random.Next(9);
            if (puzzle[r, c] != 0)
            {
                puzzle[r, c] = 0;
                toRemove--;
            }
        }

        // Store initial state
        for (int i = 0; i < 9; i++)
            for (int j = 0; j < 9; j++)
                initial[i, j] = puzzle[i, j];
    }

    private static bool Solve(int[,] board)
    {
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (board[row, col] == 0)
                {
                    var numbers = Enumerable.Range(1, 9).OrderBy(_ => Random.Next()).ToList();
                    foreach (var num in numbers)
                    {
                        if (IsValid(board, row, col, num))
                        {
                            board[row, col] = num;
                            if (Solve(board)) return true;
                            board[row, col] = 0;
                        }
                    }
                    return false;
                }
            }
        }
        return true;
    }

    private static bool IsValid(int[,] board, int row, int col, int num)
    {
        // Check row
        for (int c = 0; c < 9; c++)
            if (board[row, c] == num) return false;

        // Check column
        for (int r = 0; r < 9; r++)
            if (board[r, col] == num) return false;

        // Check 3x3 box
        int boxRow = (row / 3) * 3, boxCol = (col / 3) * 3;
        for (int r = 0; r < 3; r++)
            for (int c = 0; c < 3; c++)
                if (board[boxRow + r, boxCol + c] == num) return false;

        return true;
    }

    private static void DrawBoard()
    {
        Console.Clear();
        Console.WriteLine("🔢 Sudoku\n");
        Console.WriteLine("Enter: row col value (or H for hint, Q to quit)\n");

        // Column numbers
        Console.Write("    ");
        for (int c = 1; c <= 9; c++) Console.Write($" {c} ");
        Console.WriteLine();

        for (int r = 0; r < 9; r++)
        {
            if (r % 3 == 0)
            {
                Console.Write("   ┌");
                for (int c = 0; c < 9; c++)
                {
                    Console.Write(c % 3 == 0 && c > 0 ? "─┬" : "──");
                }
                Console.WriteLine("┐");
            }

            Console.Write($"{r + 1,2} │");
            for (int c = 0; c < 9; c++)
            {
                if (c % 3 == 0 && c > 0) Console.Write("│");
                
                char ch = puzzle[r, c] == 0 ? '·' : puzzle[r, c].ToString()[0];
                bool isInitial = initial[r, c] != 0;
                
                if (isInitial)
                    Console.Write($" {ch} ");
                else
                    Console.Write($"[{ch}]");
            }
            Console.WriteLine("│");
        }

        Console.Write("   └");
        for (int c = 0; c < 9; c++)
        {
            Console.Write(c % 3 == 0 && c > 0 ? "─┴" : "──");
        }
        Console.WriteLine("┘");
    }

    private static void GetMove()
    {
        Console.Write("\nYour move: ");
        string? input = Console.ReadLine()?.Trim().ToUpper();

        if (input == "Q")
        {
            Console.WriteLine("Thanks for playing!");
            Environment.Exit(0);
        }

        if (input == "H")
        {
            GiveHint();
            return;
        }

        var parts = input?.Split(' ');
        if (parts?.Length != 3 || 
            !int.TryParse(parts[0], out int row) || 
            !int.TryParse(parts[1], out int col) || 
            !int.TryParse(parts[2], out int value))
        {
            Console.WriteLine("Invalid! Use: row col value (1-9)");
            return;
        }

        row--; col--; // Convert to 0-based

        if (row < 0 || row > 8 || col < 0 || col > 8)
        {
            Console.WriteLine("Out of bounds!");
            return;
        }

        if (value < 1 || value > 9)
        {
            Console.WriteLine("Value must be 1-9!");
            return;
        }

        if (initial[row, col] != 0)
        {
            Console.WriteLine("Cannot change initial values!");
            return;
        }

        if (value == solution[row, col])
        {
            puzzle[row, col] = value;
            Console.WriteLine("✅ Correct!");
        }
        else
        {
            Console.WriteLine("❌ Incorrect! Try again.");
        }
    }

    private static void GiveHint()
    {
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                if (puzzle[r, c] == 0)
                {
                    puzzle[r, c] = solution[r, c];
                    Console.WriteLine($"💡 Hint: Row {r + 1}, Col {c + 1} = {solution[r, c]}");
                    return;
                }
            }
        }
    }

    private static bool IsSolved()
    {
        for (int r = 0; r < 9; r++)
            for (int c = 0; c < 9; c++)
                if (puzzle[r, c] == 0) return false;
        return true;
    }
}
