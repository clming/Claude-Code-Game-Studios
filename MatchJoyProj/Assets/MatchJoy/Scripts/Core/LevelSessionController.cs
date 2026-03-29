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

        private readonly SwapResolutionService _swapResolutionService = new();
        private readonly CascadeResolver _cascadeResolver = new();

        private BoardState _boardState;
        private MoveCounter _moveCounter;
        private GoalTracker _goalTracker;

        public BoardState BoardState => _boardState;

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
            }

            BuildSession();
        }

        private void OnDestroy()
        {
            if (_boardView != null)
            {
                _boardView.CellClicked -= HandleBoardCellClicked;
                _boardView.SwipeRequested -= HandleBoardSwipeRequested;
            }
        }

        public void BuildSession()
        {
            _gameFlowController.BeginLevelSetup();
            _boardState = BoardBuilder.Build(_levelDefinition);
            _moveCounter = new MoveCounter(_levelDefinition.MoveLimit);
            _goalTracker = new GoalTracker(_levelDefinition.Goals);
            _boardView?.Render(_boardState);
            _hudPresenter?.ShowMoves(_moveCounter.RemainingMoves);
            _resultsPresenter?.Hide();
            _gameFlowController.EnterLevelActive();
        }

        public bool TryHandleSwap(BoardCoordinate source, BoardCoordinate target)
        {
            if (_boardInputController != null)
            {
                _boardInputController.ClearSelection();
            }

            var result = _swapResolutionService.Resolve(_boardState, new SwapRequest(source, target));
            if (!result.Accepted)
            {
                Debug.Log($"Rejected swap {source} -> {target}", this);
                _boardView?.Render(_boardState);
                return false;
            }

            _moveCounter.ConsumeAcceptedMove();
            var clearSummary = _cascadeResolver.Resolve(_boardState, result.MatchGroups);
            _goalTracker.RegisterAcceptedClear(clearSummary);
            _boardView?.Render(_boardState);
            _hudPresenter?.ShowMoves(_moveCounter.RemainingMoves);

            if (_goalTracker.AreAllGoalsComplete())
            {
                _gameFlowController.ShowResults();
                _resultsPresenter?.ShowVictory(_moveCounter.RemainingMoves);
            }
            else if (_moveCounter.RemainingMoves <= 0)
            {
                _gameFlowController.ShowResults();
                _resultsPresenter?.ShowFailure();
            }

            Debug.Log($"Accepted swap {source} -> {target}, cleared {clearSummary.ClearedCellCount} cells", this);
            return true;
        }

        private void HandleBoardCellClicked(BoardCoordinate coordinate)
        {
            if (_boardInputController == null || _boardState == null)
            {
                return;
            }

            if (_boardInputController.HasSelection)
            {
                if (_boardInputController.TryBuildSwap(coordinate, out var request))
                {
                    TryHandleSwap(request.Source, request.Target);
                    _boardView?.Render(_boardState);
                    return;
                }

                _boardInputController.TrySelect(coordinate);
                _boardView?.Render(_boardState, _boardInputController.SelectedCoordinate);
                return;
            }

            if (_boardInputController.TrySelect(coordinate))
            {
                _boardView?.Render(_boardState, _boardInputController.SelectedCoordinate);
            }
        }

        private void HandleBoardSwipeRequested(BoardCoordinate source, BoardCoordinate target)
        {
            if (_boardState == null)
            {
                return;
            }

            TryHandleSwap(source, target);
        }
    }
}
