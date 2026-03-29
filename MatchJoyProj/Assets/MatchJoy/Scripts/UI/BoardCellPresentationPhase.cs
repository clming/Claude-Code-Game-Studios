using UnityEngine;

namespace MatchJoy.UI
{
    public enum BoardCellPresentationPhaseType
    {
        Wait,
        SwapPreview,
        ClearFade,
        ApplyVisualState,
        SelectionPulse,
        TileChangePulse,
        InitialPopulateFade,
        RefillDrop
    }

    public readonly struct BoardCellPresentationPhase
    {
        private BoardCellPresentationPhase(
            BoardCellPresentationPhaseType type,
            float durationSeconds = 0f,
            Vector2 offset = default)
        {
            Type = type;
            DurationSeconds = durationSeconds;
            Offset = offset;
        }

        public BoardCellPresentationPhaseType Type { get; }
        public float DurationSeconds { get; }
        public Vector2 Offset { get; }

        public static BoardCellPresentationPhase Wait(float durationSeconds)
        {
            return new BoardCellPresentationPhase(BoardCellPresentationPhaseType.Wait, durationSeconds);
        }

        public static BoardCellPresentationPhase SwapPreview(float durationSeconds, Vector2 offset)
        {
            return new BoardCellPresentationPhase(BoardCellPresentationPhaseType.SwapPreview, durationSeconds, offset);
        }

        public static BoardCellPresentationPhase ClearFade(float durationSeconds)
        {
            return new BoardCellPresentationPhase(BoardCellPresentationPhaseType.ClearFade, durationSeconds);
        }

        public static BoardCellPresentationPhase ApplyVisualState()
        {
            return new BoardCellPresentationPhase(BoardCellPresentationPhaseType.ApplyVisualState);
        }

        public static BoardCellPresentationPhase SelectionPulse()
        {
            return new BoardCellPresentationPhase(BoardCellPresentationPhaseType.SelectionPulse);
        }

        public static BoardCellPresentationPhase TileChangePulse()
        {
            return new BoardCellPresentationPhase(BoardCellPresentationPhaseType.TileChangePulse);
        }

        public static BoardCellPresentationPhase InitialPopulateFade()
        {
            return new BoardCellPresentationPhase(BoardCellPresentationPhaseType.InitialPopulateFade);
        }

        public static BoardCellPresentationPhase RefillDrop(Vector2 originOffset)
        {
            return new BoardCellPresentationPhase(BoardCellPresentationPhaseType.RefillDrop, 0f, originOffset);
        }
    }
}
