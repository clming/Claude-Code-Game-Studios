using System.Collections.Generic;

namespace MatchJoy.Board
{
    public sealed class MatchGroup
    {
        public MatchGroup(List<BoardCoordinate> coordinates)
        {
            Coordinates = coordinates;
        }

        public List<BoardCoordinate> Coordinates { get; }
    }
}
