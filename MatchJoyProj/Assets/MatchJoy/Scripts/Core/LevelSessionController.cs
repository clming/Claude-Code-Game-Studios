using MatchJoy.Authoring;
using MatchJoy.Board;
using MatchJoy.Flow;
using MatchJoy.Goals;
using MatchJoy.Input;
using MatchJoy.UI;
using UnityEngine;

namespace MatchJoy.Core
{
    public sealed class LevelSessionController : MonoBehaviour
    {
        [SerializeField] private GameFlowController _gameFlowController;
        [SerializeField] private LevelDefinition _levelDefinition;
        [SerializeField] private HudPresenter _hudPresenter;
        [SerializeField] private ResultsPresenter _resultsPresenter;
        [SerializeField] private BoardView _boardView;
        [SerializeField] private BoardInputController _boardInputController;
        [SerializeField] private bool _logPresentationSettlement;

        private readonly SwapResolutionService _swapResolutionService = new();
        private readonly CascadeResolver _cascadeResolver = new();

        private BoardState _boardState;
        private MoveCounter _moveCounter;
        private GoalTracker _goalTracker;
        private bool _isAwaitingPresentationSettlement;

        public BoardState BoardState => _boardState;
        public bool IsAwaitingPresentationSettlement => _isAwaitingPresentationSettlement;

        private void Start()
        {
            if (_levelDefinition == null || _gameFlowController == null)
            {
                Debug.LogWarning("LevelSessionController is missing required references.", this);
                return;
            }

            if (_boardView != null)
            {
                _boardView.CellClicked += HandleBoardCellClicked;
                _boardView.SwipeRequested += HandleBoardSwipeRequested;
                _boardView.PresentationCompleted += HandleBoardPresentationCompleted;
            }

            BuildSession();
        }

        private void OnDestroy()
        {
            if (_boardView != null)
            {
                _boardView.CellClicked -= HandleBoardCellClicked;
                _boardView.SwipeRequested -= HandleBoardSwipeRequested;
                _boardView.PresentationCompleted -= HandleBoardPresentationCompleted;
            }
        }

        public void BuildSession()
        {
            _isAwaitingPresentationSettlement = false;

            _gameFlowController.BeginLevelSetup();
            _boardState = BoardBuilder.Build(_levelDefinition);
            _moveCounter = new MoveCounter(_levelDefinition.MoveLimit);
            _goalTracker = new GoalTracker(_levelDefinition.Goals);
            RefreshBoardSessionContext();
            _boardView?.Render(_boardState, BoardPresentationRenderRequest.Immediate(null, "Initial Session Build", BoardPresentationIntent.InitialBuild));
            _hudPresenter?.ShowLevelStatus(_levelDefinition, _moveCounter.RemainingMoves, _goalTracker.CreateSnapshot());
            _resultsPresenter?.Hide();
            _gameFlowController.EnterLevelActive();
        }

        public bool TryHandleSwap(BoardCoordinate source, BoardCoordinate target)
        {
            if (_isAwaitingPresentationSettlement)
            {
                Debug.Log($"Ignored swap {source} -> {target} while board presentation is still settling.", this);
                return false;
            }

            if (_boardInputController != null)
            {
                _boardInputController.ClearSelection();
            }

            _isAwaitingPresentationSettlement = false;

            var result = _swapResolutionService.Resolve(_boardState, new SwapRequest(source, target));
            if (!result.Accepted)
            {
                Debug.Log($"Rejected swap {source} -> {target}", this);
                RefreshBoardSessionContext();
                _boardView?.Render(_boardState, BoardPresentationRenderRequest.Immediate(null, $"Rejected Swap Refresh {source} -> {target}", BoardPresentationIntent.RejectedSwapRefresh));
                return false;
            }

            _moveCounter.ConsumeAcceptedMove();
            var clearSummary = _cascadeResolver.Resolve(_boardState, result.MatchGroups);
            _goalTracker.RegisterAcceptedClear(clearSummary);

            _isAwaitingPresentationSettlement = true;
            _gameFlowController?.EnterLevelPresentationSettling();
            if (_logPresentationSettlement)
            {
                Debug.Log($"Board presentation settlement started for accepted swap {source} -> {target}.", this);
            }

            RefreshBoardSessionContext();
            _boardView?.Render(
                _boardState,
                BoardPresentationRenderRequest.ResolvedSwap(source, target, $"Accepted Swap Resolve {source} -> {target}", BoardPresentationIntent.AcceptedSwapResolve));

            Debug.Log($"Accepted swap {source} -> {target}, cleared {clearSummary.ClearedCellCount} cells", this);
            return true;
        }

        private void HandleBoardPresentationCompleted()
        {
            if (!_isAwaitingPresentationSettlement)
            {
                return;
            }

            _isAwaitingPresentationSettlement = false;
            if (_logPresentationSettlement)
            {
                Debug.Log("Board presentation settlement completed. Applying HUD and result updates.", this);
            }

            _hudPresenter?.ShowLevelStatus(_levelDefinition, _moveCounter.RemainingMoves, _goalTracker.CreateSnapshot());
            RefreshBoardSessionContext();

            if (_goalTracker.AreAllGoalsComplete())
            {
                _gameFlowController.ShowResults();
                _resultsPresenter?.ShowVictory(_moveCounter.RemainingMoves, _levelDefinition);
            }
            else if (_moveCounter.RemainingMoves <= 0)
            {
                _gameFlowController.ShowResults();
                _resultsPresenter?.ShowFailure(_levelDefinition);
            }
            else
            {
                _gameFlowController?.EnterLevelActive();
            }
        }

        private void HandleBoardCellClicked(BoardCoordinate coordinate)
        {
            if (_isAwaitingPresentationSettlement)
            {
                return;
            }

            if (_boardInputController == null || _boardState == null)
            {
                return;
            }

            if (_boardInputController.HasSelection)
            {
                if (_boardInputController.TryBuildSwap(coordinate, out var request))
                {
                    TryHandleSwap(request.Source, request.Target);
                    return;
                }

                _boardInputController.TrySelect(coordinate);
                RefreshBoardSessionContext();
                _boardView?.Render(
                    _boardState,
                    BoardPresentationRenderRequest.Immediate(
                        _boardInputController.SelectedCoordinate,
                        $"Selection Refresh {coordinate}",
                        BoardPresentationIntent.SelectionRefresh));
                return;
            }

            if (_boardInputController.TrySelect(coordinate))
            {
                RefreshBoardSessionContext();
                _boardView?.Render(
                    _boardState,
                    BoardPresentationRenderRequest.Immediate(
                        _boardInputController.SelectedCoordinate,
                        $"Selection Refresh {coordinate}",
                        BoardPresentationIntent.SelectionRefresh));
            }
        }

        private void HandleBoardSwipeRequested(BoardCoordinate source, BoardCoordinate target)
        {
            if (_isAwaitingPresentationSettlement)
            {
                return;
            }

            if (_boardState == null)
            {
                return;
            }

            TryHandleSwap(source, target);
        }

        private void RefreshBoardSessionContext()
        {
            _boardView?.ShowSessionContext(_levelDefinition, _moveCounter?.RemainingMoves ?? 0, _goalTracker.CreateSnapshot());
        }
    }
}
