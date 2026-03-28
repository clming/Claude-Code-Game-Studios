using TMPro;
using UnityEngine;

namespace MatchJoy.UI
{
    public sealed class HudPresenter : MonoBehaviour
    {
        [SerializeField] private TMP_Text _movesLabel;

        public void ShowMoves(int remainingMoves)
        {
            if (_movesLabel != null)
            {
                _movesLabel.text = $"Moves: {remainingMoves}";
            }
        }
    }
}
