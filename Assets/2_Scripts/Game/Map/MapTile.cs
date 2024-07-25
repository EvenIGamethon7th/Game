using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace _2_Scripts.Game.Map
{
    public class MapTile : MonoBehaviour
    {
        [field: SerializeField]
        public Tilemap UnitTile { get; private set; }
        [field: SerializeField]
        public Tilemap MonsterTile { get; private set; }
    }
}