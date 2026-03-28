using MatchJoy.Board;

namespace MatchJoy.Input
{
    public sealed class TileSelectionState
    {
        public bool HasSelection { get; private set; }
        public BoardCoordinate SelectedCoordinate { get; private set; }

        public void Select(BoardCoordinate coordinate)
        {
            SelectedCoordinate = coordinate;
            HasSelection = true;
        }

        public void Clear()
        {
            HasSelection = false;
        }
    }
}
