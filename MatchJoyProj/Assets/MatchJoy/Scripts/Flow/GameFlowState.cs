namespace MatchJoy.Flow
{
    public enum GameFlowState
    {
        Boot = 0,
        ShellReady = 1,
        ChapterMapActive = 2,
        LevelSessionSetup = 3,
        LevelActive = 4,
        LevelPaused = 5,
        ResultPresentation = 6,
        RetryTransition = 7,
        ReturnToMapTransition = 8,
    }
}
