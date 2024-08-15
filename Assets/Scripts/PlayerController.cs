//#define MOBILE_CONTROLS                           //Uncomment for real gameplay

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Dodge_This
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D playerRb;

        [Range(25f, 30f)]
        [SerializeField] private float jumpForce;

        [Space]
        private bool unAlive = false;
        private byte jumpCount;

        [Header("Particle System")]
        [SerializeField] private GameObject[] player_PS;                //0,1 : Player Death | 2 : Player Hit Ground Snow

        [Header("Audio Clip")]
        [SerializeField] private AudioClip[] clips_SE;
        [SerializeField] private AudioSource playerAudioSource;

        private void OnDisable()
        {
            GameManager.instance.OnGameRestart -= ResetPlayerStats;
        }

        private void OnEnable()
        {
            GameManager.instance.OnGameRestart += ResetPlayerStats;
        }

        // Update is called once per frame
        private void Update()
        {
            if (!GameManager.instance.gamePaused && GameManager.instance.gameStarted && !unAlive && jumpCount < 2)
            {
#if MOBILE_CONTROLS
                if (Touch.activeTouches.Count > 0 && Touch.activeTouches[0].phase == UnityEngine.InputSystem.TouchPhase.Began)
#else
                if (UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame)
#endif
                {
                    //Debug.Log($"Pressed");
                    jumpCount++;
                    playerRb.velocity = new Vector2(0f, jumpForce);
                    playerAudioSource.PlayOneShot(clips_SE[0]);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!unAlive && GameManager.instance.gameStarted)
            {
                if (collision.CompareTag("Obstacle"))
                {
                    //Debug.Log($"Got Hit : {collision.name}");             //Working
                    UnAlive();
                }

                if (collision.CompareTag("Ground"))
                {
                    jumpCount = 0;
                    playerRb.velocity = Vector2.zero;
                    player_PS[2].GetComponent<ParticleSystem>().Play();

                    //Debug.Log($"Resetting Jump Counter : {jumpCount}");
                }
            }
        }

        private void UnAlive()
        {
            unAlive = true;
            GameManager.instance.OnPlayerUnAlive?.Invoke(false);
            transform.GetChild(0).gameObject.SetActive(false);
            GameManager.instance.gameStarted = false;

            playerAudioSource.PlayOneShot(clips_SE[1]);

            //Reset RigidBody2D
            playerRb.bodyType = RigidbodyType2D.Kinematic;
            playerRb.velocity = Vector2.zero;

            for (int i = 0; i < 2; i++)
            {
                player_PS[i].SetActive(true);
            }
            //Debug.Log($"Player UnAlive : {unAlive}");
        }

        private void ResetPlayerStats()
        {
            unAlive = false;
            jumpCount = 0;
            playerRb.bodyType = RigidbodyType2D.Dynamic;
            transform.position = new Vector2(0f, -3.12f);
            transform.GetChild(0).gameObject.SetActive(true);

            for (int i = 0; i < 2; i++)
            {
                player_PS[i].SetActive(false);
            }
        }
    }
}