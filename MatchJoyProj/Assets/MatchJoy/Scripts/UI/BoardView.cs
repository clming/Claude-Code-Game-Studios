using System;
using System.Collections.Generic;
using System.Text;
using MatchJoy.Board;
using UnityEngine;

namespace MatchJoy.UI
{
    public sealed class BoardView : MonoBehaviour
    {
        public enum PresentationMode
        {
            Immediate,
            ResolvedSequence
        }

        public readonly struct SwapPresentation
        {
            public SwapPresentation(BoardCoordinate source, BoardCoordinate target)
            {
                Source = source;
                Target = target;
            }

            public BoardCoordinate Source { get; }
            public BoardCoordinate Target { get; }
        }

        private readonly struct CellRenderSnapshot
        {
            public CellRenderSnapshot(bool isPlayable, bool isOccupied, int tileId, bool isSelected)
            {
                IsPlayable = isPlayable;
                IsOccupied = isOccupied;
                TileId = tileId;
                IsSelected = isSelected;
            }

            public bool IsPlayable { get; }
            public bool IsOccupied { get; }
            public int TileId { get; }
            public bool IsSelected { get; }
        }

        [SerializeField] private RectTransform _cellRoot;
        [SerializeField] private BoardCellView _cellTemplate;
        [SerializeField] private Vector2 _cellSize = new(72f, 72f);
        [SerializeField] private Vector2 _cellSpacing = new(6f, 6f);
        [SerializeField] private bool _logDiffRefreshes;
        [SerializeField] private bool _logPresentationLifecycle;
        [SerializeField] private float _initialPopulateStepDelay = 0.012f;
        [SerializeField] private float _refillRowStepDelay = 0.018f;
        [SerializeField] private float _tileChangeWaveStepDelay = 0.01f;
        [SerializeField] private float _refillSpawnOffsetCells = 0.45f;
        [SerializeField] private float _swapPreviewDuration = 0.1f;
        [SerializeField] private float _swapPreviewOffsetCells = 0.22f;
        [SerializeField] private float _clearPreviewDuration = 0.06f;
        [SerializeField] private int _presentationHistoryLimit = 6;

        private readonly Dictionary<(int X, int Y), BoardCellView> _spawnedCells = new();
        private readonly Dictionary<(int X, int Y), CellRenderSnapshot> _lastRenderedStates = new();
        private readonly BoardPresentationPassContext _presentationPassContext = new();

        public event Action<BoardCoordinate> CellClicked;
        public event Action<BoardCoordinate, BoardCoordinate> SwipeRequested;
        public event Action PresentationCompleted;

        public BoardPresentationPassSummary LastPresentationPassSummary => _presentationPassContext.LastSummary;
        public IReadOnlyList<BoardPresentationPassSummary> PresentationHistory => _presentationPassContext.History;

        public string BuildPresentationDebugSummary()
        {
            var builder = new StringBuilder();
            builder.AppendLine("BoardView Presentation Settings");
            builder.AppendLine($"- Log Diff Refreshes: {_logDiffRefreshes}");
            builder.AppendLine($"- Log Presentation Lifecycle: {_logPresentationLifecycle}");
            builder.AppendLine($"- Cell Size: {_cellSize}");
            builder.AppendLine($"- Cell Spacing: {_cellSpacing}");
            builder.AppendLine($"- Initial Populate Step Delay: {_initialPopulateStepDelay}");
            builder.AppendLine($"- Refill Row Step Delay: {_refillRowStepDelay}");
            builder.AppendLine($"- Tile Change Wave Step Delay: {_tileChangeWaveStepDelay}");
            builder.AppendLine($"- Refill Spawn Offset Cells: {_refillSpawnOffsetCells}");
            builder.AppendLine($"- Swap Preview Duration: {_swapPreviewDuration}");
            builder.AppendLine($"- Swap Preview Offset Cells: {_swapPreviewOffsetCells}");
            builder.AppendLine($"- Clear Preview Duration: {_clearPreviewDuration}");
            builder.AppendLine($"- Presentation History Limit: {_presentationHistoryLimit}");
            builder.AppendLine($"- Spawned Cells: {_spawnedCells.Count}");
            builder.AppendLine($"- Active Presentation Token: {_presentationPassContext.Token}");
            builder.AppendLine($"- Pending Animated Cell Count: {_presentationPassContext.PendingAnimatedCellCount}");
            if (_presentationPassContext.LastSummary != null)
            {
                builder.Append(_presentationPassContext.LastSummary.BuildDebugString());
            }
            return builder.ToString();
        }

        public string BuildLastPresentationPassDebugSummary()
        {
            return _presentationPassContext.LastSummary != null
                ? _presentationPassContext.LastSummary.BuildDebugString()
                : "Last Presentation Pass\n- None recorded yet.";
        }

        public string BuildLastPresentationPassTestLogSnippet(string testId = "PB-XX", string title = "Presentation Observation")
        {
            return _presentationPassContext.LastSummary != null
                ? _presentationPassContext.LastSummary.BuildTestLogSnippet(testId, title)
                : $"### {testId} - {title}\n\n- Scene action: No presentation pass recorded yet.\n- Expected: \n- Actual: No presentation pass recorded yet.\n- Repro frequency: \n- Console evidence: none\n- Most likely layer: \n- Next fix idea: ";
        }

        public string BuildLastPresentationPassTestLogSnippetForLatestLabel()
        {
            if (_presentationPassContext.LastSummary == null)
            {
                return BuildLastPresentationPassTestLogSnippet();
            }

            var suggestedTestId = _presentationPassContext.LastSummary.Intent switch
            {
                BoardPresentationIntent.AcceptedSwapResolve => "PB-03",
                BoardPresentationIntent.RejectedSwapRefresh => "PB-02",
                BoardPresentationIntent.SelectionRefresh => "PB-01",
                _ => "PB-XX"
            };

            return _presentationPassContext.LastSummary.BuildTestLogSnippet(suggestedTestId, _presentationPassContext.LastSummary.Label);
        }

        public string BuildPresentationHistoryDebugSummary()
        {
            if (_presentationPassContext.History.Count == 0)
            {
                return "Presentation History\n- None recorded yet.";
            }

            var builder = new StringBuilder();
            builder.AppendLine("Presentation History");
            foreach (var summary in _presentationPassContext.History)
            {
                builder.Append(summary.BuildDebugString());
            }

            return builder.ToString();
        }

        public string BuildCompactPresentationHistoryDebugSummary()
        {
            if (_presentationPassContext.History.Count == 0)
            {
                return "Presentation History\n- None recorded yet.";
            }

            var builder = new StringBuilder();
            builder.AppendLine("Presentation History");
            foreach (var summary in _presentationPassContext.History)
            {
                builder.AppendLine(summary.BuildCompactDebugString());
            }

            return builder.ToString();
        }

        public void ClearPresentationHistory()
        {
            _presentationPassContext.History.Clear();
            _presentationPassContext.LastSummary = null;
        }

        public void Render(BoardState board, BoardPresentationRenderRequest request)
        {
            if (board == null || _cellRoot == null || _cellTemplate == null)
            {
                return;
            }

            EnsureGrid(board);
            var plan = CreatePresentationPlan();
            var passPlanner = CreatePresentationPassPlanner();

            var currentPresentationToken = _presentationPassContext.Token + 1;
            _presentationPassContext.ResetForNewPass(currentPresentationToken, Time.realtimeSinceStartup);
            var transitionCounts = _logPresentationLifecycle
                ? new Dictionary<BoardCellView.RefreshTransitionType, int>()
                : null;
            var phaseCounts = _logPresentationLifecycle
                ? new Dictionary<BoardCellPresentationPhaseType, int>()
                : null;

            var updatedCellCount = 0;
            foreach (var cell in board.GetAllCells())
            {
                if (!_spawnedCells.TryGetValue((cell.Coordinate.X, cell.Coordinate.Y), out var view))
                {
                    continue;
                }

                var isSelected = request.SelectedCoordinate.HasValue
                    && request.SelectedCoordinate.Value.X == cell.Coordinate.X
                    && request.SelectedCoordinate.Value.Y == cell.Coordinate.Y;
                var nextSnapshot = new CellRenderSnapshot(cell.IsPlayable, cell.IsOccupied, cell.TileId, isSelected);
                var hasPreviousSnapshot = _lastRenderedStates.TryGetValue((cell.Coordinate.X, cell.Coordinate.Y), out var previousSnapshot);

                if (hasPreviousSnapshot && previousSnapshot.Equals(nextSnapshot))
                {
                    continue;
                }

                var instruction = plan.BuildInstruction(
                    board,
                    cell.Coordinate,
                    hasPreviousSnapshot,
                    previousSnapshot.IsOccupied,
                    previousSnapshot.TileId,
                    previousSnapshot.IsSelected,
                    nextSnapshot.IsOccupied,
                    nextSnapshot.TileId,
                    nextSnapshot.IsSelected,
                    request);
                var transition = instruction.Transition;

                if (transitionCounts != null)
                {
                    if (!transitionCounts.ContainsKey(transition.Type))
                    {
                        transitionCounts[transition.Type] = 0;
                    }

                    transitionCounts[transition.Type]++;
                }

                if (phaseCounts != null)
                {
                    foreach (var phase in instruction.Phases)
                    {
                        if (!phaseCounts.ContainsKey(phase.Type))
                        {
                            phaseCounts[phase.Type] = 0;
                        }

                        phaseCounts[phase.Type]++;
                    }
                }

                var isAnimated = view.Render(
                    cell.IsPlayable,
                    cell.IsOccupied,
                    cell.TileId,
                    isSelected,
                    instruction,
                    () => HandleCellPresentationCompleted(currentPresentationToken));

                if (isAnimated)
                {
                    _presentationPassContext.PendingAnimatedCellCount++;
                }

                _lastRenderedStates[(cell.Coordinate.X, cell.Coordinate.Y)] = nextSnapshot;
                updatedCellCount++;
            }

            if (_logDiffRefreshes)
            {
                Debug.Log($"BoardView diff refresh updated {updatedCellCount} cells.", this);
            }

            var resolvedLabel = ResolvePresentationLabel(request);
            var clonedTransitionCounts = CloneCounts(transitionCounts);
            var clonedPhaseCounts = CloneCounts(phaseCounts);
            var passPlanningContext = new BoardPresentationPassPlanningContext(
                request.PresentationMode,
                request.PresentationIntent,
                request.PresentationStage,
                clonedTransitionCounts,
                clonedPhaseCounts,
                updatedCellCount,
                _presentationPassContext.PendingAnimatedCellCount);
            var passPlan = passPlanner.BuildPlan(passPlanningContext);
            if (_logPresentationLifecycle)
            {
                Debug.Log(
                    $"BoardView presentation pass {currentPresentationToken} started. Intent={request.PresentationIntent}, Stage={passPlan.Stage}, Steps={BuildStepSummary(passPlan.Steps)}, Label={resolvedLabel}, Mode={request.PresentationMode}, UpdatedCells={updatedCellCount}, AnimatedCells={_presentationPassContext.PendingAnimatedCellCount}, Transitions={BuildTransitionSummary(transitionCounts)}, Phases={BuildPhaseSummary(phaseCounts)}.",
                    this);
            }

            _presentationPassContext.LastSummary = new BoardPresentationPassSummary(
                currentPresentationToken,
                request.PresentationMode,
                resolvedLabel,
                request.PresentationIntent,
                passPlan.Stage,
                passPlan.Steps,
                updatedCellCount,
                _presentationPassContext.PendingAnimatedCellCount,
                _presentationPassContext.PendingAnimatedCellCount == 0,
                0f,
                clonedTransitionCounts,
                clonedPhaseCounts);
            RecordPresentationSummary(_presentationPassContext.LastSummary);

            if (_presentationPassContext.PendingAnimatedCellCount == 0)
            {
                if (_logPresentationLifecycle)
                {
                    Debug.Log($"BoardView presentation pass {currentPresentationToken} completed immediately.", this);
                }

                MarkPresentationSummaryCompleted(currentPresentationToken);
                PresentationCompleted?.Invoke();
            }
        }

        private void OnDestroy()
        {
            foreach (var view in _spawnedCells.Values)
            {
                if (view != null)
                {
                    view.Clicked -= HandleCellClicked;
                    view.SwipeRequested -= HandleSwipeRequested;
                }
            }
        }

        private BoardPresentationPlan CreatePresentationPlan()
        {
            return new BoardPresentationPlan(
                _cellSize,
                _initialPopulateStepDelay,
                _refillRowStepDelay,
                _tileChangeWaveStepDelay,
                _refillSpawnOffsetCells,
                _swapPreviewDuration,
                _swapPreviewOffsetCells,
                _clearPreviewDuration);
        }

        private static BoardPresentationPassPlanner CreatePresentationPassPlanner()
        {
            return new BoardPresentationPassPlanner();
        }

        private void EnsureGrid(BoardState board)
        {
            for (var y = 0; y < board.Height; y++)
            {
                for (var x = 0; x < board.Width; x++)
                {
                    if (_spawnedCells.ContainsKey((x, y)))
                    {
                        continue;
                    }

                    var instance = Instantiate(_cellTemplate, _cellRoot);
                    instance.gameObject.name = $"Cell_{x}_{y}";
                    instance.gameObject.SetActive(true);
                    instance.SetCoordinate(new BoardCoordinate(x, y));
                    instance.Clicked += HandleCellClicked;
                    instance.SwipeRequested += HandleSwipeRequested;

                    if (instance.TryGetComponent<RectTransform>(out var rectTransform))
                    {
                        rectTransform.anchorMin = new Vector2(0f, 0f);
                        rectTransform.anchorMax = new Vector2(0f, 0f);
                        rectTransform.pivot = new Vector2(0f, 0f);
                        rectTransform.sizeDelta = _cellSize;
                        rectTransform.anchoredPosition = new Vector2(
                            x * (_cellSize.x + _cellSpacing.x),
                            y * (_cellSize.y + _cellSpacing.y));
                        rectTransform.localScale = Vector3.one;
                    }

                    _spawnedCells[(x, y)] = instance;
                }
            }

            _cellTemplate.gameObject.SetActive(false);
        }

        private void HandleCellPresentationCompleted(int presentationToken)
        {
            if (presentationToken != _presentationPassContext.Token)
            {
                return;
            }

            _presentationPassContext.PendingAnimatedCellCount = Mathf.Max(0, _presentationPassContext.PendingAnimatedCellCount - 1);
            if (_presentationPassContext.PendingAnimatedCellCount == 0)
            {
                if (_logPresentationLifecycle)
                {
                    Debug.Log($"BoardView presentation pass {presentationToken} completed after animated cells settled.", this);
                }

                if (_presentationPassContext.LastSummary != null && _presentationPassContext.LastSummary.Token == presentationToken)
                {
                    MarkPresentationSummaryCompleted(presentationToken);
                }

                PresentationCompleted?.Invoke();
            }
        }

        private void MarkPresentationSummaryCompleted(int presentationToken)
        {
            if (_presentationPassContext.LastSummary == null || _presentationPassContext.LastSummary.Token != presentationToken)
            {
                return;
            }

            var durationSeconds = Mathf.Max(0f, Time.realtimeSinceStartup - _presentationPassContext.StartRealtime);
            _presentationPassContext.LastSummary = new BoardPresentationPassSummary(
                _presentationPassContext.LastSummary.Token,
                _presentationPassContext.LastSummary.Mode,
                _presentationPassContext.LastSummary.Label,
                _presentationPassContext.LastSummary.Intent,
                _presentationPassContext.LastSummary.Stage,
                _presentationPassContext.LastSummary.Steps,
                _presentationPassContext.LastSummary.UpdatedCellCount,
                _presentationPassContext.LastSummary.AnimatedCellCount,
                true,
                durationSeconds,
                _presentationPassContext.LastSummary.TransitionCounts,
                _presentationPassContext.LastSummary.PhaseCounts);
            ReplacePresentationSummary(_presentationPassContext.LastSummary);
        }

        private void RecordPresentationSummary(BoardPresentationPassSummary summary)
        {
            if (summary == null)
            {
                return;
            }

            _presentationPassContext.History.Add(summary);
            var historyLimit = Mathf.Max(1, _presentationHistoryLimit);
            while (_presentationPassContext.History.Count > historyLimit)
            {
                _presentationPassContext.History.RemoveAt(0);
            }
        }

        private void ReplacePresentationSummary(BoardPresentationPassSummary summary)
        {
            if (summary == null)
            {
                return;
            }

            for (var i = _presentationPassContext.History.Count - 1; i >= 0; i--)
            {
                if (_presentationPassContext.History[i].Token == summary.Token)
                {
                    _presentationPassContext.History[i] = summary;
                    return;
                }
            }
        }

        private static string BuildTransitionSummary(Dictionary<BoardCellView.RefreshTransitionType, int> transitionCounts)
        {
            if (transitionCounts == null || transitionCounts.Count == 0)
            {
                return "none";
            }

            var parts = new List<string>();
            foreach (var pair in transitionCounts)
            {
                parts.Add($"{pair.Key}:{pair.Value}");
            }

            return string.Join(", ", parts);
        }

        private static string BuildPhaseSummary(Dictionary<BoardCellPresentationPhaseType, int> phaseCounts)
        {
            if (phaseCounts == null || phaseCounts.Count == 0)
            {
                return "none";
            }

            var parts = new List<string>();
            foreach (var pair in phaseCounts)
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
                parts.Add(steps[i].Type.ToString());
            }

            return string.Join(" -> ", parts);
        }

        private static IReadOnlyDictionary<T, int> CloneCounts<T>(Dictionary<T, int> counts)
        {
            if (counts == null || counts.Count == 0)
            {
                return new Dictionary<T, int>();
            }

            return new Dictionary<T, int>(counts);
        }

        private static string ResolvePresentationLabel(BoardPresentationRenderRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.PresentationLabel))
            {
                return request.PresentationLabel;
            }

            if (request.PresentationMode == PresentationMode.ResolvedSequence && request.SwapPresentation.HasValue)
            {
                return $"Resolved Swap {request.SwapPresentation.Value.Source} -> {request.SwapPresentation.Value.Target}";
            }

            return request.PresentationMode == PresentationMode.Immediate
                ? "Immediate Refresh"
                : "Resolved Sequence";
        }

        private void HandleCellClicked(BoardCoordinate coordinate)
        {
            CellClicked?.Invoke(coordinate);
        }

        private void HandleSwipeRequested(BoardCoordinate source, BoardCoordinate target)
        {
            SwipeRequested?.Invoke(source, target);
        }
    }
}
