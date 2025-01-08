using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project8
{
    public class Player
    {
        List<Point> m_PossibleMovesListForComputer = new List<Point>();
        private readonly string m_PlayerName;
        private readonly string m_PlayerType;
        private readonly string m_PlayerPiece;
        private readonly string m_KingPiece;   
        private int m_PlayerScore;

        public Player() { }
        // Constructor for creating a player
        public Player(string i_PlayerName, string i_PlayerType, string i_PieceType, int i_PlayerScore, string i_KingType)
        {
            m_PlayerName = i_PlayerName;
            m_PlayerType = i_PlayerType;
            m_PlayerPiece = i_PieceType;
            m_PlayerScore = i_PlayerScore;
            m_KingPiece = i_KingType; 
        }

        public string PlayerType
        {
            get => m_PlayerType;
        }

        public string PlayerName
        {
            get => m_PlayerName;
        }

        public string PlayerPiece
        {
            get => m_PlayerPiece;
        }

        public int PlayerScore
        {
            get => m_PlayerScore;
            set => m_PlayerScore = value;
        }
        public string KingPiece
        {
            get => m_KingPiece;
        }
        public void AddPossibleMove(Point move)
        {
            m_PossibleMovesListForComputer.Add(move);
        }

        public List<Point> GetPossibleMoves()
        {
            return m_PossibleMovesListForComputer;
        }

        public bool OwnsPiece(string piece)
        {
            return piece.Equals(m_PlayerPiece) || piece.Equals(m_KingPiece);
        }

        internal void ClearPossibleMoves()
        {
            m_PossibleMovesListForComputer.Clear();
        }
    }
}