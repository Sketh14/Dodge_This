//#define MOBILE_CONTROLS                           //Uncomment for real gameplay

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Double_Jump
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
        [SerializeField] private GameObject[] player_PS;

        [Header("Local Reference Script")]
        [SerializeField] private GameLogic localGameLogic;

        [Header("Audio Clip")]
        [SerializeField] private AudioClip[] clips_SE;
        [SerializeField] private AudioSource playerAudioSource;
            
        private void OnEnable()
        {
            localGameLogic.OnGameRestart += ResetPlayerStats;   
        }

        private void OnDisable()
        {
            localGameLogic.OnGameRestart -= ResetPlayerStats;
        }

        // Start is called before the first frame update
        private void Start()
        {

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
            localGameLogic.OnPlayerUnAlive?.Invoke(false);
            transform.GetComponent<SpriteRenderer>().enabled = false;
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
            transform.GetComponent<SpriteRenderer>().enabled = true;
            playerRb.bodyType = RigidbodyType2D.Dynamic;
            transform.position = new Vector2(0f, -3.12f);

            for (int i = 0; i < 2; i++)
            {
                player_PS[i].SetActive(false);
            }
        }
    }
}