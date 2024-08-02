using UnityEngine;

namespace Dodge_This
{
    public class GameLogic : MonoBehaviour
    {
        public System.Action OnGameRestart, OnGameStarted, OnPlayerScored;
        public System.Action<bool> OnPlayerUnAlive, OnGameplayPaused;
    }
}