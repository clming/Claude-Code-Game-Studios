using System.Collections.Generic;
using MatchJoy.Input;

namespace MatchJoy.Board
{
    public sealed class SwapResolutionService
    {
        public SwapResolutionResult Resolve(BoardState board, SwapRequest request)
        {
            if (!board.CanSwap(request.Source, request.Target))
            {
                return SwapResolutionResult.Rejected(request);
            }

            board.SwapTiles(request.Source, request.Target);
            var matches = FindMatches(board);
            if (matches.Count == 0)
            {
                board.SwapTiles(request.Source, request.Target);
                return SwapResolutionResult.Rejected(request);
            }

            return SwapResolutionResult.Accepted(request, matches);
        }

        private static List<MatchGroup> FindMatches(BoardState board)
        {
            var groups = new List<MatchGroup>();

            for (var y = 0; y < board.Height; y++)
            {
                var run = new List<BoardCoordinate>();
                var lastTileId = -2;
                for (var x = 0; x < board.Width; x++)
                {
                    var coordinate = new BoardCoordinate(x, y);
                    if (!board.TryGetCell(coordinate, out var cell) || !cell.IsOccupied)
                    {
                        FlushRun(run, groups);
                        lastTileId = -2;
                        continue;
                    }

                    if (cell.TileId != lastTileId)
                    {
                        FlushRun(run, groups);
                        run = new List<BoardCoordinate>();
                    }

                    run.Add(coordinate);
                    lastTileId = cell.TileId;
                }

                FlushRun(run, groups);
            }

            for (var x = 0; x < board.Width; x++)
            {
                var run = new List<BoardCoordinate>();
                var lastTileId = -2;
                for (var y = 0; y < board.Height; y++)
                {
                    var coordinate = new BoardCoordinate(x, y);
                    if (!board.TryGetCell(coordinate, out var cell) || !cell.IsOccupied)
                    {
                        FlushRun(run, groups);
                        lastTileId = -2;
                        continue;
                    }

                    if (cell.TileId != lastTileId)
                    {
                        FlushRun(run, groups);
                        run = new List<BoardCoordinate>();
                    }

                    run.Add(coordinate);
                    lastTileId = cell.TileId;
                }

                FlushRun(run, groups);
            }

            return groups;
        }

        private static void FlushRun(List<BoardCoordinate> run, List<MatchGroup> groups)
        {
            if (run.Count >= 3)
            {
                groups.Add(new MatchGroup(new List<BoardCoordinate>(run)));
            }
        }
    }
}
