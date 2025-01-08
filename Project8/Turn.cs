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
            bool isPieceOnPromotionTile = board.IsPromotionTile(nextPoint);
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
                if (isPieceOnPromotionTile)
                {
                    string valueAtPosition = board.GetValueAtPosition(currentPoint);
                    board.NewestBoardMatrix[currentPoint.x, currentPoint.y] = board.DecideWhichKingForCurrentPiece(valueAtPosition);
                }
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

        public bool IsOptimalMove(Point currentPoint, Point nextPoint, Board currentBoard, Player currentPlayer)
        {
            bool isOptimalMove = true;
            // Check if the current move is a capture move
            bool isCurrentMoveCapture = currentBoard.IsCaptureMove(currentPoint, nextPoint);
            if (isCurrentMoveCapture)
            {
                return isOptimalMove;
            }
            // If the current move is not a capture move, check if any capture moves are available
            if (!isCurrentMoveCapture)
            {
                for (int row = 0; row < currentBoard.BoardSize; row++)
                {
                    for (int col = 0; col < currentBoard.BoardSize; col++)
                    {
                        Point currentPiecePoint = new Point(row, col);
                        string tileValue = currentBoard.GetValueAtPosition(currentPiecePoint);

                        // Check if the tile belongs to the current player
                        if (currentPlayer.OwnsPiece(tileValue))
                        {
                            // Check all possible diagonal moves (2 squares, as 1-square moves are not captures)
                            int[] rowOffsets = { -2, 2 };
                            int[] colOffsets = { -2, 2 };
                            if (currentBoard.IsKing(tileValue))
                            {
                                foreach (int rowOffset in rowOffsets)
                                {
                                    foreach (int colOffset in colOffsets)
                                    {
                                        Point potentialCapturePoint = new Point(row + rowOffset, col + colOffset);
                                        isOptimalMove = ValidatePossibleOffsetCapture(currentPiecePoint, potentialCapturePoint, currentBoard, currentPlayer);
                                    }
                                }
                            }
                            else
                            {
                                if(currentBoard.GetValueAtPosition(currentPiecePoint) == " X ")
                                {
                                    foreach(int colOffset in colOffsets)
                                    {
                                        Point potentialCapturePoint = new Point(row + rowOffsets[0], col + colOffset);
                                        isOptimalMove = ValidatePossibleOffsetCapture(currentPiecePoint, potentialCapturePoint, currentBoard, currentPlayer);
                                    }
                                }
                                else
                                {
                                    foreach (int colOffset in colOffsets)
                                    {
                                        Point potentialCapturePoint = new Point(row + rowOffsets[1], col + colOffset);
                                        isOptimalMove = ValidatePossibleOffsetCapture(currentPiecePoint, potentialCapturePoint, currentBoard, currentPlayer);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return isOptimalMove;
        }

        public bool ValidatePossibleOffsetCapture(Point i_CurrentPoint, Point i_PossibleCapturePoint, Board i_CurrentBoard, Player i_CurrentPlayer)
        {
            if (!i_CurrentBoard.CheckIfMoveOutOfBounds(i_PossibleCapturePoint))
            {
                if (!i_CurrentBoard.IsTileEmpty(i_CurrentBoard.GetTileBetween(i_CurrentPoint, i_PossibleCapturePoint))
                    && !i_CurrentPlayer.OwnsPiece(i_CurrentBoard.GetValueAtPosition(i_CurrentBoard.GetTileBetween(i_CurrentPoint, i_PossibleCapturePoint))))
                {
                    // If a capture move exists, the current move is not optimal
                    return false;
                }
            }

            // If no better moves exist, the move is optimal
            return true;
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
      */
    }
}