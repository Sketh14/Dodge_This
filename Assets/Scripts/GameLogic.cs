using UnityEngine;

namespace Double_Jump
{
    public class GameLogic : MonoBehaviour
    {
        public System.Action OnGameRestart, OnGameStarted, OnPlayerScored;
        public System.Action<bool> OnPlayerUnAlive;
    }
}