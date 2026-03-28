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

            BuildSession();
        }

        public void BuildSession()
        {
            _gameFlowController.BeginLevelSetup();
            _boardState = BoardBuilder.Build(_levelDefinition);
            _moveCounter = new MoveCounter(_levelDefinition.MoveLimit);
            _goalTracker = new GoalTracker(_levelDefinition.Goals);
            _hudPresenter?.ShowMoves(_moveCounter.RemainingMoves);
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
                return false;
            }

            _moveCounter.ConsumeAcceptedMove();
            var clearedCount = _cascadeResolver.Resolve(_boardState, result.MatchGroups);
            _goalTracker.RegisterAcceptedClear(clearedCount);
            _hudPresenter?.ShowMoves(_moveCounter.RemainingMoves);

            if (_goalTracker.AreAllGoalsComplete())
            {
                _gameFlowController.ShowResults();
            }
            else if (_moveCounter.RemainingMoves <= 0)
            {
                _gameFlowController.ShowResults();
            }

            Debug.Log($"Accepted swap {source} -> {target}, cleared {clearedCount} cells", this);
            return true;
        }
    }
}
