namespace MatchJoy.Flow
{
    public enum GameFlowState
    {
        Boot = 0,
        ShellReady = 1,
        ChapterMapActive = 2,
        LevelSessionSetup = 3,
        LevelActive = 4,
        LevelPresentationSettling = 5,
        LevelPaused = 6,
        ResultPresentation = 7,
        RetryTransition = 8,
        ReturnToMapTransition = 9,
    }
}
