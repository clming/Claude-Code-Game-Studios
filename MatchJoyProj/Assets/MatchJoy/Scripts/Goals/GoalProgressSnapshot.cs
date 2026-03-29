namespace MatchJoy.Goals
{
    public readonly struct GoalProgressSnapshot
    {
        public GoalProgressSnapshot(int completedGoals, int totalGoals, int[] currentProgress)
        {
            CompletedGoals = completedGoals;
            TotalGoals = totalGoals;
            CurrentProgress = currentProgress;
        }

        public int CompletedGoals { get; }
        public int TotalGoals { get; }
        public int[] CurrentProgress { get; }
    }
}
