using MatchJoy.Board;
using MatchJoy.Flow;
using UnityEngine;

namespace MatchJoy.Input
{
    public sealed class BoardInputController : MonoBehaviour
    {
        [SerializeField] private GameFlowController _gameFlowController;

        private readonly TileSelectionState _selectionState = new();

        public bool HasSelection => _selectionState.HasSelection;
        public BoardCoordinate SelectedCoordinate => _selectionState.SelectedCoordinate;

        public bool TrySelect(BoardCoordinate coordinate)
        {
            if (_gameFlowController == null || !_gameFlowController.IsGameplayInputAllowed())
            {
                return false;
            }

            _selectionState.Select(coordinate);
            return true;
        }

        public bool TryBuildSwap(BoardCoordinate target, out SwapRequest request)
        {
            request = default;
            if (!_selectionState.HasSelection)
            {
                return false;
            }

            var dx = Mathf.Abs(target.X - _selectionState.SelectedCoordinate.X);
            var dy = Mathf.Abs(target.Y - _selectionState.SelectedCoordinate.Y);
            if (dx + dy != 1)
            {
                return false;
            }

            request = new SwapRequest(_selectionState.SelectedCoordinate, target);
            _selectionState.Clear();
            return true;
        }

        public void ClearSelection()
        {
            _selectionState.Clear();
        }
    }
}
