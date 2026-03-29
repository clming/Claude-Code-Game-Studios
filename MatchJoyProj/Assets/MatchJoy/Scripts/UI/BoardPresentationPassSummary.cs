using System.Collections.Generic;
using System.Text;

namespace MatchJoy.UI
{
    public sealed class BoardPresentationPassSummary
    {
        public BoardPresentationPassSummary(
            int token,
            BoardView.PresentationMode mode,
            string label,
            BoardPresentationIntent intent,
            BoardPresentationStage stage,
            IReadOnlyList<BoardPresentationStep> steps,
            int updatedCellCount,
            int animatedCellCount,
            bool completedImmediately,
            float durationSeconds,
            IReadOnlyDictionary<BoardCellView.RefreshTransitionType, int> transitionCounts,
            IReadOnlyDictionary<BoardCellPresentationPhaseType, int> phaseCounts)
        {
            Token = token;
            Mode = mode;
            Label = label;
            Intent = intent;
            Stage = stage;
            Steps = steps;
            UpdatedCellCount = updatedCellCount;
            AnimatedCellCount = animatedCellCount;
            CompletedImmediately = completedImmediately;
            DurationSeconds = durationSeconds;
            TransitionCounts = transitionCounts;
            PhaseCounts = phaseCounts;
        }

        public int Token { get; }
        public BoardView.PresentationMode Mode { get; }
        public string Label { get; }
        public BoardPresentationIntent Intent { get; }
        public BoardPresentationStage Stage { get; }
        public IReadOnlyList<BoardPresentationStep> Steps { get; }
        public int UpdatedCellCount { get; }
        public int AnimatedCellCount { get; }
        public bool CompletedImmediately { get; }
        public float DurationSeconds { get; }
        public IReadOnlyDictionary<BoardCellView.RefreshTransitionType, int> TransitionCounts { get; }
        public IReadOnlyDictionary<BoardCellPresentationPhaseType, int> PhaseCounts { get; }

        public string BuildDebugString()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Last Presentation Pass");
            builder.AppendLine($"- Token: {Token}");
            builder.AppendLine($"- Mode: {Mode}");
            builder.AppendLine($"- Label: {Label}");
            builder.AppendLine($"- Intent: {Intent}");
            builder.AppendLine($"- Stage: {Stage}");
            builder.AppendLine($"- Steps: {BuildStepSummary(Steps)}");
            builder.AppendLine($"- Updated Cells: {UpdatedCellCount}");
            builder.AppendLine($"- Animated Cells: {AnimatedCellCount}");
            builder.AppendLine($"- Completed Immediately: {CompletedImmediately}");
            builder.AppendLine($"- Duration Seconds: {DurationSeconds:F3}");
            builder.AppendLine($"- Transitions: {BuildCountSummary(TransitionCounts)}");
            builder.AppendLine($"- Phases: {BuildCountSummary(PhaseCounts)}");
            return builder.ToString();
        }

        public string BuildCompactDebugString()
        {
            return $"#{Token} [{Label}] Intent={Intent}, Stage={Stage}, Steps={BuildStepSummary(Steps)}, Mode={Mode}, Updated={UpdatedCellCount}, Animated={AnimatedCellCount}, Settled={CompletedImmediately}, Duration={DurationSeconds:F3}s, Transitions={BuildCountSummary(TransitionCounts)}, Phases={BuildCountSummary(PhaseCounts)}";
        }

        public string BuildTestLogSnippet(string testId = "PB-XX", string title = "Presentation Observation")
        {
            var builder = new StringBuilder();
            builder.AppendLine($"### {testId} - {title}");
            builder.AppendLine();
            builder.AppendLine($"- Scene action: {Label}");
            builder.AppendLine($"- Expected: ");
            builder.AppendLine($"- Actual: Pass #{Token} ran in stage `{Stage}` with mode `{Mode}` and settled in approximately `{DurationSeconds:F3}s`.");
            builder.AppendLine($"- Board steps: `{BuildStepSummary(Steps)}`");
            builder.AppendLine($"- Repro frequency: ");
            builder.AppendLine($"- Console evidence: `Updated={UpdatedCellCount}`, `Animated={AnimatedCellCount}`, `Transitions={BuildCountSummary(TransitionCounts)}`, `Phases={BuildCountSummary(PhaseCounts)}`");
            builder.AppendLine($"- Most likely layer: ");
            builder.AppendLine($"- Next fix idea: ");
            return builder.ToString();
        }

        private static string BuildCountSummary<T>(IReadOnlyDictionary<T, int> counts)
        {
            if (counts == null || counts.Count == 0)
            {
                return "none";
            }

            var parts = new List<string>();
            foreach (var pair in counts)
            {
                parts.Add($"{pair.Key}:{pair.Value}");
            }

            return string.Join(", ", parts);
        }

        private static string BuildStepSummary(IReadOnlyList<BoardPresentationStep> steps)
        {
            if (steps == null || steps.Count == 0)
            {
                return "none";
            }

            var parts = new List<string>();
            for (var i = 0; i < steps.Count; i++)
            {
                parts.Add($"{steps[i].Type}({steps[i].Reason},{steps[i].CompletionMode})");
            }

            return string.Join(" -> ", parts);
        }
    }
}
