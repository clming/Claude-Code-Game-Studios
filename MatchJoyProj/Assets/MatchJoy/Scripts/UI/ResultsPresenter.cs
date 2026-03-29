using UnityEngine;
using UnityEngine.UI;

namespace MatchJoy.UI
{
    public sealed class ResultsPresenter : MonoBehaviour
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private Text _headlineLabel;
        [SerializeField] private Text _detailsLabel;

        public void ShowVictory(int remainingMoves)
        {
            SetVisible(true);
            SetText("Victory", $"Remaining Moves: {remainingMoves}");
        }

        public void ShowFailure()
        {
            SetVisible(true);
            SetText("Failure", "No remaining moves. Retry to try again.");
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

        private void SetText(string headline, string details)
        {
            if (_headlineLabel != null)
            {
                _headlineLabel.text = headline;
            }

            if (_detailsLabel != null)
            {
                _detailsLabel.text = details;
            }
        }
    }
}
