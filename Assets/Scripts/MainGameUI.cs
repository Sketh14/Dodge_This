//#define MOBILE_CONTROLS                           //Uncomment for real gameplay

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Dodge_This
{
    public class MainGameUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private GameObject pausedBanner;
        [SerializeField] private RectTransform _clickImg, _spaceBarImg;
        private float blinkRateMultiplier = 1f;                //blink rate of 1f is enough
        [SerializeField] private TMP_Text currentScoreTxt, finalScoreTxt;

        // [Header("Local Reference Script")]
        // [SerializeField] private GameManager localGameManager;

        [Header("Scoring")]
        // private int score;
        private Coroutine clickBlinkCoroutine;

        [Header("Buttons")]
        [SerializeField] private Button _startGameBt;
        [SerializeField] private Button _restartBt, _pauseBt, _showCreditsBt, _exitCreditsBt;

        [Header("Panels")]
        [SerializeField] private GameObject _mainGameplayPanel;
        [SerializeField] private GameObject _mainMenuPanel, _settingsPanel, _gameOverPanel;

        [Header("SFX")]
        [SerializeField] private AudioSource bgm_Source;

        private void OnDestroy()
        {
            GameManager.instance.OnPlayerUnAlive -= InvokeGameOverUpdate;
            GameManager.instance.OnPlayerScored -= UpdateScore;
        }

        private void InvokeGameOverUpdate(bool dummyData)
        {
            Invoke(nameof(ShowGameOverPanel), 0.6f);
        }

        private void ShowGameOverPanel()
        {
            _gameOverPanel.SetActive(true);
            finalScoreTxt.text = GameManager.instance.Score.ToString();
            bgm_Source.Stop();
        }

        private void Start()
        {
            clickBlinkCoroutine = StartCoroutine(ClickRate());

            //Buttons
            _startGameBt.onClick.AddListener(() => StartGame());

            _restartBt.onClick.AddListener(() => RestartGame());

            _pauseBt.onClick.AddListener(() => { ToggleGameStatus(true); });
            _showCreditsBt.onClick.AddListener(() =>
            {
                _settingsPanel.SetActive(true);
                _mainMenuPanel.SetActive(false);
            });
            _exitCreditsBt.onClick.AddListener(() =>
            {
                _settingsPanel.SetActive(false);
                _mainMenuPanel.SetActive(true);
            });

            //Actions
            GameManager.instance.OnPlayerUnAlive += InvokeGameOverUpdate;
            GameManager.instance.OnPlayerScored += UpdateScore;
        }

        private void Update()
        {
#if MOBILE_CONTROLS
            if (gamePaused && Touch.activeTouches.Count > 0 && Touch.activeTouches[0].phase == UnityEngine.InputSystem.TouchPhase.Began)
#else
            if (GameManager.instance.gamePaused && UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame)
#endif
            {
                ToggleGameStatus(false);
            }
        }

        //On Restart button under Game Over Panel
        public void RestartGame()
        {
            _mainMenuPanel.SetActive(true);
            _mainGameplayPanel.SetActive(false);
            _gameOverPanel.SetActive(false);
            GameManager.instance.OnGameRestart?.Invoke();
            clickBlinkCoroutine = StartCoroutine(ClickRate());
            bgm_Source.Play();
            currentScoreTxt.text = "0";
        }

        //On Start button under Main Menu Panel
        public void StartGame()
        {
            _mainMenuPanel.SetActive(false);
            _mainGameplayPanel.SetActive(true);

            GameManager.instance.OnGameStarted?.Invoke();
            GameManager.instance.gameStarted = true;
            // bgm_Source.Play();

            if (clickBlinkCoroutine != null)
                StopCoroutine(clickBlinkCoroutine);
        }

        //On Pause and Resume button, under Main Gameplay Panel
        public void ToggleGameStatus(bool pauseEnabled)
        {
            //Debug.Log($"Toggle Game Status called, status : {pauseEnabled}");
            if (pauseEnabled)
            {
                GameManager.instance.gamePaused = true;
                Time.timeScale = 0f;
                pausedBanner.SetActive(true);
            }
            else
            {
                Time.timeScale = 1f;
                pausedBanner.SetActive(false);
                Invoke(nameof(SetGamePauseStatus), 0.1f);
            }
        }

        private void SetGamePauseStatus()
        {
            GameManager.instance.gamePaused = false;
        }

        private void UpdateScore()
        {
            GameManager.instance.Score++;
            currentScoreTxt.text = GameManager.instance.Score.ToString();
        }

        private IEnumerator ClickRate()
        {
            //Debug.Log($"ClickBlink called");
            float tempTime = 0f;
            byte startYVal = 140, finalYVal = 120, switchStatus = 0;        //140 | 120, 137 | 126

            while (true)
            {
                tempTime += blinkRateMultiplier * Time.deltaTime;

                if (tempTime >= 1)
                {
                    tempTime = 0f;

                    if (switchStatus == 0)
                    {
                        _clickImg.anchoredPosition = new Vector2(0f, finalYVal);
                        _spaceBarImg.anchoredPosition = new Vector2(0f, finalYVal + 6);
                        switchStatus = 1;
                    }
                    else
                    {
                        _clickImg.anchoredPosition = new Vector2(0f, startYVal);
                        _spaceBarImg.anchoredPosition = new Vector2(0f, startYVal - 3);
                        switchStatus = 0;
                    }
                }

                yield return null;
            }
        }
    }
}
