using MatchJoy.Board;

namespace MatchJoy.UI
{
    public enum BoardPresentationIntent
    {
        Unknown,
        InitialBuild,
        SelectionRefresh,
        RejectedSwapRefresh,
        AcceptedSwapResolve
    }

    public readonly struct BoardPresentationRenderRequest
    {
        public BoardPresentationRenderRequest(
            BoardCoordinate? selectedCoordinate = null,
            BoardView.PresentationMode presentationMode = BoardView.PresentationMode.Immediate,
            BoardView.SwapPresentation? swapPresentation = null,
            string presentationLabel = null,
            BoardPresentationIntent presentationIntent = BoardPresentationIntent.Unknown,
            BoardPresentationStage presentationStage = BoardPresentationStage.Unknown)
        {
            SelectedCoordinate = selectedCoordinate;
            PresentationMode = presentationMode;
            SwapPresentation = swapPresentation;
            PresentationLabel = presentationLabel;
            PresentationIntent = presentationIntent;
            PresentationStage = presentationStage;
        }

        public BoardCoordinate? SelectedCoordinate { get; }
        public BoardView.PresentationMode PresentationMode { get; }
        public BoardView.SwapPresentation? SwapPresentation { get; }
        public string PresentationLabel { get; }
        public BoardPresentationIntent PresentationIntent { get; }
        public BoardPresentationStage PresentationStage { get; }

        public static BoardPresentationRenderRequest Immediate(
            BoardCoordinate? selectedCoordinate = null,
            string presentationLabel = null,
            BoardPresentationIntent presentationIntent = BoardPresentationIntent.Unknown)
        {
            return new BoardPresentationRenderRequest(
                selectedCoordinate,
                BoardView.PresentationMode.Immediate,
                null,
                presentationLabel,
                presentationIntent,
                ResolveStage(presentationIntent));
        }

        public static BoardPresentationRenderRequest ResolvedSwap(
            BoardCoordinate source,
            BoardCoordinate target,
            string presentationLabel = null,
            BoardPresentationIntent presentationIntent = BoardPresentationIntent.AcceptedSwapResolve)
        {
            return new BoardPresentationRenderRequest(
                null,
                BoardView.PresentationMode.ResolvedSequence,
                new BoardView.SwapPresentation(source, target),
                presentationLabel,
                presentationIntent,
                BoardPresentationStage.SwapResolution);
        }

        private static BoardPresentationStage ResolveStage(BoardPresentationIntent presentationIntent)
        {
            return presentationIntent switch
            {
                BoardPresentationIntent.InitialBuild => BoardPresentationStage.InitialBuild,
                BoardPresentationIntent.SelectionRefresh => BoardPresentationStage.InteractionRefresh,
                BoardPresentationIntent.RejectedSwapRefresh => BoardPresentationStage.InteractionRefresh,
                BoardPresentationIntent.AcceptedSwapResolve => BoardPresentationStage.SwapResolution,
                _ => BoardPresentationStage.Unknown
            };
        }
    }
}
