using System;
using System.Collections;
using MatchJoy.Board;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MatchJoy.UI
{
    public sealed class BoardCellView : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public enum RefreshTransitionType
        {
            None,
            InitialPopulate,
            SelectionPulse,
            TileChanged,
            Refilled
        }

        public enum PreApplyEffectType
        {
            None,
            SwapPreview,
            ClearFade
        }

        public readonly struct RefreshTransition
        {
            public RefreshTransition(
                RefreshTransitionType type,
                bool applyStateAfterDelay,
                float delaySeconds = 0f,
                Vector2 originOffset = default,
                Vector2 preApplyOffset = default,
                float preApplyDuration = 0f,
                PreApplyEffectType preApplyEffectType = PreApplyEffectType.None)
            {
                Type = type;
                ApplyStateAfterDelay = applyStateAfterDelay;
                DelaySeconds = delaySeconds;
                OriginOffset = originOffset;
                PreApplyOffset = preApplyOffset;
                PreApplyDuration = preApplyDuration;
                PreApplyEffectType = preApplyEffectType;
            }

            public RefreshTransitionType Type { get; }
            public bool ApplyStateAfterDelay { get; }
            public float DelaySeconds { get; }
            public Vector2 OriginOffset { get; }
            public Vector2 PreApplyOffset { get; }
            public float PreApplyDuration { get; }
            public PreApplyEffectType PreApplyEffectType { get; }
        }

        private const float SwipeThreshold = 24f;

        [SerializeField] private Image _background;
        [SerializeField] private Text _tileLabel;
        [SerializeField] private RectTransform _animatedRoot;
        [SerializeField] private float _selectionPulseScale = 1.08f;
        [SerializeField] private float _tileChangeScale = 1.12f;
        [SerializeField] private float _selectionPulseDuration = 0.08f;
        [SerializeField] private float _tileChangeDuration = 0.12f;
        [SerializeField] private float _refillFadeDuration = 0.12f;
        [SerializeField] private float _refillDropDuration = 0.16f;
        [SerializeField] private float _clearFadeScale = 0.82f;

        private bool _isPlayable;
        private BoardCoordinate _coordinate;
        private Vector2 _dragStartPosition;
        private bool _hasTriggeredSwipe;
        private Coroutine _activeAnimation;

        public event Action<BoardCoordinate> Clicked;
        public event Action<BoardCoordinate, BoardCoordinate> SwipeRequested;

        public void SetCoordinate(BoardCoordinate coordinate)
        {
            _coordinate = coordinate;
        }

        public bool Render(bool isPlayable, bool isOccupied, int tileId, bool isSelected, RefreshTransition transition, Action completion = null)
        {
            _isPlayable = isPlayable;

            if (_activeAnimation != null)
            {
                StopCoroutine(_activeAnimation);
                _activeAnimation = null;
            }

            EnsureAnimatedRoot();
            ResetAnimatedRoot();

            if (!RequiresAnimation(transition))
            {
                ApplyVisualState(isPlayable, isOccupied, tileId, isSelected);
                completion?.Invoke();
                return false;
            }

            _activeAnimation = StartCoroutine(RunTransition(isPlayable, isOccupied, tileId, isSelected, transition, completion));
            return true;
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

        private IEnumerator RunTransition(bool isPlayable, bool isOccupied, int tileId, bool isSelected, RefreshTransition transition, Action completion)
        {
            if (transition.ApplyStateAfterDelay)
            {
                yield return ApplyStateThenAnimate(isPlayable, isOccupied, tileId, isSelected, transition);
            }
            else
            {
                ApplyVisualState(isPlayable, isOccupied, tileId, isSelected);
                yield return PlayRefreshAnimation(transition, isOccupied);
            }

            _activeAnimation = null;
            completion?.Invoke();
        }

        private static bool RequiresAnimation(RefreshTransition transition)
        {
            return transition.Type != RefreshTransitionType.None
                || transition.ApplyStateAfterDelay
                || transition.DelaySeconds > 0f
                || transition.PreApplyDuration > 0f;
        }

        private IEnumerator ApplyStateThenAnimate(bool isPlayable, bool isOccupied, int tileId, bool isSelected, RefreshTransition transition)
        {
            if (transition.PreApplyDuration > 0f)
            {
                switch (transition.PreApplyEffectType)
                {
                    case PreApplyEffectType.SwapPreview:
                        yield return PlaySwapPreview(transition.PreApplyOffset, transition.PreApplyDuration);
                        break;
                    case PreApplyEffectType.ClearFade:
                        yield return PlayClearFade(transition.PreApplyDuration);
                        break;
                }
            }

            if (transition.DelaySeconds > 0f)
            {
                yield return new WaitForSecondsRealtime(transition.DelaySeconds);
            }

            ApplyVisualState(isPlayable, isOccupied, tileId, isSelected);
            yield return PlayRefreshAnimation(new RefreshTransition(transition.Type, false, 0f, transition.OriginOffset), isOccupied);
        }

        private IEnumerator PlaySwapPreview(Vector2 offset, float duration)
        {
            var elapsed = 0f;
            var halfDuration = Mathf.Max(0.001f, duration * 0.5f);

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var normalized = elapsed <= halfDuration
                    ? elapsed / halfDuration
                    : 1f - ((elapsed - halfDuration) / halfDuration);
                var eased = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(normalized));
                _animatedRoot.anchoredPosition = Vector2.Lerp(Vector2.zero, offset, eased);
                yield return null;
            }

            _animatedRoot.anchoredPosition = Vector2.zero;
        }

        private IEnumerator PlayClearFade(float duration)
        {
            var backgroundColor = _background != null ? _background.color : Color.clear;
            var labelColor = _tileLabel != null ? _tileLabel.color : Color.clear;
            var elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var normalized = Mathf.Clamp01(elapsed / duration);
                var eased = Mathf.SmoothStep(0f, 1f, normalized);
                var scale = Mathf.Lerp(1f, _clearFadeScale, eased);
                _animatedRoot.localScale = new Vector3(scale, scale, 1f);
                SetAlpha(Mathf.Lerp(1f, 0f, eased));
                yield return null;
            }

            if (_background != null)
            {
                _background.color = backgroundColor;
            }

            if (_tileLabel != null)
            {
                _tileLabel.color = labelColor;
            }

            ResetAnimatedRoot();
        }

        private void ApplyVisualState(bool isPlayable, bool isOccupied, int tileId, bool isSelected)
        {
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

        private IEnumerator PlayRefreshAnimation(RefreshTransition transition, bool isOccupied)
        {
            if (_animatedRoot == null)
            {
                yield break;
            }

            switch (transition.Type)
            {
                case RefreshTransitionType.InitialPopulate:
                    if (isOccupied)
                    {
                        yield return PlayFadeInAnimation(transition.DelaySeconds);
                    }
                    break;
                case RefreshTransitionType.SelectionPulse:
                    yield return PlayScalePulse(_selectionPulseScale, _selectionPulseDuration, transition.DelaySeconds);
                    break;
                case RefreshTransitionType.TileChanged:
                    yield return PlayScalePulse(_tileChangeScale, _tileChangeDuration, transition.DelaySeconds);
                    break;
                case RefreshTransitionType.Refilled:
                    yield return PlayRefillAnimation(transition.DelaySeconds, transition.OriginOffset);
                    break;
            }
        }

        private IEnumerator PlayScalePulse(float peakScale, float duration, float delaySeconds)
        {
            if (delaySeconds > 0f)
            {
                yield return new WaitForSecondsRealtime(delaySeconds);
            }

            var elapsed = 0f;
            var halfDuration = Mathf.Max(0.001f, duration * 0.5f);

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var normalized = elapsed <= halfDuration
                    ? elapsed / halfDuration
                    : 1f - ((elapsed - halfDuration) / halfDuration);
                var scale = Mathf.Lerp(1f, peakScale, Mathf.Clamp01(normalized));
                _animatedRoot.localScale = new Vector3(scale, scale, 1f);
                yield return null;
            }

            ResetAnimatedRoot();
        }

        private IEnumerator PlayFadeInAnimation(float delaySeconds)
        {
            if (delaySeconds > 0f)
            {
                yield return new WaitForSecondsRealtime(delaySeconds);
            }

            var backgroundColor = _background != null ? _background.color : Color.clear;
            var labelColor = _tileLabel != null ? _tileLabel.color : Color.clear;
            var elapsed = 0f;

            SetAlpha(0f);

            while (elapsed < _refillFadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                SetAlpha(Mathf.Clamp01(elapsed / _refillFadeDuration));
                yield return null;
            }

            if (_background != null)
            {
                _background.color = backgroundColor;
            }

            if (_tileLabel != null)
            {
                _tileLabel.color = labelColor;
            }
        }

        private IEnumerator PlayRefillAnimation(float delaySeconds, Vector2 originOffset)
        {
            if (delaySeconds > 0f)
            {
                yield return new WaitForSecondsRealtime(delaySeconds);
            }

            var backgroundColor = _background != null ? _background.color : Color.clear;
            var labelColor = _tileLabel != null ? _tileLabel.color : Color.clear;
            var elapsed = 0f;

            _animatedRoot.anchoredPosition = originOffset;
            SetAlpha(0f);

            while (elapsed < _refillDropDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                var normalized = Mathf.Clamp01(elapsed / _refillDropDuration);
                var eased = 1f - Mathf.Pow(1f - normalized, 3f);
                _animatedRoot.anchoredPosition = Vector2.Lerp(originOffset, Vector2.zero, eased);
                SetAlpha(Mathf.Clamp01(elapsed / _refillFadeDuration));
                yield return null;
            }

            _animatedRoot.anchoredPosition = Vector2.zero;

            if (_background != null)
            {
                _background.color = backgroundColor;
            }

            if (_tileLabel != null)
            {
                _tileLabel.color = labelColor;
            }

            yield return PlayScalePulse(_tileChangeScale, _tileChangeDuration, 0f);
            ResetAnimatedRoot();
        }

        private void EnsureAnimatedRoot()
        {
            if (_animatedRoot == null)
            {
                _animatedRoot = transform as RectTransform;
            }
        }

        private void ResetAnimatedRoot()
        {
            if (_animatedRoot == null)
            {
                return;
            }

            _animatedRoot.localScale = Vector3.one;
            _animatedRoot.anchoredPosition = Vector2.zero;
        }

        private void SetAlpha(float alpha)
        {
            if (_background != null)
            {
                var backgroundColor = _background.color;
                backgroundColor.a = alpha;
                _background.color = backgroundColor;
            }

            if (_tileLabel != null)
            {
                var labelColor = _tileLabel.color;
                labelColor.a = alpha;
                _tileLabel.color = labelColor;
            }
        }
    }
}
