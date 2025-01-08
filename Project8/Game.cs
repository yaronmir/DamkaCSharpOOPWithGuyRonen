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
        Player[] m_PlayersArray = new Player[2];

        //Method to get board size input from user
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
            Turn turn = new Turn();
            return turn.CheckIfMoveIsValid(i_PointsOfMove[0], i_PointsOfMove[1], m_Board, i_CurrentPlayerTurn);
        }

        internal bool ValidatePlayerName(string playerName)
        {
            return playerName.Length > 20 || playerName.Length == 0 || playerName.Contains(" ");
        }

        public void InitializeGame(Enums.GameTypeChoice gameType, string i_FirstPlayerName, string i_SecondPlayerName)
        {
            m_PlayersArray[0] = new Player(i_FirstPlayerName, "player", "X", 0, "K");
            m_PlayersArray[1] = new Player(i_SecondPlayerName,
                                           gameType == Enums.GameTypeChoice.SinglePlayer ? "Computer" : "player", "O", 0, "U");
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

        internal void MakeComputerMove()
        {
            throw new NotImplementedException();
        }
    }
}