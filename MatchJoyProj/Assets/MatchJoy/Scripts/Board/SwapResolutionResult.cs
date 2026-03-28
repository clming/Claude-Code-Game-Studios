using System.Collections.Generic;
using MatchJoy.Input;

namespace MatchJoy.Board
{
    public sealed class SwapResolutionResult
    {
        public static SwapResolutionResult Rejected(SwapRequest request)
        {
            return new SwapResolutionResult(request, false, new List<MatchGroup>());
        }

        public static SwapResolutionResult Accepted(SwapRequest request, List<MatchGroup> matchGroups)
        {
            return new SwapResolutionResult(request, true, matchGroups);
        }

        private SwapResolutionResult(SwapRequest request, bool accepted, List<MatchGroup> matchGroups)
        {
            Request = request;
            Accepted = accepted;
            MatchGroups = matchGroups;
        }

        public SwapRequest Request { get; }
        public bool Accepted { get; }
        public List<MatchGroup> MatchGroups { get; }
    }
}
