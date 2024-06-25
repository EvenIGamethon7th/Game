using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _2_Scripts.ScriptableObjects
{
    [CreateAssetMenu]
    public class TileData : SerializedScriptableObject
    {
        [SerializeField]
        public TileBase[] Tiles { get; private set; }
        [SerializeField]
        public bool IsGround { get; private set; }
    }
}