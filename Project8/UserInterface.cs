using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Ex02.ConsoleUtils;
namespace Project8
{


    internal class UserInterface
    {
        Enums enums = new Enums();
        private Game m_Game = new Game();
        int turnNumber = 0;
        bool isFirstMove = true;
        public void Run()
        {
            bool endConditionReached = false;
            Console.WriteLine("Please enter a user name consisting of maximum 20 letters without spaces:");
            string firstPlayerName = GetValidPlayerName();
            GetBoardSizeFromUser();
            Console.WriteLine("Please enter 1 for singleplayer game (vs computer) or 2 for multiplayer game");
            int userGameChoice = int.Parse(Console.ReadLine());
            while (ValidateGameChoice(userGameChoice))
            {
                Console.WriteLine("Invalid choice,Please enter 1 for singleplayer game (vs computer) or 2 for multiplayer game");
                userGameChoice = int.Parse(Console.ReadLine());
            }
            InitializeGame(userGameChoice, firstPlayerName);

            while (!endConditionReached)
            {
                Player currentPlayer = m_Game.GetCurrentPlayer(turnNumber);
                PrintBoard();
                if (isFirstMove)
                {
                    Console.Write($" " + currentPlayer.PlayerName + "'s turn: ");
                    isFirstMove = false;
                }
                else if (m_Game.GetCurrentPlayer(turnNumber).PlayerType.Equals("Computer"))
                {
                    Console.WriteLine("It is computer's turn (press Enter to see its move):");
                    Console.ReadLine();
                    m_Game.MakeComputerMove();
                    Ex02.ConsoleUtils.Screen.Clear();
                }
                else
                {
                    Console.WriteLine($"It is {0}'s turn ({1}): ", m_Game.GetCurrentPlayer(turnNumber).PlayerName, m_Game.GetCurrentPlayer(turnNumber).PlayerPiece);
                }

                string userMoveString = Console.ReadLine();
                if (userMoveString == "Q")
                {
                    endConditionReached = true;
                }
                //checks if the syntax of the move is valid, if not returns false,
                //and if it is, it checks if the move itself is possible, piece movement wise
                else
                {
                    GetMoveFromUser(userMoveString, currentPlayer);
                    Ex02.ConsoleUtils.Screen.Clear();
                }
                if (turnNumber == 1)
                {
                    turnNumber = 0;
                }
                else
                {
                    turnNumber++;
                }
            }
        }

        private bool IsGameOver()
        {
            return !(isQuitConditionPressed() && IsDrawAchieved() && IsVictoryAchieved());
        }

        private bool IsVictoryAchieved()
        {
            throw new NotImplementedException();
        }

        private bool IsDrawAchieved()
        {
            throw new NotImplementedException();
        }

        private bool isQuitConditionPressed()
        {
            throw new NotImplementedException();
        }

        private void InitializeGame(int userGameChoice, string i_FirstPlayerName)
        {
            if (userGameChoice.Equals((int)Enums.GameTypeChoice.SinglePlayer))
            {
                GetUserChoiceForGameType(Enums.GameTypeChoice.SinglePlayer, i_FirstPlayerName, "Computer");

            }
            else if (userGameChoice.Equals((int)Enums.GameTypeChoice.MultiPlayer))
            {
                Console.WriteLine("Enter a name for the second player: ");
                string secondPlayerName = GetValidPlayerName();
                GetUserChoiceForGameType(Enums.GameTypeChoice.MultiPlayer, i_FirstPlayerName, secondPlayerName);

            }

        }
        private bool ValidateGameChoice(int userGameChoice)
        {
            return userGameChoice != ((int)Enums.GameTypeChoice.SinglePlayer) && userGameChoice != ((int)Enums.GameTypeChoice.MultiPlayer);
        }

        //Method to get board size input from user
        public void GetBoardSizeFromUser()
        {
            bool IsValidBoardSize = true;
            while (IsValidBoardSize)
            {
                Console.WriteLine("Please enter the size of the board, it should be either 6, 8 or 10");
                try
                {
                    m_Game.SetBoardSizeFromUser(Console.ReadLine());
                    IsValidBoardSize = false;
                }
                catch (Exception e)
                {
                    {
                        Console.WriteLine(e.Message);
                    };
                }
            }
        }
        public void PrintBoard()
        {
            int boardSize = m_Game.GetBoardSize();
            // Column identifiers
            char[] columnHeaders = new char[boardSize];
            for (int i = 0; i < boardSize; i++)
            {
                columnHeaders[i] = (char)('a' + i);
            }
            Console.Write("   "); // Padding for the row numbers
            foreach (char header in columnHeaders)
            {
                Console.Write($"  {header}   ");
            }
            Console.WriteLine();

            Console.WriteLine("   " + new string('=', (boardSize * 6)));

            // Rows with row identifiers and board content
            Point point = new Point(0, 0);
            for (int i = 0; i < boardSize; i++)
            {
                // Row identifier
                Console.Write($"{(char)('A' + i)} |");

                for (int j = 0; j < boardSize; j++)
                {
                    point.x = i;
                    point.y = j;
                    Console.Write($" {m_Game.GetPositionValueFromBoard(point)} |");
                }
                Console.WriteLine();

                // Row separator
                Console.WriteLine("   " + new string('=', (boardSize * 6)));
            }
        }


        public void GetUserChoiceForGameType(Enums.GameTypeChoice i_ChoiceMade, string i_FirstPlayerName, string i_SecondPlayerName)
        {
            m_Game.InitializeGame(i_ChoiceMade, i_FirstPlayerName, i_SecondPlayerName);
        }

        private string GetValidPlayerName()
        {
            string playerName = null;
            bool IsValidName = false;

            while (!IsValidName)
            {
                playerName = Console.ReadLine();
                if (m_Game.ValidatePlayerName(playerName))
                {
                    Console.WriteLine("Invalid player name entered. Please enter a name without spaces and with a maximum of 20 characters.");
                }
                else
                {
                    IsValidName = true;
                }
            }
            return playerName;
        }
        public void GetMoveFromUser(string i_MoveDescription, Player i_PlayersMove)
        {
            Point[] pointsMovement = new Point[2];
            while (!CheckIfValidMoveSyntax(i_MoveDescription, out pointsMovement))
            {
                Console.WriteLine("Invalid move syntax. Please enter a move in this format : ROWcol>ROWcol");
                i_MoveDescription = Console.ReadLine();
            }
            if(m_Game.ValidateNewTurn(pointsMovement, i_PlayersMove))
            {
                m_Game.UpdateBoard(pointsMovement);
            }
        }

        // Method to validate the move syntax
        public bool CheckIfValidMoveSyntax(string i_MoveDescription, out Point[] o_PointsToReturn)
        {
            // Initialize the output points array
            o_PointsToReturn = new Point[2];

            if (string.IsNullOrWhiteSpace(i_MoveDescription))
            {
                return false;
            }

            // Split the input string by '>'
            string[] moveParts = i_MoveDescription.Split('>');
            if (moveParts.Length != 2)
            {
                return false; // Invalid move format
            }

            // Validate each part and populate the output points array
            for (int i = 0; i < moveParts.Length; i++)
            {
                string part = moveParts[i];
                if (string.IsNullOrWhiteSpace(part) || part.Length < 2)
                {
                    return false; // Invalid move part
                }

                char rowChar = part[0];
                char colChar = part[1];

                int row = rowChar - 'A';
                int col = char.ToLower(colChar) - 'a';

                o_PointsToReturn[i] = new Point(row, col);
            }

            return true;
        }
        public Point ParseMovePart(string movePart)
        {
            char rowChar = movePart[0];
            char colChar = movePart[1];

            // Convert to zero-based indices
            int row = rowChar - 'A'; // Convert '1'-'9' to 0-based
            int col = char.ToLower(colChar) - 'a'; // Convert 'a'-'h' to 0-based

            return new Point(row, col);
        }
    }
}