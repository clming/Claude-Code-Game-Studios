using System.Collections.Generic;

namespace MatchJoy.UI
{
    internal sealed class BoardPresentationPassContext
    {
        public int Token { get; set; }
        public int PendingAnimatedCellCount { get; set; }
        public float StartRealtime { get; set; }
        public BoardPresentationPassSummary LastSummary { get; set; }
        public List<BoardPresentationPassSummary> History { get; } = new();

        public void ResetForNewPass(int token, float startRealtime)
        {
            Token = token;
            StartRealtime = startRealtime;
            PendingAnimatedCellCount = 0;
        }
    }
}
