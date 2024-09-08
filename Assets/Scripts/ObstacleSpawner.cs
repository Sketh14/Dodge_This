// #define MULTIPLE_OBSTACLE

using System.Collections.Generic;
using UnityEngine;

namespace Dodge_This
{
    public class ObstacleSpawner : MonoBehaviour
    {
        //[Header("Test Variables")]
        //[SerializeField] private bool enableSpawn;

        [SerializeField] private float[] spawnPointsY;              //Why Vector3??
        [SerializeField] private float spawnTime;
        [SerializeField] private bool spawnEnabled = false;                  //Serialize for test
        private float timeAtSpawn;

        [Header("Obstacle")]
        [SerializeField] private byte _obstacleCount;
        [SerializeField] private GameObject _obstaclePrefab;
        private GameObject _tempObstacle;
        private Queue<GameObject> _obstaclePool;

#if MULTIPLE_OBSTACLE
        [SerializeField] private byte _obstacleIndex = 0, totalObstacles = 1;
        [SerializeField] private ObstacleTag[] obstacleTags;
#endif

        // [Header("Local Refernce Script")]
        // [SerializeField] private GameLogic localGameLogic;

        private void OnDestroy()
        {

            GameManager.instance.OnGameStarted -= InvokeObstacleSpawn;
            GameManager.instance.OnPlayerUnAlive -= ToggleSpawn;
        }

        // Start is called before the first frame update
        void Start()
        {
            //Invoke(nameof(SpawnObstacle), initialSpawnTime);
            //startPosX = transform.position.x;
            _obstaclePool = new Queue<GameObject>();
            AllocatePool();

            GameManager.instance.OnGameStarted += InvokeObstacleSpawn;
            GameManager.instance.OnPlayerUnAlive += ToggleSpawn;
        }

        private void InvokeObstacleSpawn()
        {
            spawnEnabled = true;
            Invoke(nameof(SpawnObstacle), 0f);
        }

        private void ResetStats()
        {
            transform.localPosition = Vector3.zero;
            ToggleSpawn(false);
            Debug.Log($"Calling For Reset : {spawnEnabled}");
        }

        #region ObstaclePool
        private void AllocatePool()
        {
            GameObject poolHolder = new GameObject("Obstacle_Pool");
            poolHolder.transform.parent = transform;

            for (int i = 0; i < _obstacleCount; i++)
            {
                GameObject tempObstacle = Instantiate(_obstaclePrefab, poolHolder.transform);
                tempObstacle.name = tempObstacle.ToString() + i;
                tempObstacle.SetActive(false);

                _obstaclePool.Enqueue(tempObstacle);
            }
        }

        public GameObject GetObstacle()
        {
            _tempObstacle = _obstaclePool.Dequeue();

            _tempObstacle.transform.position = new Vector2(20f, 7f);           //(20, 7) for staying out of camera perspective
            //tempObstacle.SetActive(true);

            _obstaclePool.Enqueue(_tempObstacle);
            return _tempObstacle;
        }
        #endregion ObstaclePool


        private void ContinueMainGameplay()
        {
            Invoke(nameof(ToggleSpawnHelper), 1f);
        }

        private void ToggleSpawnHelper() { ToggleSpawn(true); }
        private void ToggleSpawn(bool toggleValue)
        {
            //As this is called multiple times. i.e. at restart is twice called
            //if (GameManager.instance.gameStarted)
            {
                spawnEnabled = toggleValue;
                // Debug.Log($"spawnEnabled : {spawnEnabled}");
                //Debug.Log($"Time Now : {Time.unscaledTime}");

                //In case the next obstacle is in the process of spawning and gets stopped as the spawn variable is not enabled
                //Invoke the spawn func with the time left before the pause button was clicked
                if (spawnEnabled)
                    Invoke(nameof(SpawnObstacle), spawnTime - timeAtSpawn);
                else
                {
                    //Get the time difference between the invoke time and the time went when the pause button was clicked
                    timeAtSpawn = Time.unscaledTime - timeAtSpawn;
                    CancelInvoke(nameof(SpawnObstacle));
                }
            }
            //else
            //this.enabled = false;               //If Called again during Restart

            //Debug.Log($"Toggle Spawn status : {spawnEnabled}");
        }

#if MULTIPLE_OBSTACLE
        private byte ChooseObstacleGroup()
        {
            byte obstacleUnitIndex;

            obstacleUnitIndex = (byte)Random.Range(1, totalObstacles);           //0 would be for Test, for future

            return obstacleUnitIndex;
        }
#endif

        public void SpawnObstacle()
        {
            //enableSpawn = false;
            timeAtSpawn = Time.unscaledTime;

#if MULTIPLE_OBSTACLE
            obstacleIndex = 0;
            //obstacleIndex = ChooseObstacleGroup();                   //Uncomment for Future additions if more obstacles are added
            _tempObstacle = ObstaclePoolManager.instance.ReUseObstacle(obstacleTags[obstacleIndex], Quaternion.identity);
#endif

            _tempObstacle = GetObstacle();
            SetObstaclePosition(ref _tempObstacle);

            _tempObstacle.SetActive(true);
            //Debug.Log($"Object : {tempObstacle.name}, status : {tempObstacle.activeSelf}");

            if (spawnEnabled)
                Invoke(nameof(SpawnObstacle), Random.Range(0.2f, spawnTime));
            //Debug.Log($"Spawning Obstacle : {obstacleGroups[obstacleGroupIndex].name}, Spawn After : {obstacleGroups[obstacleGroupIndex].spawnNextAfter}");
        }

        private void SetObstaclePosition(ref GameObject obstacleToBePlaced)
        {
            bool marginLeft = (Random.Range(0, 10) / 2 == 0) ? true : false;
            obstacleToBePlaced.GetComponent<ObstacleController>().OnLeftMargin = marginLeft;
            obstacleToBePlaced.GetComponent<ObstacleController>().SetStats(ref marginLeft);

            byte spawnPointIndex;
            spawnPointIndex = (byte)Random.Range(1, spawnPointsY.Length);           //0 would be for Test, for future

            if (marginLeft)
                obstacleToBePlaced.transform.position = new Vector2(-11.5f, spawnPointsY[spawnPointIndex]);
            else
                obstacleToBePlaced.transform.position = new Vector2(11.5f, spawnPointsY[spawnPointIndex]);
            //Debug.Log($"obstacleUnitIndex : {obstacleUnitIndex} ,tempSpawnPos : {tempSpawnPos}, addDisX : {addDisX}, addDisY : {addDisY}");
        }
    }
}