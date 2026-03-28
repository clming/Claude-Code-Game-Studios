using MatchJoy.Authoring;

namespace MatchJoy.Board
{
    public static class BoardBuilder
    {
        public static BoardState Build(LevelDefinition definition)
        {
            var board = new BoardState(definition.BoardWidth, definition.BoardHeight);

            for (var y = 0; y < definition.BoardHeight; y++)
            {
                for (var x = 0; x < definition.BoardWidth; x++)
                {
                    var coordinate = new BoardCoordinate(x, y);
                    var tileId = definition.GetInitialTileId(x, y);
                    var state = tileId >= 0 ? BoardCellState.Occupied : BoardCellState.Empty;
                    board.SetCell(new BoardCell(coordinate, state, tileId));
                }
            }

            return board;
        }
    }
}
