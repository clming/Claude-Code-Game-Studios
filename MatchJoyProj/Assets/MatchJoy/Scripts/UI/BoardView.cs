using System;
using System.Collections.Generic;
using MatchJoy.Board;
using UnityEngine;

namespace MatchJoy.UI
{
    public sealed class BoardView : MonoBehaviour
    {
        private readonly struct CellRenderSnapshot
        {
            public CellRenderSnapshot(bool isPlayable, bool isOccupied, int tileId, bool isSelected)
            {
                IsPlayable = isPlayable;
                IsOccupied = isOccupied;
                TileId = tileId;
                IsSelected = isSelected;
            }

            public bool IsPlayable { get; }
            public bool IsOccupied { get; }
            public int TileId { get; }
            public bool IsSelected { get; }
        }

        [SerializeField] private RectTransform _cellRoot;
        [SerializeField] private BoardCellView _cellTemplate;
        [SerializeField] private Vector2 _cellSize = new(72f, 72f);
        [SerializeField] private Vector2 _cellSpacing = new(6f, 6f);
        [SerializeField] private bool _logDiffRefreshes;

        private readonly Dictionary<(int X, int Y), BoardCellView> _spawnedCells = new();
        private readonly Dictionary<(int X, int Y), CellRenderSnapshot> _lastRenderedStates = new();

        public event Action<BoardCoordinate> CellClicked;
        public event Action<BoardCoordinate, BoardCoordinate> SwipeRequested;

        public void Render(BoardState board, BoardCoordinate? selectedCoordinate = null)
        {
            if (board == null || _cellRoot == null || _cellTemplate == null)
            {
                return;
            }

            EnsureGrid(board);

            var updatedCellCount = 0;
            foreach (var cell in board.GetAllCells())
            {
                if (_spawnedCells.TryGetValue((cell.Coordinate.X, cell.Coordinate.Y), out var view))
                {
                    var isSelected = selectedCoordinate.HasValue
                        && selectedCoordinate.Value.X == cell.Coordinate.X
                        && selectedCoordinate.Value.Y == cell.Coordinate.Y;
                    var nextSnapshot = new CellRenderSnapshot(cell.IsPlayable, cell.IsOccupied, cell.TileId, isSelected);

                    if (_lastRenderedStates.TryGetValue((cell.Coordinate.X, cell.Coordinate.Y), out var previousSnapshot)
                        && previousSnapshot.Equals(nextSnapshot))
                    {
                        continue;
                    }

                    view.Render(cell.IsPlayable, cell.IsOccupied, cell.TileId, isSelected);
                    _lastRenderedStates[(cell.Coordinate.X, cell.Coordinate.Y)] = nextSnapshot;
                    updatedCellCount++;
                }
            }

            if (_logDiffRefreshes)
            {
                Debug.Log($"BoardView diff refresh updated {updatedCellCount} cells.", this);
            }
        }

        private void OnDestroy()
        {
            foreach (var view in _spawnedCells.Values)
            {
                if (view != null)
                {
                    view.Clicked -= HandleCellClicked;
                    view.SwipeRequested -= HandleSwipeRequested;
                }
            }
        }

        private void EnsureGrid(BoardState board)
        {
            for (var y = 0; y < board.Height; y++)
            {
                for (var x = 0; x < board.Width; x++)
                {
                    if (_spawnedCells.ContainsKey((x, y)))
                    {
                        continue;
                    }

                    var instance = Instantiate(_cellTemplate, _cellRoot);
                    instance.gameObject.name = $"Cell_{x}_{y}";
                    instance.gameObject.SetActive(true);
                    instance.SetCoordinate(new BoardCoordinate(x, y));
                    instance.Clicked += HandleCellClicked;
                    instance.SwipeRequested += HandleSwipeRequested;

                    if (instance.TryGetComponent<RectTransform>(out var rectTransform))
                    {
                        rectTransform.anchorMin = new Vector2(0f, 0f);
                        rectTransform.anchorMax = new Vector2(0f, 0f);
                        rectTransform.pivot = new Vector2(0f, 0f);
                        rectTransform.sizeDelta = _cellSize;
                        rectTransform.anchoredPosition = new Vector2(
                            x * (_cellSize.x + _cellSpacing.x),
                            y * (_cellSize.y + _cellSpacing.y));
                        rectTransform.localScale = Vector3.one;
                    }

                    _spawnedCells[(x, y)] = instance;
                }
            }

            _cellTemplate.gameObject.SetActive(false);
        }

        private void HandleCellClicked(BoardCoordinate coordinate)
        {
            CellClicked?.Invoke(coordinate);
        }

        private void HandleSwipeRequested(BoardCoordinate source, BoardCoordinate target)
        {
            SwipeRequested?.Invoke(source, target);
        }
    }
}
