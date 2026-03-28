using System.Collections.Generic;

namespace MatchJoy.Board
{
    public sealed class BoardState
    {
        private readonly Dictionary<(int X, int Y), BoardCell> _cells = new();

        public int Width { get; }
        public int Height { get; }

        public BoardState(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public void SetCell(BoardCell cell)
        {
            _cells[(cell.Coordinate.X, cell.Coordinate.Y)] = cell;
        }

        public bool TryGetCell(BoardCoordinate coordinate, out BoardCell cell)
        {
            return _cells.TryGetValue((coordinate.X, coordinate.Y), out cell);
        }

        public bool IsInBounds(BoardCoordinate coordinate)
        {
            return coordinate.X >= 0 && coordinate.X < Width && coordinate.Y >= 0 && coordinate.Y < Height;
        }

        public IEnumerable<BoardCell> GetAllCells()
        {
            return _cells.Values;
        }

        public bool CanSwap(BoardCoordinate source, BoardCoordinate target)
        {
            if (!IsInBounds(source) || !IsInBounds(target))
            {
                return false;
            }

            if (!TryGetCell(source, out var sourceCell) || !TryGetCell(target, out var targetCell))
            {
                return false;
            }

            return sourceCell.IsOccupied && targetCell.IsOccupied && sourceCell.IsPlayable && targetCell.IsPlayable;
        }

        public void SwapTiles(BoardCoordinate source, BoardCoordinate target)
        {
            if (!TryGetCell(source, out var sourceCell) || !TryGetCell(target, out var targetCell))
            {
                return;
            }

            var sourceTileId = sourceCell.TileId;
            sourceCell.SetTile(targetCell.TileId);
            targetCell.SetTile(sourceTileId);
        }
    }
}
