//#define MOBILE_CONTROLS                           //Uncomment for real gameplay

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Double_Jump
{
    public class MainGameUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private GameObject gameOverPanel, pausedBanner;
        [SerializeField] private RawImage clickImg;
        private float blinkRateMultiplier = 1f;                //blink rate of 1f is enough
        [SerializeField] private TMP_Text currentScoreTxt, finalScoreTxt;

        [Header("Local Reference Script")]
        [SerializeField] private GameLogic localGameLogic;

        [Header("Scoring")]
        private int score;
        private Coroutine clickBlinkCoroutine;

        [Header("Animation")]
        [SerializeField] private Animator cameraAnimator;
        [SerializeField] private string[] cameraAnimatorStates;

        [Header("SFX")]
        [SerializeField] private AudioSource bgm_Source;

        private void OnEnable()
        {
            localGameLogic.OnPlayerUnAlive += InvokeGameOverUpdate;
            localGameLogic.OnPlayerScored += UpdateScore;
        }

        private void OnDisable()
        {
            localGameLogic.OnPlayerUnAlive -= InvokeGameOverUpdate;
            localGameLogic.OnPlayerScored -= UpdateScore;
        }

        private void InvokeGameOverUpdate(bool dummyData)
        {
            cameraAnimator.Play(cameraAnimatorStates[0], 0, 0f);
            Invoke(nameof(ShowGameOverPanel), 0.6f);
        }

        private void ShowGameOverPanel()
        {
            gameOverPanel.SetActive(true);
            finalScoreTxt.text = score.ToString();
            bgm_Source.Stop();
        }

        private void Start()
        {
            clickBlinkCoroutine = StartCoroutine(ClickBlink());
        }

        private void Update()
        {
#if MOBILE_CONTROLS
            if (gamePaused && Touch.activeTouches.Count > 0 && Touch.activeTouches[0].phase == UnityEngine.InputSystem.TouchPhase.Began)
#else
            if (GameManager.instance.gamePaused && UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                ToggleGameStatus(false);
            }
#endif
        }

        //On Restart button under Game Over Panel
        public void RestartGame()
        {
            localGameLogic.OnGameRestart?.Invoke();
            clickBlinkCoroutine = StartCoroutine(ClickBlink());
        }

        //On Start button under Main Menu Panel
        public void StartGame()
        {
            localGameLogic.OnGameStarted?.Invoke();
            GameManager.instance.gameStarted = true;
            bgm_Source.Play();

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
            score++;
            currentScoreTxt.text = score.ToString();
        }

        private IEnumerator ClickBlink()
        {
            //Debug.Log($"ClickBlink called");
            float tempTime = 0f;
            byte alphaVal0 = 0, alphaVal1 = 1;

            while (true)
            {
                tempTime += blinkRateMultiplier * Time.deltaTime;

                if (tempTime >= 1)
                {
                    tempTime = 0f;

                    byte tempAlphaVal = alphaVal0;
                    alphaVal0 = alphaVal1;
                    alphaVal1 = tempAlphaVal;

                    clickImg.color = new Color(1f, 1f, 1f, alphaVal0);
                }

                yield return null;
            }
        }
    }
}
