using MatchJoy.Board;

namespace MatchJoy.Input
{
    public readonly struct SwapRequest
    {
        public SwapRequest(BoardCoordinate source, BoardCoordinate target)
        {
            Source = source;
            Target = target;
        }

        public BoardCoordinate Source { get; }
        public BoardCoordinate Target { get; }
    }
}
