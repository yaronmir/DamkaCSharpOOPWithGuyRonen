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

            // Ensure the piece matches the current player
            if (!currentPlayer.OwnsPiece(currentPiece.Trim()))
            {
                throw new Exception("Piece type doesn't fit the player, please select a fitting piece.");
            }

            // Ensure the move is within bounds
            if (currentBoard.CheckIfMoveOutOfBounds(nextPoint) || currentBoard.CheckIfMoveOutOfBounds(currentPoint))
            {
                throw new Exception("Out of bounds move, please try again.");
            }

            // Calculate move differences
            int xDiff = Math.Abs(nextPoint.x - currentPoint.x);
            int yDiff = Math.Abs(nextPoint.y - currentPoint.y);

            // Ensure diagonal movement
            if (!(xDiff == 1 && yDiff == 1) && !(xDiff == 2 && yDiff == 2))
            {
                throw new Exception("Invalid move: must move diagonally by 1 or 2 squares.");
            }

            Enums.TileType tileType = currentBoard.ConvertPositionValueToTileType(currentPiece);

            switch (tileType)
            {
                case Enums.TileType.X:
                    return ValidateMoveForPiece(currentPoint, nextPoint, currentBoard, Enums.TileType.O, Enums.TileType.U, isKing: false, isMovingUp: false);

                case Enums.TileType.O:
                    return ValidateMoveForPiece(currentPoint, nextPoint, currentBoard, Enums.TileType.X, Enums.TileType.K, isKing: false, isMovingUp: true);

                case Enums.TileType.K:
                    return ValidateMoveForPiece(currentPoint, nextPoint, currentBoard, Enums.TileType.O, Enums.TileType.U, isKing: true, isMovingUp: false);

                case Enums.TileType.U:
                    return ValidateMoveForPiece(currentPoint, nextPoint, currentBoard, Enums.TileType.X, Enums.TileType.K, isKing: true, isMovingUp: true);

                default:
                    throw new Exception("Unknown tile type.");
            }
        }

        private bool ValidateMoveForPiece(Point currentPoint, Point nextPoint, Board board, Enums.TileType opponentRegularPieceType, Enums.TileType opponentKingPieceType, bool isKing, bool isMovingUp)
        {
            // Check directional restrictions
            if (!isKing)
            {
                if (!isMovingUp && nextPoint.x > currentPoint.x)
                {
                    throw new Exception("Invalid move: non-king piece cannot move backwards.");
                }
                if (isMovingUp && nextPoint.x < currentPoint.x)
                {
                    throw new Exception("Invalid move: non-king piece cannot move backwards.");
                }
            }

            // Check if the target tile is empty or a valid capture
            if (board.IsTileEmpty(nextPoint) && (!board.IsCaptureMove(currentPoint,nextPoint)))
            {
                return true; // Simple move to an empty tile
            }
            else if (board.IsCaptureMove(currentPoint, nextPoint))
            {
                Point capturedPiece = board.GetTileBetween(currentPoint, nextPoint);
                if (!board.IsTileEmpty(capturedPiece))
                {
                    Enums.TileType capturedPieceType = board.ConvertPositionValueToTileType(board.GetValueAtPosition(capturedPiece));
                    if (capturedPieceType == opponentRegularPieceType || capturedPieceType == opponentKingPieceType)
                    {
                        board.EraseCapturedPiece(capturedPiece);
                        return true; // Valid capture move
                    }
                }
                throw new Exception("Invalid capture move: target must contain an opponent piece.");
            }

            throw new Exception("Invalid move: target tile is not empty and no valid capture.");
        }

      /*  public void ScanBoardForMoves(Board board, Player currentPlayer)
        {
            currentPlayer.ClearPossibleMoves(); // Clear previous moves
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)
                {
                    Point currentPoint = new Point(row, col);
                    string tileValue = board.GetValueAtPosition(currentPoint);

                    // Check if the tile belongs to the current player
                    if (currentPlayer.OwnsPiece(tileValue))
                    {
                        Enums.TileType tileType = board.ConvertPositionValueToTileType(tileValue);

                        // Check all possible diagonal moves (1 or 2 squares)
                        int[] rowOffsets = { -1, 1, -2, 2 };
                        int[] colOffsets = { -1, 1, -2, 2 };

                        for (int i = 0; i < rowOffsets.Length; i++)
                        {
                            Point nextPoint = new Point(row + rowOffsets[i], col + colOffsets[i]);
                            try
                            {
                                if (CheckIfMoveIsValid(currentPoint, nextPoint, board, currentPlayer))
                                {
                                    currentPlayer.AddPossibleMove(nextPoint);
                                }
                            }
                            catch (Exception)
                            {
                                // Ignore invalid moves
                            }
                        }
                    }
                }
            }
        }
    */}
}