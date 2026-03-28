using MatchJoy.Board;
using MatchJoy.Core;
using UnityEngine;

namespace MatchJoy.Debugging
{
    public sealed class PrototypeSessionDriver : MonoBehaviour
    {
        [SerializeField] private LevelSessionController _levelSessionController;
        [SerializeField] private bool _logBoardOnStart = true;
        [SerializeField] private Vector2Int _testSwapSource = new(0, 0);
        [SerializeField] private Vector2Int _testSwapTarget = new(1, 0);

        private void Start()
        {
            if (_levelSessionController == null)
            {
                Debug.LogWarning("PrototypeSessionDriver is missing LevelSessionController reference.", this);
                return;
            }

            if (_logBoardOnStart && _levelSessionController.BoardState != null)
            {
                BoardDebugPrinter.LogBoard(_levelSessionController.BoardState, this, "Initial Board");
            }
        }

        [ContextMenu("Run Test Swap")]
        public void RunTestSwap()
        {
            if (_levelSessionController == null || _levelSessionController.BoardState == null)
            {
                Debug.LogWarning("Level session is not ready.", this);
                return;
            }

            var source = new BoardCoordinate(_testSwapSource.x, _testSwapSource.y);
            var target = new BoardCoordinate(_testSwapTarget.x, _testSwapTarget.y);
            var accepted = _levelSessionController.TryHandleSwap(source, target);
            BoardDebugPrinter.LogBoard(_levelSessionController.BoardState, this, accepted ? "Board After Accepted Swap" : "Board After Rejected Swap");
        }
    }
}
