using System.Text;
using MatchJoy.Board;
using UnityEngine;

namespace MatchJoy.Debugging
{
    public static class BoardDebugPrinter
    {
        public static string BuildBoardString(BoardState board)
        {
            var builder = new StringBuilder();
            for (var y = board.Height - 1; y >= 0; y--)
            {
                for (var x = 0; x < board.Width; x++)
                {
                    var coordinate = new BoardCoordinate(x, y);
                    if (!board.TryGetCell(coordinate, out var cell) || !cell.IsPlayable)
                    {
                        builder.Append(" XX");
                        continue;
                    }

                    builder.Append(cell.IsOccupied ? $" {cell.TileId:D2}" : " ..");
                }

                builder.AppendLine();
            }

            return builder.ToString();
        }

        public static void LogBoard(BoardState board, Object context, string label)
        {
            Debug.Log($"{label}\n{BuildBoardString(board)}", context);
        }
    }
}
