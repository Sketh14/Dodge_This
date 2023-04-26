using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Double_Jump
{
    public class MainGameUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private RawImage clickImg;
        private float blinkRateMultiplier = 1f;                //blink rate of 1f is enough
        [SerializeField] private TMP_Text currentScoreTxt, finalScoreTxt;

        [Header("Local Reference Script")]
        [SerializeField] private GameLogic localGameLogic;

        [Header("Scoring")]
        private int score;
        private Coroutine clickBlinkCoroutine;

        private void OnEnable()
        {
            localGameLogic.OnPlayerUnAlive += ShowGameOverPanel;
            localGameLogic.OnPlayerScored += UpdateScore;
        }

        private void OnDisable()
        {
            localGameLogic.OnPlayerUnAlive -= ShowGameOverPanel;
            localGameLogic.OnPlayerScored -= UpdateScore;
        }

        private void ShowGameOverPanel(bool dummyData)
        {
            gameOverPanel.SetActive(true);
            finalScoreTxt.text = score.ToString();
        }

        private void Start()
        {
            clickBlinkCoroutine = StartCoroutine(ClickBlink());
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

            if (clickBlinkCoroutine != null)
                StopCoroutine(clickBlinkCoroutine);
        }

        private void UpdateScore()
        {
            score++;
            currentScoreTxt.text = score.ToString();
        }

        private IEnumerator ClickBlink()
        {
            Debug.Log($"ClickBlink called");
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
