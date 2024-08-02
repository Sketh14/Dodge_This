using System.Collections;
using UnityEngine;

namespace Dodge_This
{
    //[ExecuteInEditMode]
    public class ObstacleController : MonoBehaviour
    {
        [SerializeField] private ObstacleTag obstacleTag;
        public bool activated, onLeftMargin;
        private float startPosX, marginPosX = 12f;
        [Range(0.1f, 1f)]
        [SerializeField] private float speedMultiplier = 1f;
        [SerializeField] private float rotateSpeed = 2f;

        [Header("Local Refernce Script")]
        [SerializeField] private GameLogic localGameLogic;

        private void OnEnable()
        {
            localGameLogic = GameManager.instance.gameLogicReference;
            Invoke(nameof(CheckWithinViewPort), 0f);
            _ = StartCoroutine(MoveToOtherMargin());
        }

        private void OnDisable()
        {
            CancelInvoke(nameof(CheckWithinViewPort));
        }

        private void FixedUpdate()
        {
            transform.Rotate(new Vector3(0f, 0f, rotateSpeed));
        }

        private void CheckWithinViewPort()
        {
            if (!activated)
            {

                if (onLeftMargin)
                {
                    if (transform.position.x > (marginPosX - 0.5f))
                    {
                        activated = true;
                        gameObject.SetActive(false);

                        if (GameManager.instance.gameStarted)
                            localGameLogic.OnPlayerScored?.Invoke();
                        //Debug.Log($"Scored Right : {transform.name}");
                    }
                }
                else
                {
                    if (transform.position.x < (-marginPosX + 0.5f))
                    {
                        activated = true;
                        gameObject.SetActive(false);

                        if (GameManager.instance.gameStarted)
                            localGameLogic.OnPlayerScored?.Invoke();
                        //Debug.Log($"Scored Left : {transform.name}");
                    }
                }
            }
            Invoke(nameof(CheckWithinViewPort), 1f);
        }

        public void SetStats(ref bool margin)
        {
            activated = false;
            onLeftMargin = margin;

            if (onLeftMargin)
                startPosX = -marginPosX;
            else
                startPosX = marginPosX;
        }

        private IEnumerator MoveToOtherMargin()
        {
            float tempTime = 0;
            while (true)
            {
                if (!GameManager.instance.gameStarted)
                    tempTime += (speedMultiplier * 2f) * Time.deltaTime;
                else
                    tempTime += speedMultiplier * Time.deltaTime;

                if (tempTime >= 1)
                {
                    tempTime = 0;
                    break;
                }

                transform.position = new Vector2(Mathf.Lerp(startPosX, (startPosX * -1f), tempTime), transform.position.y);         //-1f as to be the opposite
                yield return null;
            }
        }
    }
}