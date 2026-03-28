using MatchJoy.Authoring;

namespace MatchJoy.Goals
{
    public sealed class GoalTracker
    {
        private readonly GoalDefinition[] _goals;
        private int _progressCount;

        public GoalTracker(GoalDefinition[] goals)
        {
            _goals = goals;
        }

        public void RegisterAcceptedClear(int clearedCellCount)
        {
            _progressCount += clearedCellCount;
        }

        public GoalProgressSnapshot CreateSnapshot()
        {
            var completedGoals = 0;
            foreach (var goal in _goals)
            {
                if (_progressCount >= goal.TargetCount)
                {
                    completedGoals++;
                }
            }

            return new GoalProgressSnapshot(completedGoals, _goals.Length);
        }

        public bool AreAllGoalsComplete()
        {
            return _goals.Length > 0 && CreateSnapshot().CompletedGoals >= _goals.Length;
        }
    }
}
