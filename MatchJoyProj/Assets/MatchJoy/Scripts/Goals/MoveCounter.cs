namespace MatchJoy.Goals
{
    public sealed class MoveCounter
    {
        public MoveCounter(int startingMoves)
        {
            RemainingMoves = startingMoves;
        }

        public int RemainingMoves { get; private set; }

        public void ConsumeAcceptedMove()
        {
            if (RemainingMoves > 0)
            {
                RemainingMoves--;
            }
        }
    }
}
