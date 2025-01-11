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
        bool isValidGameType = false;
        bool wasQPressed = false;
        string lastMoveMade;
        string userCurrentMove;
        Player lastPlayerThatPlayed;
        public void Run()
        {
            Console.WriteLine("Please enter a user name consisting of maximum 20 letters without spaces:");
            string firstPlayerName = GetValidPlayerName();
            GetBoardSizeFromUser();
            Console.WriteLine("Please enter 1 for singleplayer game (vs computer) or 2 for multiplayer game");
            int userGameChoice;
            while (!isValidGameType)
            {
                string input = Console.ReadLine();
                if (int.TryParse(input, out userGameChoice) && !ValidateGameChoice(userGameChoice))
                {
                    isValidGameType = true;
                    InitializeGame(userGameChoice, firstPlayerName);
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please enter 1 for singleplayer game (vs computer) or 2 for multiplayer game.");
                }
            }

            while (!IsGameOver())
            {   
                Player currentPlayer = m_Game.GetCurrentPlayer(turnNumber);
                PrintBoard();
                if (!isFirstMove)
                {
                    Console.WriteLine($"{lastPlayerThatPlayed.PlayerName}'s move was ({lastPlayerThatPlayed.PlayerPiece}): {lastMoveMade}");
                }
                if (isFirstMove)
                {
                    Console.Write($" " + currentPlayer.PlayerName + "'s turn: ");
                    userCurrentMove = MakeMoveForPlayer(currentPlayer);
                    if (isQuitConditionPressed(userCurrentMove))
                    {
                        Console.WriteLine($"{m_Game.GetCurrentPlayer(turnNumber).PlayerName} wins by forfeit with the score of {m_Game.CalculateWinnersScore()}");
                        break;
                    }
                    lastMoveMade = userCurrentMove;
                    isFirstMove = false;
                }
                else if (m_Game.GetCurrentPlayer(turnNumber).PlayerType.Equals("Computer"))
                {
                    Console.Write("It is computer's turn (press Enter to see its move):");
                    Console.ReadLine();
                    try
                    {
                        m_Game.CheckingPossibleValidMoves(m_Game.GetCurrentBoard(), m_Game.GetCurrentPlayer(turnNumber));
                        lastMoveMade = m_Game.MakeComputerMove(m_Game.GetCurrentPlayer(turnNumber));
                        Ex02.ConsoleUtils.Screen.Clear();
                    }
                    catch (Exception e)
                    {
                        { Console.WriteLine(e); }
                    }
                }
                else if (!isFirstMove && m_Game.GetCurrentPlayer(turnNumber).PlayerType.Equals("Player"))
                {
                    Console.WriteLine($"It is {currentPlayer.PlayerName}'s turn ({currentPlayer.PlayerPiece}): ");
                    userCurrentMove = MakeMoveForPlayer(currentPlayer);
                    if (isQuitConditionPressed(userCurrentMove))
                    {
                        Console.WriteLine($"{lastPlayerThatPlayed.PlayerName} wins by forfeit with the score of {m_Game.CalculateWinnersScore()}");
                        break;
                    }
                    lastMoveMade = userCurrentMove;
                }

                if (turnNumber == 1)
                {
                        lastPlayerThatPlayed = m_Game.GetCurrentPlayer(turnNumber);
                        turnNumber = 0;
                }
                else
                {
                    lastPlayerThatPlayed = m_Game.GetCurrentPlayer(turnNumber);
                    turnNumber++;
                }
            }

            Console.WriteLine("Would you like to keep playing? Enter Y if yes and N if not");
            CheckIfPlayAgain(Console.ReadLine());
            

        }

        private void CheckIfPlayAgain(string i_userInput)
        {
        }

        private string MakeMoveForPlayer(Player i_PlayerToPlay)
        {
            Point[] movementPoints = new Point[2];
            string userMoveString = Console.ReadLine();

            //checks if the syntax of the move is valid, if not returns false,
            //and if it is, it checks if the move itself is possible, piece movement wise
            bool isTheInputAGoodMove = false;
            while (!isTheInputAGoodMove)
            {
                if (userMoveString == "Q")
                {
                    return "Q";
                }
                if (!ValidateSyntaxOfMove(userMoveString, i_PlayerToPlay, out movementPoints))
                {
                    Console.WriteLine("Invalid move syntax, please write the move in this format: ROWcol>ROWcol. ");
                    userMoveString = Console.ReadLine();
                }
                else
                {
                    if (!CheckIfMoveIsValid(movementPoints, i_PlayerToPlay))
                    {
                        Console.WriteLine("Invalid move, please enter a new move again!");
                        userMoveString = Console.ReadLine();
                    }
                    else
                    {
                        m_Game.UpdateBoard(movementPoints);
                        Ex02.ConsoleUtils.Screen.Clear();
                        isTheInputAGoodMove = true;
                    }
                }
            }
            return userMoveString;
        }
        

        private bool CheckIfMoveIsValid(Point[] i_PointsArrayOfMove, Player i_CurrentPlayerPlaying )
        {
            return m_Game.ValidateNewTurn(i_PointsArrayOfMove, i_CurrentPlayerPlaying);
        }

        private bool IsGameOver()
        {
            return !(IsDrawAchieved() || IsVictoryAchieved());
        }

        private bool IsVictoryAchieved()
        {
            return m_Game.GetIndexOfWinningPlayer() >= 0;
        }

        private bool IsDrawAchieved()
        {
            return m_Game.CheckIfDraw();
        }

        private bool isQuitConditionPressed(string i_UserInput)
        {
            return i_UserInput == "Q";
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
        public bool ValidateSyntaxOfMove(string i_MoveDescription, Player i_PlayersMove, out Point[] o_PointsToMoveBy)
        {
            return CheckIfValidMoveSyntax(i_MoveDescription, out o_PointsToMoveBy);
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