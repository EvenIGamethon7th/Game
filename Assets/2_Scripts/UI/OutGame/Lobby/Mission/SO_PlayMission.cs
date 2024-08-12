using _2_Scripts.Utils;
using _2_Scripts.Utils.Components;
using Cargold.FrameWork.BackEnd;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace _2_Scripts.UI.OutGame.Lobby
{
    [Serializable]
    public class SO_PlayMission
    {
        [SerializeField]
       public List<MissionClearCondition> ClearConditions { get; private set; }
       [SerializeField]
       public ItemKey ItemKey { get; private set; }
       [SerializeField]
       public int Amount { get; private set; }

       [SerializeField]
       public int SortNum { get; private set; }

       [SerializeField]
       public MissionReward ItemAcquisition;
       private PlayMission Mission;

       public int GetCurrentProgress()
       {
           return ClearConditions.Sum(x => x.GetCurrentProgress());
       }
       public int GetMaxProgress()
       {
           return ClearConditions.Sum(x => x.GetMaxProgress());
       }
       
       public void InitMission(string missionKey)
       {
           Mission = BackEndManager.Instance.UserPlayMission[missionKey];
       }

       public bool IsClearMission()
       {
         return ClearConditions.All(x => x.IsClear());
       }
       public bool ShouldGrantReward()
       {
           if(Mission.IsClear || !IsClearMission())
           {
               return false;
           }
           Mission.IsClear = true;
           MessageBroker.Default.Publish(new GameMessage<Define.RewardEvent>(EGameMessage.RewardOpenPopUp,new Define.RewardEvent
           {
               count = Amount,
               name = ItemKey.GetData.name,
               sprite = ItemKey.GetData.Icon,
               rewardEvent = () =>
               {
                   BackEndManager.Instance.SaveCharacterData();
               }
           }));
           ItemAcquisition.ItemAcquisition.AcquireItem(ItemKey,Amount);
           return true;
       } 
    }
}