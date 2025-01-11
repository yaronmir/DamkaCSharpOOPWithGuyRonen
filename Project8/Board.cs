using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project8;



namespace Project8
{
    public class Board
    {
        private int m_boardSize;
        private string[,] m_NewestBoardMatrix;

        public string[,] NewestBoardMatrix
        {
            get
            {
                return m_NewestBoardMatrix;
            }
            set
            {
                m_NewestBoardMatrix = value;
            }
        }

        // Property to get and set the board size
        public int BoardSize //  TODO ask guy about naming convention
        {
            get => m_boardSize;
            set
            {

                if (value != 6 && value != 8 && value != 10)
                {
                    throw new ArgumentException("Board size you input isn't valid. " +
                                                "Please enter a valid board size: 6, 8, or 10.");
                }
                m_boardSize = value;
                InitializeBoardMatrix();
            }
        }

        // Method to initialize the board matrix
        private void InitializeBoardMatrix()
        {
            m_NewestBoardMatrix = new string[m_boardSize, m_boardSize];

            // Initialize top and bottom pieces
            for (int i = 0; i < m_boardSize; i++)
            {
                for (int j = 0; j < m_boardSize; j++)
                {
                    if (i < m_boardSize / 2 - 1)
                    {
                        m_NewestBoardMatrix[i, j] = (i + j) % 2 == 1 ? " O " : "   ";
                    }
                    else if (i > m_boardSize / 2)
                    {
                        m_NewestBoardMatrix[i, j] = (i + j) % 2 == 1 ? " X " : "   ";
                    }
                    else
                    {
                        m_NewestBoardMatrix[i, j] = "   ";
                    }
                }
            }
        }
        public void UpdateBoard(Point i_StartPoint, Point i_EndPoint)
        {
            m_NewestBoardMatrix[i_EndPoint.x,i_EndPoint.y] = GetValueAtPosition(i_StartPoint);
            m_NewestBoardMatrix[i_StartPoint.x,i_StartPoint.y] = "   ";
            if (IsCaptureMove(i_StartPoint, i_EndPoint))
            {
                EraseCapturedPiece(GetTileBetween(i_StartPoint,i_EndPoint));
            }

        }


        public string GetValueAtPosition(Point i_currentPoint)
        {
            if (CheckIfMoveOutOfBounds(i_currentPoint))
                throw new IndexOutOfRangeException("Row or column is out of bounds.");
            return m_NewestBoardMatrix[i_currentPoint.x, i_currentPoint.y];
        }

        public Enums.TileType ConvertPositionValueToTileType(string i_ValueOfTile)
        {
            if (i_ValueOfTile == " X ") return Enums.TileType.X;
            if (i_ValueOfTile == " O ") return Enums.TileType.O;
            if (i_ValueOfTile == " U ") return Enums.TileType.U;
            if (i_ValueOfTile == " K ") return Enums.TileType.K;
            return Enums.TileType.Empty;
        }


        public bool IsTileEmpty(Point i_point)
        {
            return ConvertPositionValueToTileType(GetValueAtPosition(i_point)) == Enums.TileType.Empty;
        }

        public Point GetDiagonalTile(Point i_currentPoint, Point i_nextPoint)
        {
            return new Point(Math.Abs(i_currentPoint.x - i_nextPoint.x),Math.Abs(i_currentPoint.y -i_nextPoint.y));
        }

        public bool IsCaptureMove(Point i_currentPoint, Point i_nextPoint)
        {
            int xDiff = Math.Abs(i_nextPoint.x - i_currentPoint.x);
            int yDiff = Math.Abs(i_nextPoint.y - i_currentPoint.y);

            return xDiff == 2 && yDiff == 2;
        }
        
        public Point GetTileBetween(Point i_currentPoint, Point i_nextPoint)
        {
            int midX = (i_currentPoint.x + i_nextPoint.x) / 2;
            int midY = (i_currentPoint.y + i_nextPoint.y) / 2;
            return new Point(midX, midY);
        }
        public bool CheckIfMoveOutOfBounds(Point i_pointToCheck)
        {
            return i_pointToCheck.x < 0 || i_pointToCheck.x >= m_boardSize || i_pointToCheck.y < 0 || i_pointToCheck.y >= m_boardSize;
        }

        internal void EraseCapturedPiece(Point capturedPiece)
        {
            m_NewestBoardMatrix[capturedPiece.x, capturedPiece.y] = "   ";
        }

        public bool IsPromotionTile(Point nextPoint)
        {
            return nextPoint.x == m_boardSize - 1 || nextPoint.x == 0;
        }

        public string UpdateBoardWithKingPiece(Point i_PointToPromoteToKing)
        {
            return GetValueAtPosition(i_PointToPromoteToKing);    
        }

        public string DecideWhichKingForCurrentPiece(string i_ValueAtPosition)
        {
            return i_ValueAtPosition == " X " ? " K " : " U ";
        }

        internal bool IsKing(string i_PieceTypeToCheck)
        {
            return i_PieceTypeToCheck == " K " || i_PieceTypeToCheck == " U ";
        }


        public void ExecuteMove(Point[] movePoints, Player currentPlayer)
        {

            Point startPoint = movePoints[0];
            Point endPoint = movePoints[1];


            // Validate move type
            if (IsCaptureMove(startPoint, endPoint))
            {
                // Handle capture move
                Point capturedPoint = GetTileBetween(startPoint, endPoint);

                UpdateBoard(startPoint, endPoint);

                // Check for promotion if the move lands on a promotion tile
                if (IsPromotionTile(endPoint))
                {
                    string currentPieceValue = GetValueAtPosition(endPoint);
                    NewestBoardMatrix[endPoint.x, endPoint.y] = DecideWhichKingForCurrentPiece(currentPieceValue);
                }
            }
            else
            {
                if (IsTileEmpty(endPoint))
                {
                    UpdateBoard(startPoint, endPoint);
                    // Check for promotion if the move lands on a promotion tile
                    if (IsPromotionTile(endPoint))
                    {
                        string currentPieceValue = GetValueAtPosition(endPoint);
                        NewestBoardMatrix[endPoint.x, endPoint.y] = DecideWhichKingForCurrentPiece(currentPieceValue);
                    }
                }
            }
        }

        public Dictionary<string, int> CountPiecesOnBoard()
        {
            Dictionary<string, int> pieceCounts = new Dictionary<string, int>
             {
                 { " X ", 0 }, 
                 { " O ", 0 },
                 { " K ", 0 }, 
                 { " U ", 0 }  
            };
            for (int i = 0; i < m_boardSize; i++)
            {
                for (int j = 0; j < m_boardSize; j++)
                {
                    string piece = m_NewestBoardMatrix[i, j];
                    if (pieceCounts.ContainsKey(piece))
                    {
                        pieceCounts[piece]++;
                    }
                }
            }
            return pieceCounts;
        }
    }
}
