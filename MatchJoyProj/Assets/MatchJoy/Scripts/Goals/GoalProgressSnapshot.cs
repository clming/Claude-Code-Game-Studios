namespace MatchJoy.Goals
{
    public readonly struct GoalProgressSnapshot
    {
        public GoalProgressSnapshot(int completedGoals, int totalGoals)
        {
            CompletedGoals = completedGoals;
            TotalGoals = totalGoals;
        }

        public int CompletedGoals { get; }
        public int TotalGoals { get; }
    }
}
