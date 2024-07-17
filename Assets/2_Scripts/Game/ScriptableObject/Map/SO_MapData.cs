using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _2_Scripts.Game.ScriptableObject.Map
{
    [CreateAssetMenu(menuName = "ScriptableObject/Map/MapData")]
    public class SO_MapData : SerializedScriptableObject
    {
        [Title("맵 데이터")]
        [SerializeField]
        public Dictionary<string, MapData> MapDataTable { get; private set; }
    }

    [Serializable]
    public class MapData
    {
        [SerializeField]
        public Tile BackGroundTile { get; private set; }
        [SerializeField]
        public Tile MonsterPathTile { get; private set; }
        [SerializeField]
        public GameObject DecorateObject { get; private set; }
    } 
}