namespace MatchJoy.UI
{
    public sealed class BoardPresentationPassPlan
    {
        public BoardPresentationPassPlan(
            BoardPresentationStage stage,
            IReadOnlyList<BoardPresentationStep> steps)
        {
            Stage = stage;
            Steps = steps;
        }

        public BoardPresentationStage Stage { get; }
        public IReadOnlyList<BoardPresentationStep> Steps { get; }
    }
}
