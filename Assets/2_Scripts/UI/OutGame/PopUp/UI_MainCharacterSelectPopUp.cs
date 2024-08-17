using _2_Scripts.UI.OutGame.Lobby;
using _2_Scripts.Utils;
using Cargold.FrameWork.BackEnd;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.PopUp
{
    public class UI_MainCharacterSelectPopUp : SerializedMonoBehaviour ,ISortPopUp
    {
        enum EggType
        {
            Blue,
            Red,
            Green
        }
        
        [SerializeField]
        private Dictionary<EggType,Button> eggButtons;
        [SerializeField]
        private GameObject mBackPanel;
        private GameMessage<Define.RewardEvent> mRewardEvent;
        private void Start()
        {
            mRewardEvent = new GameMessage<Define.RewardEvent>(EGameMessage.RewardOpenPopUp, null);
            foreach (var btn in eggButtons)
            {
                btn.Value.onClick.AddListener(() =>
                {
                    GrantMainCharacter(btn.Key);
                });
            }
        }


        private void GrantMainCharacter(EggType type)
        {
            string key = "";
            switch (type)
            {
                case EggType.Blue:
                    key = "Aquarine";
                    break;
                case EggType.Red:
                    key = "Rev";
                    break;
                case EggType.Green:
                    key = "Bird";
                    break;
            }

            var data = BackEndManager.Instance.UserMainCharacterData[key];
            data.AddAmount(1);
            data.EquipMainCharacter();
            var mainCharacterInfo = 
                GameManager.Instance.MainCharacterList.
                    FirstOrDefault(m => m.name == key).CharacterEvolutions[1].GetData;
            mRewardEvent.SetValue(new Define.RewardEvent()
            {
                count = 1,
                name = mainCharacterInfo.GetCharacterName(),
                sprite = mainCharacterInfo.Icon
            }); 
            MessageBroker.Default.Publish(mRewardEvent);
            IsPopUpEnd = true;
            gameObject.SetActive(false);
            mBackPanel.gameObject.SetActive(false);
            BackEndManager.Instance.IsSelectMainCharacter = true;
            BackEndManager.Instance.SaveCharacterData();
        }
        
        public int SortIndex { get; set; } = 2;
        public bool IsPopUpEnd { get; set; } = false;

        public void OnPopUp()
        {
            mBackPanel.gameObject.SetActive(true);
            this.gameObject.SetActive(true);
        }
    }
}