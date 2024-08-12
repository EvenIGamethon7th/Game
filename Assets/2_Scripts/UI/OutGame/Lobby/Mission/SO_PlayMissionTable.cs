using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace _2_Scripts.UI.OutGame.Lobby
{
    [CreateAssetMenu(fileName = "SO_PlayMissionTable", menuName = "ScriptableObject/PlayMissionTable")]
    public class SO_PlayMissionTable : SerializedScriptableObject
    {
        [SerializeField]
        public Dictionary<string,SO_PlayMission> PlayMissionTable { get; private set; }
       
        
    }
}