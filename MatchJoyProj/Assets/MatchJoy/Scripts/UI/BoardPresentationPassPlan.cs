namespace MatchJoy.UI
{
    public sealed class BoardPresentationPassPlan
    {
        public BoardPresentationPassPlan(
            BoardPresentationStage stage,
            IReadOnlyList<BoardPresentationStep> steps,
            float estimatedDurationSeconds)
        {
            Stage = stage;
            Steps = steps;
            EstimatedDurationSeconds = estimatedDurationSeconds;
        }

        public BoardPresentationStage Stage { get; }
        public IReadOnlyList<BoardPresentationStep> Steps { get; }
        public float EstimatedDurationSeconds { get; }
    }
}
