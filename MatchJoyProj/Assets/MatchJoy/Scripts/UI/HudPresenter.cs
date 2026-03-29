using UnityEngine;
using UnityEngine.UI;

namespace MatchJoy.UI
{
    public sealed class HudPresenter : MonoBehaviour
    {
        [SerializeField] private Text _movesLabel;

        public void ShowMoves(int remainingMoves)
        {
            if (_movesLabel != null)
            {
                _movesLabel.text = $"Moves: {remainingMoves}";
            }
        }
    }
}
