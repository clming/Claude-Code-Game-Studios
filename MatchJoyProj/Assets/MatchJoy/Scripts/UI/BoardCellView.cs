using System;
using MatchJoy.Board;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MatchJoy.UI
{
    public sealed class BoardCellView : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private const float SwipeThreshold = 24f;

        [SerializeField] private Image _background;
        [SerializeField] private Text _tileLabel;

        private bool _isPlayable;
        private BoardCoordinate _coordinate;
        private Vector2 _dragStartPosition;
        private bool _hasTriggeredSwipe;

        public event Action<BoardCoordinate> Clicked;
        public event Action<BoardCoordinate, BoardCoordinate> SwipeRequested;

        public void SetCoordinate(BoardCoordinate coordinate)
        {
            _coordinate = coordinate;
        }

        public void Render(bool isPlayable, bool isOccupied, int tileId, bool isSelected)
        {
            _isPlayable = isPlayable;

            if (_background != null)
            {
                _background.color = isSelected
                    ? new Color(1f, 0.92f, 0.35f, 1f)
                    : !isPlayable
                        ? new Color(0.15f, 0.15f, 0.15f, 0.75f)
                        : isOccupied
                            ? Color.white
                            : new Color(1f, 1f, 1f, 0.2f);
            }

            if (_tileLabel != null)
            {
                _tileLabel.text = isOccupied ? tileId.ToString() : string.Empty;
                _tileLabel.color = isPlayable ? Color.black : Color.clear;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isPlayable || _hasTriggeredSwipe)
            {
                return;
            }

            Clicked?.Invoke(_coordinate);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!_isPlayable)
            {
                return;
            }

            _dragStartPosition = eventData.position;
            _hasTriggeredSwipe = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isPlayable || _hasTriggeredSwipe)
            {
                return;
            }

            var delta = eventData.position - _dragStartPosition;
            if (delta.magnitude < SwipeThreshold)
            {
                return;
            }

            var direction = Mathf.Abs(delta.x) >= Mathf.Abs(delta.y)
                ? new Vector2Int(delta.x >= 0f ? 1 : -1, 0)
                : new Vector2Int(0, delta.y >= 0f ? 1 : -1);

            var target = new BoardCoordinate(_coordinate.X + direction.x, _coordinate.Y + direction.y);
            _hasTriggeredSwipe = true;
            SwipeRequested?.Invoke(_coordinate, target);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _hasTriggeredSwipe = false;
        }
    }
}
