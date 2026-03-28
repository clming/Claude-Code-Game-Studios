namespace MatchJoy.Board
{
    public sealed class BoardCell
    {
        public BoardCell(BoardCoordinate coordinate, BoardCellState state, int tileId)
        {
            Coordinate = coordinate;
            State = state;
            TileId = tileId;
        }

        public BoardCoordinate Coordinate { get; }

        public BoardCellState State { get; private set; }

        public int TileId { get; private set; }

        public bool IsPlayable => State != BoardCellState.Disabled;
        public bool IsOccupied => State == BoardCellState.Occupied;

        public void SetEmpty()
        {
            State = BoardCellState.Empty;
            TileId = -1;
        }

        public void SetTile(int tileId)
        {
            State = BoardCellState.Occupied;
            TileId = tileId;
        }
    }
}
