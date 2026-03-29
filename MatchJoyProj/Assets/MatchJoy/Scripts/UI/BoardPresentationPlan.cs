using MatchJoy.Board;
using UnityEngine;

namespace MatchJoy.UI
{
    public sealed class BoardPresentationPlan
    {
        private readonly Vector2 _cellSize;
        private readonly float _initialPopulateStepDelay;
        private readonly float _refillRowStepDelay;
        private readonly float _tileChangeWaveStepDelay;
        private readonly float _refillSpawnOffsetCells;
        private readonly float _swapPreviewDuration;
        private readonly float _swapPreviewOffsetCells;
        private readonly float _clearPreviewDuration;

        public BoardPresentationPlan(
            Vector2 cellSize,
            float initialPopulateStepDelay,
            float refillRowStepDelay,
            float tileChangeWaveStepDelay,
            float refillSpawnOffsetCells,
            float swapPreviewDuration,
            float swapPreviewOffsetCells,
            float clearPreviewDuration)
        {
            _cellSize = cellSize;
            _initialPopulateStepDelay = initialPopulateStepDelay;
            _refillRowStepDelay = refillRowStepDelay;
            _tileChangeWaveStepDelay = tileChangeWaveStepDelay;
            _refillSpawnOffsetCells = refillSpawnOffsetCells;
            _swapPreviewDuration = swapPreviewDuration;
            _swapPreviewOffsetCells = swapPreviewOffsetCells;
            _clearPreviewDuration = clearPreviewDuration;
        }

        public BoardCellPresentationInstruction BuildInstruction(
            BoardState board,
            BoardCoordinate coordinate,
            bool hasPreviousSnapshot,
            bool previousIsOccupied,
            int previousTileId,
            bool previousIsSelected,
            bool nextIsOccupied,
            int nextTileId,
            bool nextIsSelected,
            BoardPresentationRenderRequest request)
        {
            if (!hasPreviousSnapshot)
            {
                var populateDelay = request.PresentationMode == BoardView.PresentationMode.ResolvedSequence
                    ? (coordinate.Y * board.Width + coordinate.X) * _initialPopulateStepDelay
                    : 0f;
                return CreateInstruction(
                    new BoardCellView.RefreshTransition(
                        BoardCellView.RefreshTransitionType.InitialPopulate,
                        populateDelay > 0f,
                        populateDelay),
                    nextIsOccupied,
                    request);
            }

            if (previousIsSelected != nextIsSelected)
            {
                return CreateInstruction(
                    new BoardCellView.RefreshTransition(BoardCellView.RefreshTransitionType.SelectionPulse, false),
                    nextIsOccupied,
                    request);
            }

            if (!previousIsOccupied && nextIsOccupied)
            {
                var refillDelay = request.PresentationMode == BoardView.PresentationMode.ResolvedSequence
                    ? (board.Height - 1 - coordinate.Y) * _refillRowStepDelay
                    : 0f;
                var refillOffset = request.PresentationMode == BoardView.PresentationMode.ResolvedSequence
                    ? new Vector2(0f, _cellSize.y * _refillSpawnOffsetCells)
                    : Vector2.zero;
                return CreateInstruction(
                    new BoardCellView.RefreshTransition(
                        BoardCellView.RefreshTransitionType.Refilled,
                        request.PresentationMode == BoardView.PresentationMode.ResolvedSequence,
                        refillDelay,
                        refillOffset),
                    nextIsOccupied,
                    request);
            }

            if (previousTileId != nextTileId || previousIsOccupied != nextIsOccupied)
            {
                var distanceFromCenter = Mathf.Abs(coordinate.X - ((board.Width - 1) * 0.5f));
                var waveDelay = request.PresentationMode == BoardView.PresentationMode.ResolvedSequence
                    ? distanceFromCenter * _tileChangeWaveStepDelay
                    : 0f;
                var preApplyOffset = Vector2.zero;
                var preApplyDuration = 0f;
                var preApplyEffectType = BoardCellView.PreApplyEffectType.None;

                if (request.PresentationMode == BoardView.PresentationMode.ResolvedSequence
                    && request.SwapPresentation.HasValue
                    && TryBuildSwapPreviewOffset(coordinate, request.SwapPresentation.Value, out var swapOffset))
                {
                    preApplyOffset = swapOffset;
                    preApplyDuration = _swapPreviewDuration;
                    preApplyEffectType = BoardCellView.PreApplyEffectType.SwapPreview;
                }

                if (request.PresentationMode == BoardView.PresentationMode.ResolvedSequence
                    && preApplyEffectType == BoardCellView.PreApplyEffectType.None
                    && previousIsOccupied)
                {
                    preApplyDuration = _clearPreviewDuration;
                    preApplyEffectType = BoardCellView.PreApplyEffectType.ClearFade;
                }

                return CreateInstruction(
                    new BoardCellView.RefreshTransition(
                        BoardCellView.RefreshTransitionType.TileChanged,
                        request.PresentationMode == BoardView.PresentationMode.ResolvedSequence,
                        waveDelay,
                        Vector2.zero,
                        preApplyOffset,
                        preApplyDuration,
                        preApplyEffectType),
                    nextIsOccupied,
                    request);
            }

            return CreateInstruction(
                new BoardCellView.RefreshTransition(BoardCellView.RefreshTransitionType.None, false),
                nextIsOccupied,
                request);
        }

        private bool TryBuildSwapPreviewOffset(BoardCoordinate coordinate, BoardView.SwapPresentation swapPresentation, out Vector2 previewOffset)
        {
            previewOffset = Vector2.zero;

            if (coordinate.X == swapPresentation.Source.X && coordinate.Y == swapPresentation.Source.Y)
            {
                previewOffset = CreatePreviewOffset(swapPresentation.Source, swapPresentation.Target);
                return true;
            }

            if (coordinate.X == swapPresentation.Target.X && coordinate.Y == swapPresentation.Target.Y)
            {
                previewOffset = CreatePreviewOffset(swapPresentation.Target, swapPresentation.Source);
                return true;
            }

            return false;
        }

        private Vector2 CreatePreviewOffset(BoardCoordinate from, BoardCoordinate to)
        {
            var dx = to.X - from.X;
            var dy = to.Y - from.Y;
            return new Vector2(
                dx * _cellSize.x * _swapPreviewOffsetCells,
                dy * _cellSize.y * _swapPreviewOffsetCells);
        }

        private static BoardCellPresentationInstruction CreateInstruction(
            BoardCellView.RefreshTransition transition,
            bool nextIsOccupied,
            BoardPresentationRenderRequest request)
        {
            var phases = BoardCellView.BuildPresentationPhases(transition, nextIsOccupied);
            return new BoardCellPresentationInstruction(transition, request.PresentationIntent, request.PresentationStage, phases);
        }
    }
}
