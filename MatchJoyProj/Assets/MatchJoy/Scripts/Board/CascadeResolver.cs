using System.Collections.Generic;
using MatchJoy.Goals;

namespace MatchJoy.Board
{
    public sealed class CascadeResolver
    {
        public ClearResolutionSummary Resolve(BoardState board, IReadOnlyList<MatchGroup> matchGroups)
        {
            var uniqueCoordinates = new HashSet<(int X, int Y)>();
            var clearedTileCounts = new Dictionary<int, int>();

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
                    if (cell.IsOccupied)
                    {
                        if (!clearedTileCounts.ContainsKey(cell.TileId))
                        {
                            clearedTileCounts[cell.TileId] = 0;
                        }

                        clearedTileCounts[cell.TileId]++;
                    }

                    cell.SetEmpty();
                }
            }

            ApplyGravity(board);
            Refill(board);
            return new ClearResolutionSummary(uniqueCoordinates.Count, clearedTileCounts);
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
