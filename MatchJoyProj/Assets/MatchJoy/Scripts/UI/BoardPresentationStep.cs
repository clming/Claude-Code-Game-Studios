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
            int sequenceIndex,
            BoardPresentationStepType type,
            string label,
            BoardPresentationStepReason reason = BoardPresentationStepReason.Unknown,
            BoardPresentationStepCompletionMode completionMode = BoardPresentationStepCompletionMode.Immediate,
            float expectedDurationSeconds = 0f)
        {
            SequenceIndex = sequenceIndex;
            Type = type;
            Label = label;
            Reason = reason;
            CompletionMode = completionMode;
            ExpectedDurationSeconds = expectedDurationSeconds;
        }

        public int SequenceIndex { get; }
        public BoardPresentationStepType Type { get; }
        public string Label { get; }
        public BoardPresentationStepReason Reason { get; }
        public BoardPresentationStepCompletionMode CompletionMode { get; }
        public float ExpectedDurationSeconds { get; }
    }
}
