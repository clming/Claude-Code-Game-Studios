using System.Collections.Generic;

namespace MatchJoy.Board
{
    public sealed class CascadeResolver
    {
        public int Resolve(BoardState board, IReadOnlyList<MatchGroup> matchGroups)
        {
            var uniqueCoordinates = new HashSet<(int X, int Y)>();
            foreach (var group in matchGroups)
            {
                foreach (var coordinate in group.Coordinates)
                {
                    uniqueCoordinates.Add((coordinate.X, coordinate.Y));
                }
            }

            foreach (var coordinate in uniqueCoordinates)
            {
                if (board.TryGetCell(new BoardCoordinate(coordinate.X, coordinate.Y), out var cell))
                {
                    cell.SetEmpty();
                }
            }

            ApplyGravity(board);
            Refill(board);
            return uniqueCoordinates.Count;
        }

        private static void ApplyGravity(BoardState board)
        {
            for (var x = 0; x < board.Width; x++)
            {
                var occupiedTiles = new Queue<int>();
                for (var y = 0; y < board.Height; y++)
                {
                    var coordinate = new BoardCoordinate(x, y);
                    if (board.TryGetCell(coordinate, out var cell) && cell.IsOccupied)
                    {
                        occupiedTiles.Enqueue(cell.TileId);
                    }
                }

                for (var y = 0; y < board.Height; y++)
                {
                    var coordinate = new BoardCoordinate(x, y);
                    if (!board.TryGetCell(coordinate, out var cell))
                    {
                        continue;
                    }

                    if (occupiedTiles.Count > 0)
                    {
                        cell.SetTile(occupiedTiles.Dequeue());
                    }
                    else
                    {
                        cell.SetEmpty();
                    }
                }
            }
        }

        private static void Refill(BoardState board)
        {
            for (var y = 0; y < board.Height; y++)
            {
                for (var x = 0; x < board.Width; x++)
                {
                    var coordinate = new BoardCoordinate(x, y);
                    if (board.TryGetCell(coordinate, out var cell) && cell.IsPlayable && !cell.IsOccupied)
                    {
                        cell.SetTile((x + y) % 5);
                    }
                }
            }
        }
    }
}
