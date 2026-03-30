using MatchJoy.Authoring;
using UnityEngine;
using UnityEngine.UI;

namespace MatchJoy.UI
{
    public sealed class ResultsPresenter : MonoBehaviour
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private Image _panelBackground;
        [SerializeField] private Image _accentBar;
        [SerializeField] private Text _overlineLabel;
        [SerializeField] private Text _badgeLabel;
        [SerializeField] private Text _headlineLabel;
        [SerializeField] private Text _summaryLabel;
        [SerializeField] private Text _detailsLabel;
        [SerializeField] private Text _footerLabel;

        private void Awake()
        {
            ApplyBaseTheme();
        }

        public void ShowVictory(int remainingMoves, LevelDefinition levelDefinition = null)
        {
            SetVisible(true);
            ApplyStateTheme(
                new Color32(255, 241, 214, 245),
                new Color32(173, 102, 56, 255),
                new Color32(234, 150, 86, 255));
            SetOverline("ORDER COMPLETE");
            SetBadge(levelDefinition);
            SetText(
                "Workshop Clear",
                BuildVictorySummary(remainingMoves, levelDefinition),
                remainingMoves > 0
                    ? $"You closed the order with <b>{remainingMoves}</b> moves still in reserve."
                    : "You cleared the board right on the final move.");
            SetFooter(BuildFooter(levelDefinition, true));
        }

        public void ShowFailure(LevelDefinition levelDefinition = null)
        {
            SetVisible(true);
            ApplyStateTheme(
                new Color32(255, 231, 228, 245),
                new Color32(154, 76, 76, 255),
                new Color32(205, 104, 96, 255));
            SetOverline("ORDER INTERRUPTED");
            SetBadge(levelDefinition);
            SetText(
                "Batch Burned",
                BuildFailureSummary(levelDefinition),
                "The workshop ran out of moves before the order was complete. Reset, regroup, and try a cleaner setup.");
            SetFooter(BuildFooter(levelDefinition, false));
        }

        public void Hide()
        {
            SetVisible(false);
        }

        private void SetVisible(bool isVisible)
        {
            if (_root != null)
            {
                _root.SetActive(isVisible);
            }
        }

        private void SetText(string headline, string summary, string details)
        {
            if (_headlineLabel != null)
            {
                _headlineLabel.supportRichText = true;
                _headlineLabel.alignment = TextAnchor.MiddleCenter;
                _headlineLabel.fontStyle = FontStyle.Bold;
                _headlineLabel.text = headline;
            }

            if (_summaryLabel != null)
            {
                _summaryLabel.supportRichText = true;
                _summaryLabel.alignment = TextAnchor.MiddleCenter;
                _summaryLabel.text = summary;
            }

            if (_detailsLabel != null)
            {
                _detailsLabel.supportRichText = true;
                _detailsLabel.alignment = TextAnchor.MiddleCenter;
                _detailsLabel.text = details;
            }
        }

        private void SetOverline(string overline)
        {
            if (_overlineLabel == null)
            {
                return;
            }

            _overlineLabel.supportRichText = true;
            _overlineLabel.alignment = TextAnchor.MiddleCenter;
            _overlineLabel.fontStyle = FontStyle.Bold;
            _overlineLabel.text = overline;
        }

        private void SetBadge(LevelDefinition levelDefinition)
        {
            if (_badgeLabel == null)
            {
                return;
            }

            _badgeLabel.supportRichText = true;
            _badgeLabel.alignment = TextAnchor.MiddleCenter;
            _badgeLabel.text = levelDefinition != null
                ? $"<b>Chapter {levelDefinition.ChapterIndex}</b>  /  Level {levelDefinition.LevelOrder:00}"
                : "<b>Workshop Prototype</b>";
        }

        private void SetFooter(string footer)
        {
            if (_footerLabel == null)
            {
                return;
            }

            _footerLabel.supportRichText = true;
            _footerLabel.alignment = TextAnchor.MiddleCenter;
            _footerLabel.text = footer;
        }

        private void ApplyBaseTheme()
        {
            ApplyTextTheme(_overlineLabel, new Color32(176, 131, 101, 255));
            ApplyTextTheme(_headlineLabel, new Color32(112, 75, 56, 255));
            ApplyTextTheme(_summaryLabel, new Color32(160, 102, 68, 255));
            ApplyTextTheme(_detailsLabel, new Color32(126, 96, 82, 255));
            ApplyTextTheme(_badgeLabel, new Color32(156, 124, 104, 255));
            ApplyTextTheme(_footerLabel, new Color32(154, 121, 103, 255));
        }

        private void ApplyStateTheme(Color32 panelColor, Color32 headlineColor, Color32 accentColor)
        {
            if (_panelBackground != null)
            {
                _panelBackground.color = panelColor;
            }

            if (_accentBar != null)
            {
                _accentBar.color = accentColor;
            }

            if (_headlineLabel != null)
            {
                _headlineLabel.color = headlineColor;
            }
        }

        private static void ApplyTextTheme(Text label, Color32 color)
        {
            if (label == null)
            {
                return;
            }

            label.color = color;
        }

        private static string BuildVictorySummary(int remainingMoves, LevelDefinition levelDefinition)
        {
            var stars = EvaluateStarBand(remainingMoves, levelDefinition?.MoveBasedStarThresholds);
            var pace = stars switch
            {
                3 => "Signature finish",
                2 => "Solid workshop clear",
                _ => "Narrow closeout"
            };

            return $"{BuildStarString(stars)}  -  <b>{pace}</b>";
        }

        private static string BuildFailureSummary(LevelDefinition levelDefinition)
        {
            if (levelDefinition == null)
            {
                return "No rank posted for this workshop run.";
            }

            var thresholds = levelDefinition.MoveBasedStarThresholds;
            if (thresholds == null || thresholds.Length == 0)
            {
                return "No star thresholds were defined for this order.";
            }

            var target = thresholds[0];
            return $"Missed the clear. First rating band starts at <b>{target}+</b> moves remaining.";
        }

        private static int EvaluateStarBand(int remainingMoves, int[] thresholds)
        {
            if (thresholds == null || thresholds.Length == 0)
            {
                return remainingMoves > 0 ? 2 : 1;
            }

            var stars = 1;
            if (thresholds.Length > 1 && remainingMoves >= thresholds[1])
            {
                stars = 2;
            }

            if (thresholds.Length > 2 && remainingMoves >= thresholds[2])
            {
                stars = 3;
            }

            return Mathf.Clamp(stars, 1, 3);
        }

        private static string BuildStarString(int stars)
        {
            return stars switch
            {
                3 => "***",
                2 => "**-",
                _ => "*--"
            };
        }

        private static string BuildFooter(LevelDefinition levelDefinition, bool isVictory)
        {
            if (levelDefinition == null)
            {
                return isVictory
                    ? "Prototype route closed cleanly."
                    : "Prototype route still needs a steadier clear.";
            }

            return isVictory
                ? $"Chapter {levelDefinition.ChapterIndex} route sealed. Queue up the next workshop order."
                : $"Chapter {levelDefinition.ChapterIndex} route remains open. Reset the tray and push again.";
        }
    }
}
