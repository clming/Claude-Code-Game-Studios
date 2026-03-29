using System.Collections.Generic;

namespace MatchJoy.UI
{
    public readonly struct BoardCellPresentationInstruction
    {
        public BoardCellPresentationInstruction(
            BoardCellView.RefreshTransition transition,
            BoardPresentationIntent intent,
            BoardPresentationStage stage,
            IReadOnlyList<BoardCellPresentationPhase> phases)
        {
            Transition = transition;
            Intent = intent;
            Stage = stage;
            Phases = phases;
            RequiresAnimation = ComputeRequiresAnimation(phases);
        }

        public BoardCellView.RefreshTransition Transition { get; }
        public BoardPresentationIntent Intent { get; }
        public BoardPresentationStage Stage { get; }
        public IReadOnlyList<BoardCellPresentationPhase> Phases { get; }
        public bool RequiresAnimation { get; }

        private static bool ComputeRequiresAnimation(IReadOnlyList<BoardCellPresentationPhase> phases)
        {
            if (phases == null)
            {
                return false;
            }

            for (var i = 0; i < phases.Count; i++)
            {
                if (phases[i].Type != BoardCellPresentationPhaseType.ApplyVisualState)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
