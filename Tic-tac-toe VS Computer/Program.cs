using System;
using System.Media;

namespace Tic_tac_toe
{
    class Program
    {
        static char[,] board = new char[3, 3];
        static int rows = 3;
        static int cols = 3;
        static char currentPlayer;
        static char currentComputer;

        public static void Main()
        {
            Console.WriteLine("Welcome to my Tic-Tac-Toe Game");

            InitializeBoard(rows, cols, board);
            if (!ChoosingPosition())
            {
                return;
            }

            PlayGame();
        }

        static void InitializeBoard(int rows, int cols, char[,] board)
        {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    board[row, col] = ' ';
                }
            }
        }

        static void DrawingBoard(int rows, int cols, char[,] board)
        {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    Console.ForegroundColor = (board[row, col] == 'X') ? ConsoleColor.Blue : ConsoleColor.Red;

                    Console.Write($" {board[row, col]} ");

                    if (col < 2)
                    {
                        Console.Write("|");
                    }
                }
                Console.WriteLine();

                if (row < 2)
                {
                    Console.WriteLine("-----------");
                }
            }
            Console.ResetColor();
        }

        static bool IsBoardFull(int rows, int cols, char[,] board)
        {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (board[row, col] == ' ')
                    {
                        return false; // This means there is still a space on the board
                    }
                }
            }

            return true; // The board is full
        }

        static void PlayGame()
        {
            DrawingBoard(rows, cols, board);

            while (true)
            {
                if (!PlayerMove() && !ComputerMove())
                {
                    return; // Game over
                }

                if (IsBoardFull(rows, cols, board))
                {
                    //DrawingBoard(rows, cols, board);
                    Console.WriteLine("Game is a draw!");
                    break; // Exit the loop when the game is a draw
                }

                if (CheckIfPlayerWin(board))
                {
                    //DrawingBoard(rows, cols, board);
                    Console.WriteLine($"Player {currentPlayer} wins !");
                    var player = new SoundPlayer("roblox win sound effect.wav");
                    player.PlaySync();
                    break; // Exit the loop when a player wins
                }

                //if (!ComputerMove())
                //{
                //    return; // Game over
                //}

                if (CheckIfComputerWin(board))
                {
                    //DrawingBoard(rows, cols, board);
                    Console.WriteLine($"Player {currentPlayer} lose! Computer win.");
                    var player = new SoundPlayer("Lose sound effects.wav");
                    player.PlaySync();
                    break; // Exit the loop when the computer wins
                }
            }
        }

        static bool ChoosingPosition()
        {
            if (currentPlayer != default(char))
            {
                // Symbols have already been chosen, no need to ask again.
                return true;
            }

            Console.Write("Player, choose symbol (X-O): ");
            string input = Console.ReadLine();


            do
            {
                if (input == "X" || input == "x")
                {
                    currentPlayer = 'X';
                    currentComputer = 'O';
                    return true;
                }
                else if (input == "O" || input == "o")
                {
                    currentPlayer = 'O';
                    currentComputer = 'X';
                    return true;
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please choose X or O.");
                    input=Console.ReadLine();
                }
            } while (true);
        }

        static bool PlayerMove()
        {
            //int row;
            //int col;

            do
            {
                Console.WriteLine($"Player {currentPlayer} choose row and column (0-2): ");

                Console.Write("The row: ");
                string input = Console.ReadLine();
                Console.Write("The col: ");
                string input2 = Console.ReadLine();

                if (IsMoveValid(input, input2, board))
                {
                    int row = int.Parse(input);
                    int col = int.Parse(input2);

                    board[row, col] = currentPlayer;
                    ComputerMove();
                    DrawingBoard(rows, cols, board);
                    return true;
                }
                else
                {
                    Console.WriteLine("Invalid move! Try again.");
                    //input = Console.ReadLine();
                }
            } while (true);
        }

        static int MiniMax(char[,] board, char player)
        {
            if (CheckIfPlayerWin(board)) return -1; // Player wins
            if (CheckIfComputerWin(board)) return 1; // Computer wins
            if (IsBoardFull(3, 3, board)) return 0; // It's a draw

            List<int> scores = new List<int>();
            List<(int, int)> moves = new List<(int, int)>();

            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    if (board[row, col] == ' ')
                    {
                        board[row, col] = player;
                        int score = MiniMax(board, player == 'X' ? 'O' : 'X');
                        scores.Add(score);
                        moves.Add((row, col));
                        board[row, col] = ' '; 
                    }
                }
            }

            int bestMove = 0;
            if (player == currentComputer)
            {
                int maxScore = -2; 
                for (int i = 0; i < scores.Count; i++)
                {
                    if (scores[i] > maxScore)
                    {
                        maxScore = scores[i];
                        bestMove = i;
                    }
                }
            }
            else
            {
                int minScore = 2; 
                for (int i = 0; i < scores.Count; i++)
                {
                    if (scores[i] < minScore)
                    {
                        minScore = scores[i];
                        bestMove = i;
                    }
                }
            }

            return scores[bestMove];
        }

        static bool ComputerMove()
        {
            int bestScore = -2; 
            int bestRow = -1;
            int bestCol = -1;

            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    if (board[row, col] == ' ')
                    {
                        board[row, col] = currentComputer;
                        int score = MiniMax(board, currentPlayer);
                        board[row, col] = ' '; 

                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestRow = row;
                            bestCol = col;
                        }
                    }
                }
            }

            if (bestRow != -1 && bestCol != -1)
            {
                board[bestRow, bestCol] = currentComputer;
                //DrawingBoard(rows, cols, board);
                Console.WriteLine($"Computer chooses row {bestRow} and column {bestCol}");
                return true;
            }

            return false;
        }

        static bool IsMoveValid(string input, string input2, char[,] board)
        {
            return int.TryParse(input, out int row) && int.TryParse(input2, out int col) && row >= 0 && row < 3 && col >= 0 && col < 3 && board[row, col] == ' ';
        }

        static bool CheckIfComputerWin(char[,] board)
        {
            string topRow = board[0, 0].ToString() + board[0, 1].ToString() + board[0, 2].ToString();
            string midRow = board[1, 0].ToString() + board[1, 1].ToString() + board[1, 2].ToString();
            string botRow = board[2, 0].ToString() + board[2, 1].ToString() + board[2, 2].ToString();
            string firstCol = board[0, 0].ToString() + board[1, 0].ToString() + board[2, 0].ToString();
            string secondCol = board[0, 1].ToString() + board[1, 1].ToString() + board[2, 1].ToString();
            string thirdCol = board[0, 2].ToString() + board[1, 2].ToString() + board[2, 2].ToString();
            string diagonal = board[0, 0].ToString() + board[1, 1].ToString() + board[2, 2].ToString();
            string otherDiagonal = board[0, 2].ToString() + board[1, 1].ToString() + board[2, 0].ToString();

            string playerTriple = currentComputer.ToString() + currentComputer.ToString() + currentComputer.ToString();

            if (topRow.Equals(playerTriple)
                || midRow.Equals(playerTriple)
                || botRow.Equals(playerTriple)
                || firstCol.Equals(playerTriple)
                || secondCol.Equals(playerTriple)
                || thirdCol.Equals(playerTriple)
                || diagonal.Equals(playerTriple)
                || otherDiagonal.Equals(playerTriple))
            {
                return true;
            }

            return false;
        }

        static bool CheckIfPlayerWin(char[,] board)
        {
            string topRow = board[0, 0].ToString() + board[0, 1].ToString() + board[0, 2].ToString();
            string midRow = board[1, 0].ToString() + board[1, 1].ToString() + board[1, 2].ToString();
            string botRow = board[2, 0].ToString() + board[2, 1].ToString() + board[2, 2].ToString();
            string firstCol = board[0, 0].ToString() + board[1, 0].ToString() + board[2, 0].ToString();
            string secondCol = board[0, 1].ToString() + board[1, 1].ToString() + board[2, 1].ToString();
            string thirdCol = board[0, 2].ToString() + board[1, 2].ToString() + board[2, 2].ToString();
            string diagonal = board[0, 0].ToString() + board[1, 1].ToString() + board[2, 2].ToString();
            string otherDiagonal = board[0, 2].ToString() + board[1, 1].ToString() + board[2, 0].ToString();

            string playerTriple = currentPlayer.ToString() + currentPlayer.ToString() + currentPlayer.ToString();

            if (topRow.Equals(playerTriple)
                || midRow.Equals(playerTriple)
                || botRow.Equals(playerTriple)
                || firstCol.Equals(playerTriple)
                || secondCol.Equals(playerTriple)
                || thirdCol.Equals(playerTriple)
                || diagonal.Equals(playerTriple)
                || otherDiagonal.Equals(playerTriple))
            {
                return true;
            }

            return false;
        }
    }
}
