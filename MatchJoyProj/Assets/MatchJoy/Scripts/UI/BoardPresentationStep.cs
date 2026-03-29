namespace MatchJoy.UI
{
    public enum BoardPresentationStepCompletionMode
    {
        Immediate,
        AwaitAnimatedSettle
    }

    public enum BoardPresentationStepReason
    {
        Unknown,
        PassStart,
        InitialPopulation,
        InteractionSelection,
        RejectedSwapFeedback,
        ResolvedSequenceEntry,
        SwapPreview,
        ChangedCells,
        PassCompletion
    }

    public enum BoardPresentationStepType
    {
        Prepare,
        InteractionRefresh,
        SwapPreview,
        ResolveChanges,
        Complete
    }

    public readonly struct BoardPresentationStep
    {
        public BoardPresentationStep(
            BoardPresentationStepType type,
            string label,
            BoardPresentationStepReason reason = BoardPresentationStepReason.Unknown,
            BoardPresentationStepCompletionMode completionMode = BoardPresentationStepCompletionMode.Immediate)
        {
            Type = type;
            Label = label;
            Reason = reason;
            CompletionMode = completionMode;
        }

        public BoardPresentationStepType Type { get; }
        public string Label { get; }
        public BoardPresentationStepReason Reason { get; }
        public BoardPresentationStepCompletionMode CompletionMode { get; }
    }
}
