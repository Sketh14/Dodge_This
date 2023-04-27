using UnityEngine;

namespace Double_Jump
{
    public class GameManager : MonoBehaviour
    {
        [Header("Local Reference Scripts")]
        public GameLogic gameLogicReference;
        public PlayerController playerControllerReference;

        [Header("Power Ups Section")]
        public bool gameStarted, gamePaused;
        //protected bool ;

        private static GameManager _instance;
        public static GameManager instance
        {
            get => _instance;
        }

        // Start is called before the first frame update
        void Awake()
        {
            if (_instance == null && _instance != this)
            {
                _instance = this;
            }
            else
                Destroy(this);

            Application.targetFrameRate = 60;
        }
    }
}
