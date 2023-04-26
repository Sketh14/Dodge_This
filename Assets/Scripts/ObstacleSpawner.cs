//#define TEST_MODE

using UnityEngine;

namespace Double_Jump
{
    public class ObstacleSpawner : MonoBehaviour
    {
        //[Header("Test Variables")]
        //[SerializeField] private bool enableSpawn;

        [SerializeField] private float[] spawnPointsY;              //Why Vector3??
        [SerializeField] private float spawnTime, timeAtSpawn;
        [SerializeField] private byte obstacleIndex = 0, totalObstacles = 1;
        [SerializeField] private bool spawnEnabled = false;                  //Serialize for test
        [SerializeField] private ObstacleTag[] obstacleTags;

        [Header("Local Refernce Script")]
        [SerializeField] private GameLogic localGameLogic;

        private void OnEnable()
        {
            localGameLogic.OnGameStarted += InvokeObstacleSpawn;
            localGameLogic.OnPlayerUnAlive += ToggleSpawn;
        }

        private void OnDisable()
        {
            localGameLogic.OnGameStarted -= InvokeObstacleSpawn;
            localGameLogic.OnPlayerUnAlive -= ToggleSpawn;
        }

        // Start is called before the first frame update
        void Start()
        {
            //Invoke(nameof(SpawnObstacle), initialSpawnTime);
            //startPosX = transform.position.x;
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

        public void SpawnObstacle()
        {
            //enableSpawn = false;
            timeAtSpawn = Time.unscaledTime;

#if TEST_MODE
            obstacleIndex = 0;
#else
            obstacleIndex = 0;
            //obstacleIndex = ChooseObstacleGroup();                   //Uncomment for Future additions if more obstacles are added
#endif

            GameObject tempObstacle = ObstaclePoolManager.instance.ReUseObstacle(obstacleTags[obstacleIndex], Quaternion.identity);
            SetObstaclePosition(ref tempObstacle);

            tempObstacle.SetActive(true);
            //Debug.Log($"Object : {tempObstacle.name}, status : {tempObstacle.activeSelf}");

            if (spawnEnabled)
                Invoke(nameof(SpawnObstacle), Random.Range(0.2f, spawnTime));
            //Debug.Log($"Spawning Obstacle : {obstacleGroups[obstacleGroupIndex].name}, Spawn After : {obstacleGroups[obstacleGroupIndex].spawnNextAfter}");
        }

        private void ContinueMainGameplay()
        {
            Invoke(nameof(InvokeToggleSpawn), 1f);
        }

        private void InvokeToggleSpawn()
        {
            ToggleSpawn(true);
        }

        private void ToggleSpawn(bool toggleValue)
        {
            //As this is called multiple times. i.e. at restart is twice called
            //if (GameManager.instance.gameStarted)
            {
                spawnEnabled = toggleValue;
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

        private byte ChooseObstacleGroup()
        {
            byte obstacleUnitIndex;

            obstacleUnitIndex = (byte)Random.Range(1, totalObstacles);           //0 would be for Test, for future

            return obstacleUnitIndex;
        }

        private void SetObstaclePosition(ref GameObject obstacleToBePlaced)
        {
            bool marginLeft = (Random.Range(0, 10) / 2 == 0) ? true : false;
            obstacleToBePlaced.GetComponent<ObstacleController>().onLeftMargin = marginLeft;
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