using System;
using UnityEngine;

namespace MatchJoy.Flow
{
    public sealed class GameFlowController : MonoBehaviour
    {
        public GameFlowState CurrentState { get; private set; } = GameFlowState.Boot;

        public event Action<GameFlowState> StateChanged;

        public void Initialize()
        {
            TransitionTo(GameFlowState.ShellReady);
        }

        public bool IsGameplayInputAllowed()
        {
            return CurrentState == GameFlowState.LevelActive;
        }

        public void EnterChapterMap()
        {
            TransitionTo(GameFlowState.ChapterMapActive);
        }

        public void BeginLevelSetup()
        {
            TransitionTo(GameFlowState.LevelSessionSetup);
        }

        public void EnterLevelActive()
        {
            TransitionTo(GameFlowState.LevelActive);
        }

        public void PauseLevel()
        {
            if (CurrentState != GameFlowState.LevelActive)
            {
                return;
            }

            TransitionTo(GameFlowState.LevelPaused);
        }

        public void ResumeLevel()
        {
            if (CurrentState != GameFlowState.LevelPaused)
            {
                return;
            }

            TransitionTo(GameFlowState.LevelActive);
        }

        public void ShowResults()
        {
            TransitionTo(GameFlowState.ResultPresentation);
        }

        public void RetryLevel()
        {
            TransitionTo(GameFlowState.RetryTransition);
        }

        public void ReturnToMap()
        {
            TransitionTo(GameFlowState.ReturnToMapTransition);
        }

        private void TransitionTo(GameFlowState nextState)
        {
            if (CurrentState == nextState)
            {
                return;
            }

            CurrentState = nextState;
            StateChanged?.Invoke(CurrentState);
            Debug.Log($"Flow -> {CurrentState}", this);
        }
    }
}
