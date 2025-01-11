using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project8
{
    public class Turn
    {
        public bool CheckIfMoveIsValid(Point currentPoint, Point nextPoint, Board currentBoard, Player currentPlayer)
        {
            string currentPiece = currentBoard.GetValueAtPosition(currentPoint);
            bool isMoveValid;
            if (currentBoard.ConvertPositionValueToTileType(currentPiece).Equals(Enums.TileType.Empty))
            {
                return false;
            }
            if (!currentPlayer.OwnsPiece(currentPiece))
            {
                return false;
            }

            if (currentBoard.CheckIfMoveOutOfBounds(nextPoint) || currentBoard.CheckIfMoveOutOfBounds(currentPoint))
            {
                return false;
            }

            int xDiff = Math.Abs(nextPoint.x - currentPoint.x);
            int yDiff = Math.Abs(nextPoint.y - currentPoint.y);

            if (!(xDiff == 1 && yDiff == 1) && !(xDiff == 2 && yDiff == 2))
            {
                return false;
            }

            Enums.TileType tileType = currentBoard.ConvertPositionValueToTileType(currentPiece);

            switch (tileType)
            {
                case Enums.TileType.X:
                    isMoveValid = ValidateMoveForPiece(currentPlayer, currentPoint, nextPoint, currentBoard, Enums.TileType.O, Enums.TileType.U, isKing: false, isMovingUp: false);
                    break;
                case Enums.TileType.O:
                    isMoveValid =  ValidateMoveForPiece(currentPlayer, currentPoint, nextPoint, currentBoard, Enums.TileType.X, Enums.TileType.K, isKing: false, isMovingUp: true);
                    break;
                case Enums.TileType.K:
                    isMoveValid = ValidateMoveForPiece(currentPlayer, currentPoint, nextPoint, currentBoard, Enums.TileType.O, Enums.TileType.U, isKing: true, isMovingUp: false);
                    break;
                case Enums.TileType.U:
                    isMoveValid = ValidateMoveForPiece(currentPlayer, currentPoint, nextPoint, currentBoard, Enums.TileType.X, Enums.TileType.K, isKing: true, isMovingUp: true);
                    break;
                default:
                    isMoveValid = false;
                    break;
            }
            if (isMoveValid && !IsOptimalMove(currentPoint, nextPoint, currentBoard, currentPlayer))
            {
                return !isMoveValid;
            }

            return isMoveValid;
        }

        private bool ValidateMoveForPiece(Player i_CurrentPlayer, Point currentPoint, Point nextPoint, Board i_Board, Enums.TileType opponentRegularPieceType, Enums.TileType opponentKingPieceType, bool isKing, bool isMovingUp)
        {

            bool isPieceOnPromotionTile = i_Board.IsPromotionTile(nextPoint);
            if (!isKing)
            {
                if (!isMovingUp && nextPoint.x > currentPoint.x)
                {
                   return false;
                }
                if (isMovingUp && nextPoint.x < currentPoint.x)
                {
                   return false;
                }
            }

            if (i_Board.IsTileEmpty(nextPoint) && (!i_Board.IsCaptureMove(currentPoint,nextPoint)))
            {
                if (isPieceOnPromotionTile)
                {
                    string valueAtPosition = i_Board.GetValueAtPosition(currentPoint);
                    i_Board.NewestBoardMatrix[currentPoint.x, currentPoint.y] = i_Board.DecideWhichKingForCurrentPiece(valueAtPosition);
                }
                return true;
            }
            else if (i_Board.IsCaptureMove(currentPoint, nextPoint))
            {
                Point capturedPiece = i_Board.GetTileBetween(currentPoint, nextPoint);
                if (!i_Board.IsTileEmpty(capturedPiece))
                {
                    Enums.TileType capturedPieceType = i_Board.ConvertPositionValueToTileType(i_Board.GetValueAtPosition(capturedPiece));
                    if ((capturedPieceType == opponentRegularPieceType || capturedPieceType == opponentKingPieceType)
                        && i_Board.ConvertPositionValueToTileType(i_Board.GetValueAtPosition(nextPoint)).Equals(Enums.TileType.Empty))
                    {
                        string tileValue = i_Board.GetValueAtPosition(currentPoint);
                        if (CheckPossibleCaptureMoves(nextPoint, tileValue, i_Board, i_CurrentPlayer))
                        {
                           
                        }

                        if (isPieceOnPromotionTile)
                        {
                            string valueAtPosition = i_Board.GetValueAtPosition(currentPoint);
                            i_Board.NewestBoardMatrix[currentPoint.x, currentPoint.y] = i_Board.DecideWhichKingForCurrentPiece(valueAtPosition);
                        }
                        return true; 
                    }
                }
                return false;
            }

            return false;
        }

        public bool IsOptimalMove(Point currentPoint, Point nextPoint, Board currentBoard, Player currentPlayer)
        {
            if (currentBoard.IsCaptureMove(currentPoint, nextPoint))
            {
                return true;
            }
            for (int row = 0; row < currentBoard.BoardSize; row++)
            {
                for (int col = 0; col < currentBoard.BoardSize; col++)
                {
                    Point currentPiecePoint = new Point(row, col);
                    string tileValue = currentBoard.GetValueAtPosition(currentPiecePoint);
                    if (currentPlayer.OwnsPiece(tileValue))
                    {
                        if (CheckPossibleCaptureMoves(currentPiecePoint, tileValue, currentBoard, currentPlayer))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private bool CheckPossibleCaptureMoves(Point piecePoint, string tileValue, Board currentBoard, Player currentPlayer)
        {
            int[] rowOffsets = { -2, 2 };
            int[] colOffsets = { -2, 2 };

            if (currentBoard.IsKing(tileValue))
            {
                foreach (int rowOffset in rowOffsets)
                {
                    foreach (int colOffset in colOffsets)
                    {
                        Point potentialCapturePoint = new Point(piecePoint.x + rowOffset, piecePoint.y + colOffset);
                        if (ValidatePossibleOffsetCapture(piecePoint, potentialCapturePoint, currentBoard, currentPlayer))
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                int direction = tileValue == " X " ? -2 : 2;
                foreach (int colOffset in colOffsets)
                {
                    Point potentialCapturePoint = new Point(piecePoint.x + direction, piecePoint.y + colOffset);
                    if (ValidatePossibleOffsetCapture(piecePoint, potentialCapturePoint, currentBoard, currentPlayer))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool ValidatePossibleOffsetCapture(Point currentPoint, Point possibleCapturePoint, Board currentBoard, Player currentPlayer)
        {
            // Check bounds
            if (!currentBoard.CheckIfMoveOutOfBounds(possibleCapturePoint))
            {
                Point tileBetween = currentBoard.GetTileBetween(currentPoint, possibleCapturePoint);
                if (!currentBoard.IsTileEmpty(tileBetween) &&
                    !currentPlayer.OwnsPiece(currentBoard.GetValueAtPosition(tileBetween)) &&
                    currentBoard.IsTileEmpty(possibleCapturePoint))
                {
                    return true; 
                }
            }

            return false; 
        }
        public void GeneratePossibleMoves(Board currentBoard, Player currentPlayer)
        {
            currentPlayer.ClearPossibleMoves();
            for (int row = 0; row < currentBoard.BoardSize; row++)
            {
                for (int col = 0; col < currentBoard.BoardSize; col++)
                {
                    Point currentPoint = new Point(row, col);
                    string tileValue = currentBoard.GetValueAtPosition(currentPoint);

                    if (currentPlayer.OwnsPiece(tileValue))
                    {
                        int[] rowOffsets = { -1, 1, -2, 2 };
                        int[] colOffsets = { -1, 1, -2, 2 };

                        foreach (int rowOffset in rowOffsets)
                        {
                            foreach (int colOffset in colOffsets)
                            {
                                Point nextPoint = new Point(currentPoint.x + rowOffset, currentPoint.y + colOffset);
                                if (IsMoveValidForComputer(currentPoint, nextPoint, currentBoard, currentPlayer))
                                {
                                    currentPlayer.AddPossibleMove(currentPoint, nextPoint);
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool IsMoveValidForComputer(Point currentPoint, Point nextPoint, Board currentBoard, Player currentPlayer)
        {
             return CheckIfMoveIsValid(currentPoint, nextPoint, currentBoard, currentPlayer);
        }
    }
}