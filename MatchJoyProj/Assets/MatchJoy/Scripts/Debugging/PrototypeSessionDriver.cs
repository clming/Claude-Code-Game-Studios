using MatchJoy.Board;
using MatchJoy.Core;
using MatchJoy.UI;
using UnityEngine;

namespace MatchJoy.Debugging
{
    public sealed class PrototypeSessionDriver : MonoBehaviour
    {
        [SerializeField] private LevelSessionController _levelSessionController;
        [SerializeField] private bool _logBoardOnStart = true;
        [SerializeField] private Vector2Int _testSwapSource = new(0, 0);
        [SerializeField] private Vector2Int _testSwapTarget = new(1, 0);
        [SerializeField] private Vector2Int _secondTestSwapSource = new(1, 0);
        [SerializeField] private Vector2Int _secondTestSwapTarget = new(2, 0);
        [SerializeField] private BoardView _boardView;

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

        [ContextMenu("Rebuild Session")]
        public void RebuildSession()
        {
            if (_levelSessionController == null)
            {
                Debug.LogWarning("Level session controller is not assigned.", this);
                return;
            }

            _levelSessionController.BuildSession();
            if (_levelSessionController.BoardState != null)
            {
                BoardDebugPrinter.LogBoard(_levelSessionController.BoardState, this, "Board After Session Rebuild");
            }
        }

        [ContextMenu("Run Test Swap")]
        public void RunTestSwap()
        {
            TryRunSwap(_testSwapSource, _testSwapTarget, "Test Swap");
        }

        [ContextMenu("Run Second Test Swap")]
        public void RunSecondTestSwap()
        {
            TryRunSwap(_secondTestSwapSource, _secondTestSwapTarget, "Second Test Swap");
        }

        [ContextMenu("Run Test Swap Then Immediate Second Swap")]
        public void RunSwapSettlementGateCheck()
        {
            if (!TryEnsureSessionReady())
            {
                return;
            }

            var firstSource = new BoardCoordinate(_testSwapSource.x, _testSwapSource.y);
            var firstTarget = new BoardCoordinate(_testSwapTarget.x, _testSwapTarget.y);
            var secondSource = new BoardCoordinate(_secondTestSwapSource.x, _secondTestSwapSource.y);
            var secondTarget = new BoardCoordinate(_secondTestSwapTarget.x, _secondTestSwapTarget.y);

            var firstAccepted = _levelSessionController.TryHandleSwap(firstSource, firstTarget);
            Debug.Log($"Settlement gate check: first swap accepted={firstAccepted}.", this);

            var secondAccepted = _levelSessionController.TryHandleSwap(secondSource, secondTarget);
            Debug.Log($"Settlement gate check: immediate second swap accepted={secondAccepted}.", this);

            if (_levelSessionController.BoardState != null)
            {
                BoardDebugPrinter.LogBoard(
                    _levelSessionController.BoardState,
                    this,
                    "Board After Settlement Gate Check");
            }
        }

        [ContextMenu("Log Presentation Bridge Summary")]
        public void LogPresentationBridgeSummary()
        {
            if (_levelSessionController == null)
            {
                Debug.LogWarning("Level session controller is not assigned.", this);
                return;
            }

            var summary = $"Presentation Bridge Summary\n"
                + $"- Session Ready: {_levelSessionController.BoardState != null}\n"
                + $"- Awaiting Presentation Settlement: {_levelSessionController.IsAwaitingPresentationSettlement}\n"
                + $"- Primary Test Swap: {_testSwapSource} -> {_testSwapTarget}\n"
                + $"- Secondary Test Swap: {_secondTestSwapSource} -> {_secondTestSwapTarget}";

            Debug.Log(summary, this);

            if (_boardView != null)
            {
                Debug.Log(_boardView.BuildPresentationDebugSummary(), _boardView);
                if (_boardView.LastPresentationPassSummary != null)
                {
                    Debug.Log(_boardView.LastPresentationPassSummary.BuildDebugString(), _boardView);
                }
            }

            if (_levelSessionController.BoardState != null)
            {
                BoardDebugPrinter.LogBoard(_levelSessionController.BoardState, this, "Board At Presentation Summary");
            }
        }

        [ContextMenu("Log Last Presentation Pass")]
        public void LogLastPresentationPass()
        {
            if (_boardView == null)
            {
                Debug.LogWarning("BoardView is not assigned.", this);
                return;
            }

            Debug.Log(_boardView.BuildLastPresentationPassDebugSummary(), _boardView);
        }

        [ContextMenu("Log Presentation History")]
        public void LogPresentationHistory()
        {
            if (_boardView == null)
            {
                Debug.LogWarning("BoardView is not assigned.", this);
                return;
            }

            Debug.Log(_boardView.BuildPresentationHistoryDebugSummary(), _boardView);
        }

        [ContextMenu("Log Compact Presentation History")]
        public void LogCompactPresentationHistory()
        {
            if (_boardView == null)
            {
                Debug.LogWarning("BoardView is not assigned.", this);
                return;
            }

            Debug.Log(_boardView.BuildCompactPresentationHistoryDebugSummary(), _boardView);
        }

        [ContextMenu("Clear Presentation History")]
        public void ClearPresentationHistory()
        {
            if (_boardView == null)
            {
                Debug.LogWarning("BoardView is not assigned.", this);
                return;
            }

            _boardView.ClearPresentationHistory();
            Debug.Log("Cleared BoardView presentation history.", _boardView);
        }

        [ContextMenu("Log Last Pass Test Log Snippet")]
        public void LogLastPassTestLogSnippet()
        {
            if (_boardView == null)
            {
                Debug.LogWarning("BoardView is not assigned.", this);
                return;
            }

            Debug.Log(_boardView.BuildLastPresentationPassTestLogSnippet(), _boardView);
        }

        [ContextMenu("Run Presentation Validation Snapshot")]
        public void RunPresentationValidationSnapshot()
        {
            if (_levelSessionController == null)
            {
                Debug.LogWarning("Level session controller is not assigned.", this);
                return;
            }

            if (_boardView == null)
            {
                Debug.LogWarning("BoardView is not assigned.", this);
                return;
            }

            Debug.Log("Presentation Validation Snapshot", this);
            Debug.Log(_boardView.BuildPresentationDebugSummary(), _boardView);
            Debug.Log(_boardView.BuildCompactPresentationHistoryDebugSummary(), _boardView);
            Debug.Log(_boardView.BuildLastPresentationPassTestLogSnippet(), _boardView);

            if (_levelSessionController.BoardState != null)
            {
                BoardDebugPrinter.LogBoard(_levelSessionController.BoardState, this, "Board At Validation Snapshot");
            }
        }

        [ContextMenu("Run Validation Snapshot After Test Swap")]
        public void RunValidationSnapshotAfterTestSwap()
        {
            if (!TryEnsureSessionReady())
            {
                return;
            }

            if (_boardView == null)
            {
                Debug.LogWarning("BoardView is not assigned.", this);
                return;
            }

            _boardView.ClearPresentationHistory();
            Debug.Log("Cleared BoardView presentation history before validation test swap.", _boardView);

            TryRunSwap(_testSwapSource, _testSwapTarget, "Validation Test Swap");
            RunPresentationValidationSnapshot();
        }

        [ContextMenu("Run Validation Snapshot After Second Test Swap")]
        public void RunValidationSnapshotAfterSecondTestSwap()
        {
            if (!TryEnsureSessionReady())
            {
                return;
            }

            if (_boardView == null)
            {
                Debug.LogWarning("BoardView is not assigned.", this);
                return;
            }

            _boardView.ClearPresentationHistory();
            Debug.Log("Cleared BoardView presentation history before validation second test swap.", _boardView);

            TryRunSwap(_secondTestSwapSource, _secondTestSwapTarget, "Validation Second Test Swap");
            RunPresentationValidationSnapshot();
        }

        [ContextMenu("Run Settlement Gate Validation Snapshot")]
        public void RunSettlementGateValidationSnapshot()
        {
            if (!TryEnsureSessionReady())
            {
                return;
            }

            if (_boardView == null)
            {
                Debug.LogWarning("BoardView is not assigned.", this);
                return;
            }

            _boardView.ClearPresentationHistory();
            Debug.Log("Cleared BoardView presentation history before settlement gate validation.", _boardView);

            RunSwapSettlementGateCheck();
            Debug.Log(_boardView.BuildCompactPresentationHistoryDebugSummary(), _boardView);
            Debug.Log(_boardView.BuildLastPresentationPassTestLogSnippet("PB-08", "Settlement Gate Validation"), _boardView);

            if (_levelSessionController.BoardState != null)
            {
                BoardDebugPrinter.LogBoard(_levelSessionController.BoardState, this, "Board At Settlement Gate Validation Snapshot");
            }
        }

        [ContextMenu("Run Core Presentation Validation Suite")]
        public void RunCorePresentationValidationSuite()
        {
            if (!TryEnsureSessionReady())
            {
                return;
            }

            if (_boardView == null)
            {
                Debug.LogWarning("BoardView is not assigned.", this);
                return;
            }

            Debug.Log("Core Presentation Validation Suite", this);

            _boardView.ClearPresentationHistory();
            Debug.Log("Cleared BoardView presentation history before core validation suite.", _boardView);

            TryRunSwap(_testSwapSource, _testSwapTarget, "Suite Primary Swap");
            Debug.Log(_boardView.BuildCompactPresentationHistoryDebugSummary(), _boardView);
            Debug.Log(_boardView.BuildLastPresentationPassTestLogSnippet("PB-03", "Core Suite Primary Swap"), _boardView);

            _boardView.ClearPresentationHistory();
            Debug.Log("Cleared BoardView presentation history before settlement gate portion of core suite.", _boardView);

            RunSwapSettlementGateCheck();
            Debug.Log(_boardView.BuildCompactPresentationHistoryDebugSummary(), _boardView);
            Debug.Log(_boardView.BuildLastPresentationPassTestLogSnippet("PB-08", "Core Suite Settlement Gate"), _boardView);

            if (_levelSessionController.BoardState != null)
            {
                BoardDebugPrinter.LogBoard(_levelSessionController.BoardState, this, "Board After Core Presentation Validation Suite");
            }
        }

        [ContextMenu("Log Suggested Last Pass Test Log Snippet")]
        public void LogSuggestedLastPassTestLogSnippet()
        {
            if (_boardView == null)
            {
                Debug.LogWarning("BoardView is not assigned.", this);
                return;
            }

            Debug.Log(_boardView.BuildLastPresentationPassTestLogSnippetForLatestLabel(), _boardView);
        }

        private void TryRunSwap(Vector2Int sourceVector, Vector2Int targetVector, string label)
        {
            if (!TryEnsureSessionReady())
            {
                return;
            }

            var source = new BoardCoordinate(sourceVector.x, sourceVector.y);
            var target = new BoardCoordinate(targetVector.x, targetVector.y);
            var accepted = _levelSessionController.TryHandleSwap(source, target);

            Debug.Log($"{label}: {source} -> {target}, accepted={accepted}.", this);
            BoardDebugPrinter.LogBoard(
                _levelSessionController.BoardState,
                this,
                accepted ? $"{label} Board After Accepted Swap" : $"{label} Board After Rejected Swap");
        }

        private bool TryEnsureSessionReady()
        {
            if (_levelSessionController == null || _levelSessionController.BoardState == null)
            {
                Debug.LogWarning("Level session is not ready.", this);
                return false;
            }

            return true;
        }
    }
}
