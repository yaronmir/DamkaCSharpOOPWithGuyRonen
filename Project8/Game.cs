using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project8
{
    public class Game
    {
        private Board m_Board = new Board();
        private Player[] m_PlayersArray = new Player[2];
        private Turn m_TurnForPlayer = new Turn();

        //Method to get board size input from user
        public bool CheckIfDraw()
        {
            return !m_PlayersArray[0].GetPossibleMoves().Any() && !m_PlayersArray[1].GetPossibleMoves().Any();
        }

        public int GetIndexOfWinningPlayer()
        {
            Dictionary<string, int> playerPieces = m_Board.CountPiecesOnBoard();

            if ((m_PlayersArray[0].GetPossibleMoves().Any() && !m_PlayersArray[1].GetPossibleMoves().Any()) ||
                (playerPieces[" O "] == 0 && playerPieces[" U "] == 0))
            {
                return 0; 
            }
            else if ((!m_PlayersArray[0].GetPossibleMoves().Any() && m_PlayersArray[1].GetPossibleMoves().Any()) || 
                     (playerPieces[" K "] == 0 && playerPieces[" X "] == 0))
            {
                return 1;
            }
            return -1; 
        }

        public int CalculateWinnersScore()
        {
            Dictionary<string, int> playerPieces = m_Board.CountPiecesOnBoard();

            int xPlayerPiecesCounter = 0;
            int oPlayerPiecesCounter = 0;

            xPlayerPiecesCounter += playerPieces[" X "] * 1;  
            xPlayerPiecesCounter += playerPieces[" K "] * 4;  

            oPlayerPiecesCounter += playerPieces[" O "] * 1;  
            oPlayerPiecesCounter += playerPieces[" U "] * 4;

            m_PlayersArray[0].PlayerScore = xPlayerPiecesCounter;
            m_PlayersArray[1].PlayerScore = oPlayerPiecesCounter;

            return Math.Abs(xPlayerPiecesCounter - oPlayerPiecesCounter); 
        }

        public void SetBoardSizeFromUser(string i_BoardSize)
        {
            bool IsValidBoardSize = false;
            while (!IsValidBoardSize)
            {
                m_Board.BoardSize = int.Parse(i_BoardSize);
                IsValidBoardSize = true;
            }
        }

        public bool ValidateNewTurn(Point[] i_PointsOfMove, Player i_CurrentPlayerTurn)
        {
            return m_TurnForPlayer.CheckIfMoveIsValid(i_PointsOfMove[0], i_PointsOfMove[1], m_Board, i_CurrentPlayerTurn);
        }

        internal bool ValidatePlayerName(string playerName)
        {
            return playerName.Length > 20 || playerName.Length == 0 || playerName.Contains(" ");
        }

        public void InitializeGame(Enums.GameTypeChoice gameType, string i_FirstPlayerName, string i_SecondPlayerName)
        {
            m_PlayersArray[0] = new Player(i_FirstPlayerName, "Player", " X ", 0, " K ");
            m_PlayersArray[1] = new Player(i_SecondPlayerName,
                                           gameType == Enums.GameTypeChoice.SinglePlayer ? "Computer" : "Player", " O ", 0, " U ");
        }
        public int GetBoardSize()
        {
            return m_Board.BoardSize;
        }

        public string GetPositionValueFromBoard(Point i_PointToSearch)
        {
            return m_Board.GetValueAtPosition(i_PointToSearch);
        }

        public Player GetCurrentPlayer(int i_PlayerNumber)
        {
            return m_PlayersArray[i_PlayerNumber];
        }

        public void UpdateBoard(Point[] pointsMovement)
        {
            m_Board.UpdateBoard(pointsMovement[0], pointsMovement[1]);
        }

        public string MakeComputerMove(Player i_ComputerPlayer)
        {

            Point[] PointsToMoveBy = GetRandomMoveForComputer(i_ComputerPlayer.GetPossibleMoves());

            m_Board.ExecuteMove(PointsToMoveBy, i_ComputerPlayer);

            Point startPoint = PointsToMoveBy[0];
            Point endPoint = PointsToMoveBy[1];
            string start = $"{(char)('A' + startPoint.x)}{(char)('a' + startPoint.y)}";
            string end = $"{(char)('A' + endPoint.x)}{(char)('a' + endPoint.y)}";

            return $"{start}>{end}";
        }

        public Board GetCurrentBoard()
        {
            return m_Board;
        }

        public Point[] GetRandomMoveForComputer(List<Point[]> i_PointsToRandomFrom)
        {
            Random randomCounter = new Random();
            
            if (i_PointsToRandomFrom == null || i_PointsToRandomFrom.Count == 0)
            {
                throw new InvalidOperationException("No moves available to choose from.");
            }
            int randomIndex = randomCounter.Next(i_PointsToRandomFrom.Count);
            return i_PointsToRandomFrom[randomIndex];
        }

        public void CheckingPossibleValidMoves(Board i_CurrentBoard, Player i_CurrentPlayer)
        {
            m_TurnForPlayer.GeneratePossibleMoves(i_CurrentBoard, i_CurrentPlayer);
        }

    }
}