using System.Collections.Generic;

namespace MatchJoy.UI
{
    public sealed class BoardPresentationPassPlanner
    {
        private const float ShortStepDurationSeconds = 0.10f;
        private const float ResolveStepDurationSeconds = 0.12f;
        private const float AwaitSettleDurationSeconds = 0.18f;

        public BoardPresentationPassPlan BuildPlan(BoardPresentationPassPlanningContext context)
        {
            var steps = new List<BoardPresentationStep>();
            var nextSequenceIndex = 0;

            AddStep(
                steps,
                ref nextSequenceIndex,
                BoardPresentationStepType.Prepare,
                "Prepare pass",
                BoardPresentationStepReason.PassStart);

            switch (context.Stage)
            {
                case BoardPresentationStage.InitialBuild:
                    AddStep(
                        steps,
                        ref nextSequenceIndex,
                        BoardPresentationStepType.ResolveChanges,
                        "Populate initial board",
                        BoardPresentationStepReason.InitialPopulation,
                        BoardPresentationStepCompletionMode.Immediate,
                        ResolveStepDurationSeconds);
                    break;
                case BoardPresentationStage.InteractionRefresh:
                    AddStep(
                        steps,
                        ref nextSequenceIndex,
                        BoardPresentationStepType.InteractionRefresh,
                        context.Intent == BoardPresentationIntent.RejectedSwapRefresh
                            ? "Refresh rejected swap state"
                            : "Refresh interaction state",
                        context.Intent == BoardPresentationIntent.RejectedSwapRefresh
                            ? BoardPresentationStepReason.RejectedSwapFeedback
                            : BoardPresentationStepReason.InteractionSelection,
                        BoardPresentationStepCompletionMode.Immediate,
                        ShortStepDurationSeconds);
                    break;
                case BoardPresentationStage.SwapResolution:
                    if (context.Mode == BoardView.PresentationMode.ResolvedSequence)
                    {
                        AddStep(
                            steps,
                            ref nextSequenceIndex,
                            BoardPresentationStepType.InteractionRefresh,
                            "Lock into resolved sequence",
                            BoardPresentationStepReason.ResolvedSequenceEntry,
                            BoardPresentationStepCompletionMode.Immediate,
                            ShortStepDurationSeconds);
                    }

                    if (HasPhase(context.PhaseCounts, BoardCellPresentationPhaseType.SwapPreview))
                    {
                        AddStep(
                            steps,
                            ref nextSequenceIndex,
                            BoardPresentationStepType.SwapPreview,
                            "Preview accepted swap",
                            BoardPresentationStepReason.SwapPreview,
                            BoardPresentationStepCompletionMode.Immediate,
                            ShortStepDurationSeconds);
                    }

                    if (context.UpdatedCellCount > 0)
                    {
                        AddStep(
                            steps,
                            ref nextSequenceIndex,
                            BoardPresentationStepType.ResolveChanges,
                            "Resolve changed cells",
                            BoardPresentationStepReason.ChangedCells,
                            BoardPresentationStepCompletionMode.Immediate,
                            ResolveStepDurationSeconds);
                    }
                    break;
                default:
                    if (HasTransition(context.TransitionCounts, BoardCellView.RefreshTransitionType.SelectionPulse))
                    {
                        AddStep(
                            steps,
                            ref nextSequenceIndex,
                            BoardPresentationStepType.InteractionRefresh,
                            "Apply selection feedback",
                            BoardPresentationStepReason.InteractionSelection,
                            BoardPresentationStepCompletionMode.Immediate,
                            ShortStepDurationSeconds);
                    }

                    if (context.UpdatedCellCount > 0)
                    {
                        AddStep(
                            steps,
                            ref nextSequenceIndex,
                            BoardPresentationStepType.ResolveChanges,
                            "Apply board changes",
                            BoardPresentationStepReason.ChangedCells,
                            BoardPresentationStepCompletionMode.Immediate,
                            ResolveStepDurationSeconds);
                    }
                    break;
            }

            AddStep(
                steps,
                ref nextSequenceIndex,
                BoardPresentationStepType.Complete,
                context.AnimatedCellCount > 0 ? "Await animated settle" : "Complete immediately",
                BoardPresentationStepReason.PassCompletion,
                context.AnimatedCellCount > 0
                    ? BoardPresentationStepCompletionMode.AwaitAnimatedSettle
                    : BoardPresentationStepCompletionMode.Immediate,
                context.AnimatedCellCount > 0 ? AwaitSettleDurationSeconds : 0f);

            return new BoardPresentationPassPlan(
                context.Stage,
                steps,
                EstimateDurationSeconds(steps));
        }

        private static void AddStep(
            ICollection<BoardPresentationStep> steps,
            ref int nextSequenceIndex,
            BoardPresentationStepType type,
            string label,
            BoardPresentationStepReason reason,
            BoardPresentationStepCompletionMode completionMode = BoardPresentationStepCompletionMode.Immediate,
            float expectedDurationSeconds = 0f)
        {
            steps.Add(new BoardPresentationStep(nextSequenceIndex, type, label, reason, completionMode, expectedDurationSeconds));
            nextSequenceIndex++;
        }

        private static float EstimateDurationSeconds(IReadOnlyCollection<BoardPresentationStep> steps)
        {
            var total = 0f;
            foreach (var step in steps)
            {
                total += step.ExpectedDurationSeconds;
            }

            return total;
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
