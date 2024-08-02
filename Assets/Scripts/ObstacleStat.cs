using UnityEngine;

namespace Dodge_This
{
    [SerializeField] public enum ObstacleTag { Gear }

    [CreateAssetMenu(fileName = "ObstacleStat", menuName = "ObstacleStat")]
    public class ObstacleStat : ScriptableObject
    {
        public ObstacleTag tag;
        public bool activated;
    }
}