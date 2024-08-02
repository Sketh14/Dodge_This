using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dodge_This
{
    [Serializable]
    public class ObstaclePool
    {
        public GameObject prefab;
        public ObstacleTag tag;
        public byte count;
    }

    public class ObstaclePoolManager : MonoBehaviour
    {
        public ObstaclePool[] obstaclesPool;
        public Dictionary<string, Queue<GameObject>> obstaclesDictionary;


        #region Singleton
        private static ObstaclePoolManager _instance;
        public static ObstaclePoolManager instance { get { return _instance; } }
        #endregion Singleton

        // Start is called before the first frame update
        void Awake()
        {
            if (instance == null && instance != this)
                _instance = this;
            else
                Destroy(instance);
        }

        private void Start()
        {
            obstaclesDictionary = new Dictionary<string, Queue<GameObject>>();
            AllocatePool();
        }

        private void AllocatePool()
        {
            foreach (ObstaclePool obstaclePool in obstaclesPool)
            {
                GameObject poolHolder = new GameObject(obstaclePool.tag + "_Pool");
                poolHolder.transform.parent = transform;

                Queue<GameObject> pool = new Queue<GameObject>();

                for (int i = 0; i < obstaclePool.count; i++)
                {
                    GameObject tempObstacle = Instantiate(obstaclePool.prefab, poolHolder.transform);
                    tempObstacle.name = obstaclePool.tag.ToString() + i;
                    tempObstacle.SetActive(false);

                    pool.Enqueue(tempObstacle);
                }

                obstaclesDictionary.Add(obstaclePool.tag.ToString(), pool);
            }

        }

        public GameObject ReUseObstacle(ObstacleTag tag, Quaternion Rot)
        {
            if (!obstaclesDictionary.ContainsKey(tag.ToString()))
            {
                Debug.LogError($"Dictionary does not contain pool with tag : {tag}");
            }

            GameObject tempObstacle = obstaclesDictionary[tag.ToString()].Dequeue();

            tempObstacle.transform.position = new Vector2(20f, 7f);           //(20, 7) for staying out of camera perspective
            tempObstacle.transform.rotation = Rot;
            //tempObstacle.SetActive(true);

            obstaclesDictionary[tag.ToString()].Enqueue(tempObstacle);
            return tempObstacle;
        }
    }
}
