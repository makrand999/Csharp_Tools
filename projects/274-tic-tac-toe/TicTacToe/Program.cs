namespace TicTacToe;

class Program
{
    private static char[] board = Enumerable.Repeat(' ', 9).ToArray();
    private static readonly int[,] WinPatterns = {
        { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 }, // Rows
        { 0, 3, 6 }, { 1, 4, 7 }, { 2, 5, 8 }, // Columns
        { 0, 4, 8 }, { 2, 4, 6 }               // Diagonals
    };

    static void Main()
    {
        Console.WriteLine("⭕ Tic-Tac-Toe");
        Console.WriteLine("==============\n");

        Console.WriteLine("Game modes:");
        Console.WriteLine("  1. Player vs Player");
        Console.WriteLine("  2. Player vs Computer");
        Console.Write("\nChoose mode (1-2): ");

        bool vsComputer = Console.ReadLine() == "2";

        char currentPlayer = 'X';
        bool gameWon = false;

        for (int turn = 0; turn < 9 && !gameWon; turn++)
        {
            DrawBoard();

            int move;
            if (currentPlayer == 'X' || !vsComputer)
            {
                move = GetPlayerMove(currentPlayer);
            }
            else
            {
                Console.Write("Computer is thinking... ");
                move = GetComputerMove();
                Console.WriteLine(move + 1);
            }

            board[move] = currentPlayer;

            if (CheckWin(currentPlayer))
            {
                DrawBoard();
                string winner = currentPlayer == 'X' ? (vsComputer ? "You" : "Player X") : "Player O";
                Console.WriteLine($"\n🎉 {winner} win!");
                gameWon = true;
            }
            else
            {
                currentPlayer = currentPlayer == 'X' ? 'O' : 'X';
            }
        }

        if (!gameWon)
        {
            DrawBoard();
            Console.WriteLine("\n🤝 It's a draw!");
        }
    }

    private static void DrawBoard()
    {
        Console.Clear();
        Console.WriteLine("⭕ Tic-Tac-Toe\n");

        for (int i = 0; i < 3; i++)
        {
            Console.WriteLine($"  {board[i * 3]} | {board[i * 3 + 1]} | {board[i * 3 + 2]} ");
            if (i < 2) Console.WriteLine(" ---+---+---");
        }

        Console.WriteLine("\nPositions:");
        Console.WriteLine("  1 | 2 | 3 ");
        Console.WriteLine(" ---+---+---");
        Console.WriteLine("  4 | 5 | 6 ");
        Console.WriteLine(" ---+---+---");
        Console.WriteLine("  7 | 8 | 9 ");
        Console.WriteLine();
    }

    private static int GetPlayerMove(char player)
    {
        while (true)
        {
            Console.Write($"Player {player}, enter position (1-9): ");
            string? input = Console.ReadLine();

            if (int.TryParse(input, out int pos) && pos >= 1 && pos <= 9)
            {
                if (board[pos - 1] == ' ')
                {
                    return pos - 1;
                }
                Console.WriteLine("⚠️  That position is already taken!");
            }
            else
            {
                Console.WriteLine("⚠️  Please enter a number between 1 and 9.");
            }
        }
    }

    private static int GetComputerMove()
    {
        // Try to win
        for (int i = 0; i < 9; i++)
        {
            if (board[i] == ' ')
            {
                board[i] = 'O';
                if (CheckWin('O'))
                {
                    board[i] = ' ';
                    return i;
                }
                board[i] = ' ';
            }
        }

        // Block player win
        for (int i = 0; i < 9; i++)
        {
            if (board[i] == ' ')
            {
                board[i] = 'X';
                if (CheckWin('X'))
                {
                    board[i] = ' ';
                    return i;
                }
                board[i] = ' ';
            }
        }

        // Take center
        if (board[4] == ' ') return 4;

        // Take random available
        var available = board.Select((c, i) => c == ' ' ? i : -1).Where(i => i != -1).ToList();
        return available[new Random().Next(available.Count)];
    }

    private static bool CheckWin(char player)
    {
        for (int i = 0; i < 3; i++)
        {
            if (board[WinPatterns[i, 0]] == player &&
                board[WinPatterns[i, 1]] == player &&
                board[WinPatterns[i, 2]] == player)
            {
                return true;
            }
        }
        return false;
    }
}
