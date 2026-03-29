using System.Collections.Generic;

namespace MatchJoy.Goals
{
    public sealed class ClearResolutionSummary
    {
        public ClearResolutionSummary(int clearedCellCount, Dictionary<int, int> clearedTileCounts)
        {
            ClearedCellCount = clearedCellCount;
            ClearedTileCounts = clearedTileCounts;
        }

        public int ClearedCellCount { get; }
        public Dictionary<int, int> ClearedTileCounts { get; }
    }
}
