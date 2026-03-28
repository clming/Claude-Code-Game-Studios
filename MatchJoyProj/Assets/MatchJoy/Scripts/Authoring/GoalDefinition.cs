using System;
using UnityEngine;

namespace MatchJoy.Authoring
{
    [Serializable]
    public sealed class GoalDefinition
    {
        [SerializeField] private GoalType _goalType;
        [SerializeField] private int _targetCount = 1;
        [SerializeField] private int _targetTileId = -1;

        public GoalType GoalType => _goalType;
        public int TargetCount => _targetCount;
        public int TargetTileId => _targetTileId;
    }
}
