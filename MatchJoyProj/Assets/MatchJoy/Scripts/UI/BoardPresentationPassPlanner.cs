using System.Collections.Generic;

namespace MatchJoy.UI
{
    public sealed class BoardPresentationPassPlanner
    {
        public BoardPresentationPassPlan BuildPlan(BoardPresentationPassPlanningContext context)
        {
            var steps = new List<BoardPresentationStep>
            {
                new(BoardPresentationStepType.Prepare, "Prepare pass", BoardPresentationStepReason.PassStart)
            };

            switch (context.Stage)
            {
                case BoardPresentationStage.InitialBuild:
                    steps.Add(new BoardPresentationStep(
                        BoardPresentationStepType.ResolveChanges,
                        "Populate initial board",
                        BoardPresentationStepReason.InitialPopulation));
                    break;
                case BoardPresentationStage.InteractionRefresh:
                    steps.Add(new BoardPresentationStep(
                        BoardPresentationStepType.InteractionRefresh,
                        context.Intent == BoardPresentationIntent.RejectedSwapRefresh
                            ? "Refresh rejected swap state"
                            : "Refresh interaction state",
                        context.Intent == BoardPresentationIntent.RejectedSwapRefresh
                            ? BoardPresentationStepReason.RejectedSwapFeedback
                            : BoardPresentationStepReason.InteractionSelection));
                    break;
                case BoardPresentationStage.SwapResolution:
                    if (context.Mode == BoardView.PresentationMode.ResolvedSequence)
                    {
                        steps.Add(new BoardPresentationStep(
                            BoardPresentationStepType.InteractionRefresh,
                            "Lock into resolved sequence",
                            BoardPresentationStepReason.ResolvedSequenceEntry));
                    }

                    if (HasPhase(context.PhaseCounts, BoardCellPresentationPhaseType.SwapPreview))
                    {
                        steps.Add(new BoardPresentationStep(
                            BoardPresentationStepType.SwapPreview,
                            "Preview accepted swap",
                            BoardPresentationStepReason.SwapPreview));
                    }

                    if (context.UpdatedCellCount > 0)
                    {
                        steps.Add(new BoardPresentationStep(
                            BoardPresentationStepType.ResolveChanges,
                            "Resolve changed cells",
                            BoardPresentationStepReason.ChangedCells));
                    }
                    break;
                default:
                    if (HasTransition(context.TransitionCounts, BoardCellView.RefreshTransitionType.SelectionPulse))
                    {
                        steps.Add(new BoardPresentationStep(
                            BoardPresentationStepType.InteractionRefresh,
                            "Apply selection feedback",
                            BoardPresentationStepReason.InteractionSelection));
                    }

                    if (context.UpdatedCellCount > 0)
                    {
                        steps.Add(new BoardPresentationStep(
                            BoardPresentationStepType.ResolveChanges,
                            "Apply board changes",
                            BoardPresentationStepReason.ChangedCells));
                    }
                    break;
            }

            steps.Add(new BoardPresentationStep(
                BoardPresentationStepType.Complete,
                context.AnimatedCellCount > 0 ? "Await animated settle" : "Complete immediately",
                BoardPresentationStepReason.PassCompletion,
                context.AnimatedCellCount > 0
                    ? BoardPresentationStepCompletionMode.AwaitAnimatedSettle
                    : BoardPresentationStepCompletionMode.Immediate));

            return new BoardPresentationPassPlan(context.Stage, steps);
        }

        private static bool HasPhase(
            IReadOnlyDictionary<BoardCellPresentationPhaseType, int> phaseCounts,
            BoardCellPresentationPhaseType phaseType)
        {
            return phaseCounts != null
                && phaseCounts.TryGetValue(phaseType, out var count)
                && count > 0;
        }

        private static bool HasTransition(
            IReadOnlyDictionary<BoardCellView.RefreshTransitionType, int> transitionCounts,
            BoardCellView.RefreshTransitionType transitionType)
        {
            return transitionCounts != null
                && transitionCounts.TryGetValue(transitionType, out var count)
                && count > 0;
        }
    }
}
