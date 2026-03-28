using System;
using UnityEngine;

namespace MatchJoy.Authoring
{
    [CreateAssetMenu(menuName = "MatchJoy/Level Definition", fileName = "LevelDefinition")]
    public sealed class LevelDefinition : ScriptableObject
    {
        [SerializeField] private string _levelId = "level_001";
        [SerializeField] private int _chapterIndex = 1;
        [SerializeField] private int _levelOrder = 1;
        [SerializeField] private int _boardWidth = 9;
        [SerializeField] private int _boardHeight = 9;
        [SerializeField] private int _moveLimit = 20;
        [SerializeField] private int[] _initialTiles = Array.Empty<int>();
        [SerializeField] private GoalDefinition[] _goals = Array.Empty<GoalDefinition>();
        [SerializeField] private int[] _moveBasedStarThresholds = new[] { 1, 3, 5 };

        public string LevelId => _levelId;
        public int ChapterIndex => _chapterIndex;
        public int LevelOrder => _levelOrder;
        public int BoardWidth => _boardWidth;
        public int BoardHeight => _boardHeight;
        public int MoveLimit => _moveLimit;
        public GoalDefinition[] Goals => _goals;
        public int[] MoveBasedStarThresholds => _moveBasedStarThresholds;

        public int GetInitialTileId(int x, int y)
        {
            var index = y * _boardWidth + x;
            if (_initialTiles == null || index < 0 || index >= _initialTiles.Length)
            {
                return -1;
            }

            return _initialTiles[index];
        }
    }
}
