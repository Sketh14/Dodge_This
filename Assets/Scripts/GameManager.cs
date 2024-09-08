using System.Collections;
using UnityEngine;

namespace Dodge_This
{
    public class GameManager : MonoBehaviour
    {
        // [Header("Local Reference Scripts")]
        // public GameLogic gameLogicReference;
        // public PlayerController playerControllerReference;

        [Header("Game Status")]
        [SerializeField] private Transform _mainCameraTransform;
        public bool gameStarted, gamePaused, PlayerUnAlive;
        public int Score;
        private Coroutine _cameraShake;
        //protected bool ;

        // Actions
        public System.Action OnGameRestart, OnGameStarted, OnPlayerScored;
        public System.Action<bool> OnPlayerUnAlive, OnGameplayPaused;

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

        private void OnDestroy()
        {
            OnPlayerUnAlive -= ShakeCameraHelper;
            OnGameRestart -= ResetStats;
        }

        private void Start()
        {
            OnPlayerUnAlive += ShakeCameraHelper;
            OnGameRestart += ResetStats;
        }

        private void ResetStats()
        {
            Score = 0;
        }

        private void ShakeCameraHelper(bool dummyData) { _ = StartCoroutine(ShakeCamera()); }
        private IEnumerator ShakeCamera()
        {
            float elapsed = 0f, duration = 0.6f;             //Keeping constant for now
            Vector3 offset = Vector3.zero;
            Vector3 originalPos = _mainCameraTransform.localPosition;
            float x, y, magnitude = 1f;             //Keeping constant for now

            while (elapsed < duration)
            {
                // Debug.Log("Shaking");
                x = Random.Range(-1f, 1f) * magnitude;
                y = Random.Range(-1f, 1f) * magnitude;

                // Move the camera's position
                offset.x = x;
                offset.y = y;
                _mainCameraTransform.localPosition = originalPos + offset;

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Reset the camera's position
            _mainCameraTransform.localPosition = originalPos;

            yield return null;
        }
    }
}
