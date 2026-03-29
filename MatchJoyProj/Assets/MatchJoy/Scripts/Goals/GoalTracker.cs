using MatchJoy.Authoring;

namespace MatchJoy.Goals
{
    public sealed class GoalTracker
    {
        private readonly GoalDefinition[] _goals;
        private readonly int[] _progressByGoalIndex;

        public GoalTracker(GoalDefinition[] goals)
        {
            _goals = goals;
            _progressByGoalIndex = new int[_goals.Length];
        }

        public void RegisterAcceptedClear(ClearResolutionSummary summary)
        {
            for (var i = 0; i < _goals.Length; i++)
            {
                var goal = _goals[i];
                switch (goal.GoalType)
                {
                    case GoalType.ClearJelly:
                        _progressByGoalIndex[i] += summary.ClearedCellCount;
                        break;
                    case GoalType.CollectTile:
                        if (summary.ClearedTileCounts.TryGetValue(goal.TargetTileId, out var collectedCount))
                        {
                            _progressByGoalIndex[i] += collectedCount;
                        }
                        break;
                    case GoalType.ReleaseFrozenIngredient:
                        // Sprint 1 keeps this goal family stubbed until obstacle/objective systems land.
                        break;
                }
            }
        }

        public GoalProgressSnapshot CreateSnapshot()
        {
            var completedGoals = 0;
            for (var i = 0; i < _goals.Length; i++)
            {
                if (_progressByGoalIndex[i] >= _goals[i].TargetCount)
                {
                    completedGoals++;
                }
            }

            return new GoalProgressSnapshot(completedGoals, _goals.Length, _progressByGoalIndex);
        }

        public bool AreAllGoalsComplete()
        {
            return _goals.Length > 0 && CreateSnapshot().CompletedGoals >= _goals.Length;
        }
    }
}
