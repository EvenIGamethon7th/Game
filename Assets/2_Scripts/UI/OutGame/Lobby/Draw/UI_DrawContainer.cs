using _2_Scripts.Game.BackEndData.MainCharacter;
using _2_Scripts.Utils;
using Cargold.FrameWork.BackEnd;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.Lobby.Draw
{
    public class UI_DrawContainer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI mDrawCountText;
        [SerializeField] private Button mDiaButton;
        [SerializeField] private Button mTicketButton;
        private GameMessage<List<Define.RewardEvent>> mRewardEventMessage;
        private List<Define.RewardEvent> mRewardList = new List<Define.RewardEvent>()
        {
            new Define.RewardEvent()
            {
                count = 1,
                name = "",
                sprite = null
            }
        };

        private Define.RewardEvent mRewardEvent;
        private const int DRAW_COST_DIA = 850;
        private void Start()
        {
            BackEndManager.Instance.UserCurrency[ECurrency.Ticket].Subscribe(ticket =>
            {
                mDrawCountText.text = $"{ticket}개 보유";
            });
            mRewardEvent = mRewardList[0];
            mRewardEventMessage = new GameMessage<List<Define.RewardEvent>>(EGameMessage.RewardOpenPopUp, mRewardList);
            mDiaButton.onClick.AddListener(OnDiaButton);
            mTicketButton.onClick.AddListener(OnTicketButton);
        }

        private void OnTicketButton()
        {
            if(BackEndManager.Instance.UserCurrency[ECurrency.Ticket].Value <= 0)
            {
                UI_Toast_Manager.Instance.Activate_WithContent_Func("티켓이 부족합니다.");
                return;
            }
            BackEndManager.Instance.AddCurrencyData(ECurrency.Ticket,-1);
            RandomDraw();
            MessageBroker.Default.Publish(mRewardEventMessage);
        }

        private void OnDiaButton()
        {
            if(BackEndManager.Instance.UserCurrency[ECurrency.Diamond].Value < DRAW_COST_DIA)
            {
                UI_Toast_Manager.Instance.Activate_WithContent_Func("다이아가 부족합니다."); 
                return;
            }
            BackEndManager.Instance.AddCurrencyData(ECurrency.Diamond,-DRAW_COST_DIA);
            RandomDraw();
            MessageBroker.Default.Publish(mRewardEventMessage);
        }

        private void RandomDraw()
        {
            List<string> rewardList = BackEndManager.Instance.UserMainCharacterData.Keys.ToList();
            int randomIndex = Random.Range(0, rewardList.Count);
            var mainCharacterData = BackEndManager.Instance.UserMainCharacterData[rewardList[randomIndex]];
            var infoData = GameManager.Instance.MainCharacterList.FirstOrDefault(x => x.name == mainCharacterData.key)
                .CharacterEvolutions[1].GetData;
            mRewardEvent.name = infoData.GetCharacterName();
            mRewardEvent.sprite = infoData.Icon;
            mRewardEvent.rewardEvent = () =>
            {
                var mainCharacter = BackEndManager.Instance.UserMainCharacterData[mainCharacterData.key];
                if (mainCharacter.isGetType == EGetType.Lock)
                {
                    mainCharacter.isGetType = EGetType.Unlock;
                }
                mainCharacter.amount++;
                BackEndManager.Instance.SaveCharacterData();
            };
        }
    }
}