namespace Chess;

class Program
{
    private static char[,] board = new char[8, 8];
    private static bool whiteTurn = true;
    private static bool gameOver;
    private static string? lastMove;

    static void Main()
    {
        Console.WriteLine("♟️  Chess (2 Player)");
        Console.WriteLine("===================\n");

        InitializeBoard();

        while (!gameOver)
        {
            DrawBoard();
            GetMove();
        }
    }

    private static void InitializeBoard()
    {
        // Clear board
        for (int r = 0; r < 8; r++)
            for (int c = 0; c < 8; c++)
                board[r, c] = ' ';

        // Place pawns
        for (int c = 0; c < 8; c++)
        {
            board[1, c] = '♟'; // Black pawns
            board[6, c] = '♙'; // White pawns
        }

        // Place pieces
        char[] blackPieces = { '♜', '♞', '♝', '♛', '♚', '♝', '♞', '♜' };
        char[] whitePieces = { '♖', '♘', '♗', '♕', '♔', '♗', '♘', '♖' };

        for (int c = 0; c < 8; c++)
        {
            board[0, c] = blackPieces[c];
            board[7, c] = whitePieces[c];
        }
    }

    private static void DrawBoard()
    {
        Console.Clear();
        Console.WriteLine("♟️  Chess (2 Player)\n");

        if (!string.IsNullOrEmpty(lastMove))
            Console.WriteLine($"Last move: {lastMove}\n");

        Console.WriteLine(whiteTurn ? "White's turn (♙)" : "Black's turn (♟)");
        Console.WriteLine("Format: from_row from_col to_row to_col (e.g., 6 0 4 0)\n");

        // Column letters
        Console.Write("    ");
        for (char c = 'a'; c <= 'h'; c++) Console.Write($" {c} ");
        Console.WriteLine();

        for (int r = 0; r < 8; r++)
        {
            Console.Write($"  {(8 - r)} ┌");
            for (int c = 0; c < 8; c++) Console.Write("───");
            Console.WriteLine("┐");

            Console.Write($"  {(8 - r)} │");
            for (int c = 0; c < 8; c++)
            {
                char piece = board[r, c];
                // Highlight based on turn
                bool isWhite = "♙♖♘♗♕♔".Contains(piece);
                bool isBlack = "♟♜♞♝♛♚".Contains(piece);
                
                if (piece == ' ')
                    Console.Write(" · ");
                else
                    Console.Write($" {piece} ");
            }
            Console.WriteLine("│");
        }

        Console.Write("    └");
        for (int c = 0; c < 8; c++) Console.Write("───");
        Console.WriteLine("┘");

        // Column letters again
        Console.Write("    ");
        for (char c = 'a'; c <= 'h'; c++) Console.Write($" {c} ");
        Console.WriteLine();
    }

    private static void GetMove()
    {
        Console.Write("\nMove: ");
        string? input = Console.ReadLine();

        if (string.IsNullOrEmpty(input)) return;

        if (input.ToLower() == "q")
        {
            Console.WriteLine("Game ended.");
            gameOver = true;
            return;
        }

        var parts = input.Split(' ');
        if (parts.Length != 4 ||
            !int.TryParse(parts[0], out int fromRow) ||
            !int.TryParse(parts[1], out int fromCol) ||
            !int.TryParse(parts[2], out int toRow) ||
            !int.TryParse(parts[3], out int toCol))
        {
            Console.WriteLine("Invalid format! Use: from_row from_col to_row to_col");
            return;
        }

        // Convert from 1-8 to 0-7
        fromRow = 8 - fromRow;
        toRow = 8 - toRow;
        fromCol--;
        toCol--;

        if (!IsValidPosition(fromRow, fromCol) || !IsValidPosition(toRow, toCol))
        {
            Console.WriteLine("Invalid position!");
            return;
        }

        char piece = board[fromRow, fromCol];
        if (piece == ' ')
        {
            Console.WriteLine("No piece at that position!");
            return;
        }

        bool isWhite = "♙♖♘♗♕♔".Contains(piece);
        bool isBlack = "♟♜♞♝♛♚".Contains(piece);

        if ((whiteTurn && !isWhite) || (!whiteTurn && !isBlack))
        {
            Console.WriteLine($"It's {(whiteTurn ? "white" : "black")}'s turn!");
            return;
        }

        // Move piece (simplified - no move validation)
        board[toRow, toCol] = piece;
        board[fromRow, fromCol] = ' ';

        lastMove = $"{(char)('a' + fromCol)}{8 - fromRow} → {(char)('a' + toCol)}{8 - toRow}";
        whiteTurn = !whiteTurn;

        // Check for king capture (game over)
        bool whiteKingExists = false, blackKingExists = false;
        for (int r = 0; r < 8; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                if (board[r, c] == '♔') whiteKingExists = true;
                if (board[r, c] == '♚') blackKingExists = true;
            }
        }

        if (!whiteKingExists)
        {
            DrawBoard();
            Console.WriteLine("\n🏆 Black wins! White king captured!");
            gameOver = true;
        }
        else if (!blackKingExists)
        {
            DrawBoard();
            Console.WriteLine("\n🏆 White wins! Black king captured!");
            gameOver = true;
        }
    }

    private static bool IsValidPosition(int row, int col)
    {
        return row >= 0 && row < 8 && col >= 0 && col < 8;
    }
}
