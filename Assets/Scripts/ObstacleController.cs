using System.Collections;
using UnityEngine;

namespace Dodge_This
{
    //[ExecuteInEditMode]
    public class ObstacleController : MonoBehaviour
    {
        [SerializeField] private ObstacleTag obstacleTag;
        public bool DeActivated, OnLeftMargin;
        private float startPosX, marginPosX = 12f;
        [Range(0.1f, 1f)]
        [SerializeField] private float _speedMultiplier = 1f;        //0.4
        [SerializeField] private float _ogRotateSpeed = 2f;            //6
        private float rotateSpeed;

        private void OnEnable()
        {
            Invoke(nameof(CheckWithinViewPort), 0f);
            _ = StartCoroutine(MoveToOtherMargin());

            rotateSpeed = _ogRotateSpeed;
            if (OnLeftMargin)
                rotateSpeed *= -1f;
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
            if (!DeActivated)
            {
                if (OnLeftMargin)
                {
                    if (transform.position.x > (marginPosX - 0.5f))
                    {
                        DeActivated = true;
                        gameObject.SetActive(false);

                        if (GameManager.instance.gameStarted)
                            GameManager.instance.OnPlayerScored?.Invoke();
                        //Debug.Log($"Scored Right : {transform.name}");
                    }
                }
                else
                {
                    if (transform.position.x < (-marginPosX + 0.5f))
                    {
                        DeActivated = true;
                        gameObject.SetActive(false);

                        if (GameManager.instance.gameStarted)
                            GameManager.instance.OnPlayerScored?.Invoke();
                        //Debug.Log($"Scored Left : {transform.name}");
                    }
                }
            }
            Invoke(nameof(CheckWithinViewPort), 1f);
        }

        public void SetStats(ref bool margin)
        {
            DeActivated = false;
            OnLeftMargin = margin;

            if (OnLeftMargin)
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
                    tempTime += (_speedMultiplier * 2f) * Time.deltaTime;
                else
                    tempTime += _speedMultiplier * Time.deltaTime;

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