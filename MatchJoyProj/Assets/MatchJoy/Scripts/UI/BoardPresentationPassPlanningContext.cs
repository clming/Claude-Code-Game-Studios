using System.Collections.Generic;

namespace MatchJoy.UI
{
    public readonly struct BoardPresentationPassPlanningContext
    {
        public BoardPresentationPassPlanningContext(
            BoardView.PresentationMode mode,
            BoardPresentationIntent intent,
            BoardPresentationStage stage,
            IReadOnlyDictionary<BoardCellView.RefreshTransitionType, int> transitionCounts,
            IReadOnlyDictionary<BoardCellPresentationPhaseType, int> phaseCounts,
            int updatedCellCount,
            int animatedCellCount)
        {
            Mode = mode;
            Intent = intent;
            Stage = stage;
            TransitionCounts = transitionCounts;
            PhaseCounts = phaseCounts;
            UpdatedCellCount = updatedCellCount;
            AnimatedCellCount = animatedCellCount;
        }

        public BoardView.PresentationMode Mode { get; }
        public BoardPresentationIntent Intent { get; }
        public BoardPresentationStage Stage { get; }
        public IReadOnlyDictionary<BoardCellView.RefreshTransitionType, int> TransitionCounts { get; }
        public IReadOnlyDictionary<BoardCellPresentationPhaseType, int> PhaseCounts { get; }
        public int UpdatedCellCount { get; }
        public int AnimatedCellCount { get; }
    }
}
