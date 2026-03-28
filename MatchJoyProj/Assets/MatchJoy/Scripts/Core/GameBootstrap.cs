using UnityEngine;

namespace MatchJoy.Core
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private MatchJoy.Flow.GameFlowController _gameFlowController;

        private void Start()
        {
            if (_gameFlowController == null)
            {
                Debug.LogError("GameFlowController reference is missing.", this);
                return;
            }

            _gameFlowController.Initialize();
        }
    }
}
