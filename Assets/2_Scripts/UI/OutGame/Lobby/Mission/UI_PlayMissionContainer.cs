using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace _2_Scripts.UI.OutGame.Lobby
{
    public class UI_PlayMissionContainer : SerializedMonoBehaviour
    {
        [SerializeField]
        private UI_PlayMissionItem mPlayMissionItemPrefab;

        [SerializeField] private SO_PlayMissionTable mMissionTable;

        private void Start()
        {
            foreach (var soPlayMission in mMissionTable.PlayMissionTable)
            {
                soPlayMission.Value.InitMission(soPlayMission.Key);
                var playMissionItem = Instantiate(mPlayMissionItemPrefab, transform);
                playMissionItem.InitItem(soPlayMission.Key, soPlayMission.Value);
            }
        }
    }
}