using Sirenix.OdinInspector;
using System;
using System.Linq;
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
            foreach (var soPlayMission in   mMissionTable.PlayMissionTable.OrderBy(x => x.Value.SortNum)
                         .ToDictionary(x => x.Key, x => x.Value))
            {
                soPlayMission.Value.InitMission(soPlayMission.Key);
                var playMissionItem = Instantiate(mPlayMissionItemPrefab, transform);
                playMissionItem.InitItem(soPlayMission.Key, soPlayMission.Value);
            }
        }
    }
}