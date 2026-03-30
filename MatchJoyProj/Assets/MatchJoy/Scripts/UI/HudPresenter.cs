using MatchJoy.Authoring;
using MatchJoy.Goals;
using UnityEngine;
using UnityEngine.UI;

namespace MatchJoy.UI
{
    public sealed class HudPresenter : MonoBehaviour
    {
        [SerializeField] private Image _panelBackground;
        [SerializeField] private Image _accentBar;
        [SerializeField] private Text _overlineLabel;
        [SerializeField] private Text _titleLabel;
        [SerializeField] private Text _goalLabel;
        [SerializeField] private Text _movesLabel;
        [SerializeField] private Text _paceLabel;
        [SerializeField] private Text _stateLabel;
        [SerializeField] private Text _footerLabel;

        private LevelDefinition _levelDefinition;
        private GoalProgressSnapshot _goalSnapshot;

        private void Awake()
        {
            ApplyTheme();
        }

        public void ShowLevelStatus(LevelDefinition levelDefinition, int remainingMoves, GoalProgressSnapshot goalSnapshot)
        {
            _levelDefinition = levelDefinition;
            _goalSnapshot = goalSnapshot;
            ApplyStatusTheme(remainingMoves);
            ApplyLevelContext();
            ShowMoves(remainingMoves);
        }

        public void ShowMoves(int remainingMoves)
        {
            if (_movesLabel != null)
            {
                _movesLabel.supportRichText = true;
                _movesLabel.alignment = TextAnchor.MiddleCenter;
                _movesLabel.text = $"<size=20><b>{remainingMoves}</b></size>\n<size=11>moves left</size>";
            }

            if (_paceLabel != null)
            {
                _paceLabel.supportRichText = true;
                _paceLabel.alignment = TextAnchor.MiddleCenter;
                _paceLabel.text = BuildPaceSummary(remainingMoves);
            }

            if (_stateLabel != null)
            {
                _stateLabel.supportRichText = true;
                _stateLabel.text = remainingMoves <= 5
                    ? "<b>Final stretch</b>\nEvery move matters now."
                    : remainingMoves <= 10
                        ? "<b>Keep the board moving</b>\nSet up stronger clears."
                        : "<b>Workshop open</b>\nBuild a clean early board.";
            }

            if (_footerLabel != null)
            {
                _footerLabel.supportRichText = true;
                _footerLabel.alignment = TextAnchor.MiddleLeft;
                _footerLabel.text = BuildFooterSummary(remainingMoves);
            }
        }

        private void ApplyLevelContext()
        {
            if (_overlineLabel != null)
            {
                _overlineLabel.supportRichText = true;
                _overlineLabel.alignment = TextAnchor.UpperLeft;
                _overlineLabel.text = _levelDefinition != null
                    ? $"<size=11><b>WORKSHOP ROUTE / CHAPTER {_levelDefinition.ChapterIndex}</b></size>"
                    : "<size=11><b>WORKSHOP ROUTE</b></size>";
            }

            if (_titleLabel != null)
            {
                _titleLabel.supportRichText = true;
                _titleLabel.alignment = TextAnchor.UpperLeft;

                if (_levelDefinition != null)
                {
                    _titleLabel.text =
                        $"<size=11><color=#8E6B57><b>CHAPTER {_levelDefinition.ChapterIndex}</b></color></size>\n" +
                        $"<size=20><b>Level {_levelDefinition.LevelOrder:00}</b></size>";
                }
                else
                {
                    _titleLabel.text = "<size=20><b>Workshop Run</b></size>";
                }
            }

            if (_goalLabel != null)
            {
                _goalLabel.supportRichText = true;
                _goalLabel.alignment = TextAnchor.UpperLeft;

                if (_levelDefinition == null || _levelDefinition.Goals == null || _levelDefinition.Goals.Length == 0)
                {
                    _goalLabel.text = "<b>Goal</b>\nWarm up the board.";
                    return;
                }

                var completed = _goalSnapshot.TotalGoals > 0
                    ? $"{_goalSnapshot.CompletedGoals}/{_goalSnapshot.TotalGoals}"
                    : "0/0";
                _goalLabel.text = $"<b>Goals {completed}</b>\n{BuildGoalSummary(_levelDefinition, _goalSnapshot)}";
            }
        }

        private void ApplyTheme()
        {
            if (_panelBackground != null)
            {
                _panelBackground.color = new Color32(255, 247, 236, 238);
            }

            if (_accentBar != null)
            {
                _accentBar.color = new Color32(224, 164, 108, 255);
            }

            ApplyTextTheme(_overlineLabel, new Color32(172, 126, 96, 255), FontStyle.Bold);
            ApplyTextTheme(_titleLabel, new Color32(118, 79, 61, 255), FontStyle.Bold);
            ApplyTextTheme(_goalLabel, new Color32(126, 96, 82, 255), FontStyle.Normal);
            ApplyTextTheme(_movesLabel, new Color32(149, 89, 63, 255), FontStyle.Bold);
            ApplyTextTheme(_paceLabel, new Color32(185, 116, 72, 255), FontStyle.Bold);
            ApplyTextTheme(_stateLabel, new Color32(143, 112, 98, 255), FontStyle.Italic);
            ApplyTextTheme(_footerLabel, new Color32(156, 121, 102, 255), FontStyle.Normal);
        }

        private static void ApplyTextTheme(Text label, Color32 color, FontStyle fontStyle)
        {
            if (label == null)
            {
                return;
            }

            label.color = color;
            label.fontStyle = fontStyle;
        }

        private static string BuildGoalSummary(LevelDefinition levelDefinition, GoalProgressSnapshot snapshot)
        {
            var parts = new string[levelDefinition.Goals.Length];
            for (var i = 0; i < levelDefinition.Goals.Length; i++)
            {
                var goal = levelDefinition.Goals[i];
                var progress = snapshot.CurrentProgress != null && i < snapshot.CurrentProgress.Length
                    ? snapshot.CurrentProgress[i]
                    : 0;
                var target = Mathf.Max(1, goal.TargetCount);
                parts[i] = $"{DescribeGoal(goal)} {Mathf.Min(progress, target)}/{target}";
            }

            return string.Join("  /  ", parts);
        }

        private static string DescribeGoal(GoalDefinition goal)
        {
            return goal.GoalType switch
            {
                GoalType.ClearJelly => "Clear jelly",
                GoalType.CollectTile => $"Collect color {goal.TargetTileId}",
                GoalType.ReleaseFrozenIngredient => "Release ingredient",
                _ => "Finish task"
            };
        }

        private string BuildPaceSummary(int remainingMoves)
        {
            if (_levelDefinition == null)
            {
                return "<size=11><b>Prototype pace</b></size>\n<size=10>No star band available yet.</size>";
            }

            var thresholds = _levelDefinition.MoveBasedStarThresholds;
            var stars = EvaluateStarBand(remainingMoves, thresholds);
            var band = stars switch
            {
                3 => "Excellent pace",
                2 => "On-track clear",
                _ => "Scrappy finish"
            };

            return
                $"<size=11><b>{band}</b></size>\n" +
                $"<size=10>{BuildStarString(stars)}  -  {BuildThresholdSummary(thresholds)}</size>";
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
                3 => "*** forecast",
                2 => "**- forecast",
                _ => "*-- forecast"
            };
        }

        private static string BuildThresholdSummary(int[] thresholds)
        {
            if (thresholds == null || thresholds.Length == 0)
            {
                return "No move targets";
            }

            var oneStar = thresholds.Length > 0 ? thresholds[0] : 0;
            var twoStar = thresholds.Length > 1 ? thresholds[1] : oneStar;
            var threeStar = thresholds.Length > 2 ? thresholds[2] : twoStar;
            return $"1/2/3 star at {oneStar}/{twoStar}/{threeStar}+";
        }

        private void ApplyStatusTheme(int remainingMoves)
        {
            var isPressureWindow = remainingMoves <= 5;
            if (_panelBackground != null)
            {
                _panelBackground.color = isPressureWindow
                    ? new Color32(255, 241, 234, 242)
                    : new Color32(255, 247, 236, 238);
            }

            if (_accentBar != null)
            {
                _accentBar.color = isPressureWindow
                    ? new Color32(212, 113, 92, 255)
                    : remainingMoves <= 10
                        ? new Color32(228, 146, 96, 255)
                        : new Color32(224, 164, 108, 255);
            }
        }

        private string BuildFooterSummary(int remainingMoves)
        {
            if (_levelDefinition == null)
            {
                return "<size=11>Prototype board. Keep the tray moving.</size>";
            }

            var moveLimit = Mathf.Max(1, _levelDefinition.MoveLimit);
            var spend = moveLimit - remainingMoves;
            return
                $"<size=11>Spent {spend}/{moveLimit} moves  /  " +
                $"{_goalSnapshot.CompletedGoals}/{_goalSnapshot.TotalGoals} goals closed</size>";
        }
    }
}
