using System;
using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private Color _slotTint = new(0.93f, 0.80f, 0.72f, 0.34f);
        [SerializeField] private Color _blockedTint = new(0.35f, 0.26f, 0.25f, 0.86f);
        [SerializeField] private Color _selectedFillTint = new(1f, 0.79f, 0.46f, 1f);

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

        public bool Render(bool isPlayable, bool isOccupied, int tileId, bool isSelected, BoardCellPresentationInstruction instruction, Action completion = null)
        {
            _isPlayable = isPlayable;

            if (_activeAnimation != null)
            {
                StopCoroutine(_activeAnimation);
                _activeAnimation = null;
            }

            EnsureAnimatedRoot();
            ResetAnimatedRoot();

            if (!instruction.RequiresAnimation)
            {
                ApplyVisualState(isPlayable, isOccupied, tileId, isSelected);
                completion?.Invoke();
                return false;
            }

            _activeAnimation = StartCoroutine(RunInstruction(isPlayable, isOccupied, tileId, isSelected, instruction, completion));
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

        private IEnumerator RunInstruction(bool isPlayable, bool isOccupied, int tileId, bool isSelected, BoardCellPresentationInstruction instruction, Action completion)
        {
            foreach (var phase in instruction.Phases)
            {
                yield return PlayPresentationPhase(phase, isPlayable, isOccupied, tileId, isSelected);
            }

            _activeAnimation = null;
            completion?.Invoke();
        }

        public static IReadOnlyList<BoardCellPresentationPhase> BuildPresentationPhases(RefreshTransition transition, bool isOccupied)
        {
            var phases = new List<BoardCellPresentationPhase>();

            if (transition.ApplyStateAfterDelay && transition.PreApplyDuration > 0f)
            {
                switch (transition.PreApplyEffectType)
                {
                    case PreApplyEffectType.SwapPreview:
                        phases.Add(BoardCellPresentationPhase.SwapPreview(transition.PreApplyDuration, transition.PreApplyOffset));
                        break;
                    case PreApplyEffectType.ClearFade:
                        phases.Add(BoardCellPresentationPhase.ClearFade(transition.PreApplyDuration));
                        break;
                }
            }

            if (transition.ApplyStateAfterDelay && transition.DelaySeconds > 0f)
            {
                phases.Add(BoardCellPresentationPhase.Wait(transition.DelaySeconds));
            }

            phases.Add(BoardCellPresentationPhase.ApplyVisualState());

            if (!transition.ApplyStateAfterDelay && transition.DelaySeconds > 0f)
            {
                phases.Add(BoardCellPresentationPhase.Wait(transition.DelaySeconds));
            }

            switch (transition.Type)
            {
                case RefreshTransitionType.InitialPopulate:
                    if (isOccupied)
                    {
                        phases.Add(BoardCellPresentationPhase.InitialPopulateFade());
                    }
                    break;
                case RefreshTransitionType.SelectionPulse:
                    phases.Add(BoardCellPresentationPhase.SelectionPulse());
                    break;
                case RefreshTransitionType.TileChanged:
                    phases.Add(BoardCellPresentationPhase.TileChangePulse());
                    break;
                case RefreshTransitionType.Refilled:
                    phases.Add(BoardCellPresentationPhase.RefillDrop(transition.OriginOffset));
                    break;
            }

            return phases;
        }

        private IEnumerator PlayPresentationPhase(
            BoardCellPresentationPhase phase,
            bool isPlayable,
            bool isOccupied,
            int tileId,
            bool isSelected)
        {
            switch (phase.Type)
            {
                case BoardCellPresentationPhaseType.Wait:
                    if (phase.DurationSeconds > 0f)
                    {
                        yield return new WaitForSecondsRealtime(phase.DurationSeconds);
                    }
                    break;
                case BoardCellPresentationPhaseType.SwapPreview:
                    yield return PlaySwapPreview(phase.Offset, phase.DurationSeconds);
                    break;
                case BoardCellPresentationPhaseType.ClearFade:
                    yield return PlayClearFade(phase.DurationSeconds);
                    break;
                case BoardCellPresentationPhaseType.ApplyVisualState:
                    ApplyVisualState(isPlayable, isOccupied, tileId, isSelected);
                    break;
                case BoardCellPresentationPhaseType.SelectionPulse:
                    yield return PlayScalePulse(_selectionPulseScale, _selectionPulseDuration);
                    break;
                case BoardCellPresentationPhaseType.TileChangePulse:
                    yield return PlayScalePulse(_tileChangeScale, _tileChangeDuration);
                    break;
                case BoardCellPresentationPhaseType.InitialPopulateFade:
                    yield return PlayFadeInAnimation();
                    break;
                case BoardCellPresentationPhaseType.RefillDrop:
                    yield return PlayRefillAnimation(phase.Offset);
                    break;
            }
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
                _background.color = ResolveBackgroundColor(isPlayable, isOccupied, tileId, isSelected);
            }

            if (_tileLabel != null)
            {
                _tileLabel.supportRichText = true;
                _tileLabel.alignment = TextAnchor.MiddleCenter;
                _tileLabel.fontStyle = FontStyle.Bold;
                _tileLabel.text = isOccupied ? BuildTileGlyph(tileId) : string.Empty;
                _tileLabel.color = ResolveLabelColor(isPlayable, isOccupied, tileId, isSelected);
            }
        }

        private Color ResolveBackgroundColor(bool isPlayable, bool isOccupied, int tileId, bool isSelected)
        {
            if (!isPlayable)
            {
                return _blockedTint;
            }

            if (!isOccupied)
            {
                return _slotTint;
            }

            var baseColor = ResolveTileColor(tileId);
            return isSelected
                ? Color.Lerp(baseColor, _selectedFillTint, 0.45f)
                : baseColor;
        }

        private Color ResolveLabelColor(bool isPlayable, bool isOccupied, int tileId, bool isSelected)
        {
            if (!isPlayable || !isOccupied)
            {
                return Color.clear;
            }

            var tileColor = ResolveTileColor(tileId);
            var luminance = (tileColor.r * 0.299f) + (tileColor.g * 0.587f) + (tileColor.b * 0.114f);
            if (isSelected)
            {
                return new Color(0.34f, 0.19f, 0.13f, 1f);
            }

            return luminance > 0.62f
                ? new Color(0.29f, 0.20f, 0.17f, 1f)
                : new Color(1f, 0.98f, 0.95f, 1f);
        }

        private static string BuildTileGlyph(int tileId)
        {
            return tileId switch
            {
                0 => "A",
                1 => "B",
                2 => "C",
                3 => "D",
                4 => "E",
                5 => "F",
                6 => "G",
                _ => tileId.ToString()
            };
        }

        private static Color ResolveTileColor(int tileId)
        {
            return tileId switch
            {
                0 => new Color(1.00f, 0.56f, 0.49f, 1f),
                1 => new Color(1.00f, 0.77f, 0.39f, 1f),
                2 => new Color(0.99f, 0.90f, 0.45f, 1f),
                3 => new Color(0.59f, 0.84f, 0.51f, 1f),
                4 => new Color(0.47f, 0.77f, 0.98f, 1f),
                5 => new Color(0.67f, 0.62f, 0.95f, 1f),
                6 => new Color(0.99f, 0.56f, 0.76f, 1f),
                _ => new Color(0.95f, 0.88f, 0.80f, 1f)
            };
        }

        private IEnumerator PlayScalePulse(float peakScale, float duration)
        {
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

        private IEnumerator PlayFadeInAnimation()
        {
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

        private IEnumerator PlayRefillAnimation(Vector2 originOffset)
        {
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

            yield return PlayScalePulse(_tileChangeScale, _tileChangeDuration);
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
