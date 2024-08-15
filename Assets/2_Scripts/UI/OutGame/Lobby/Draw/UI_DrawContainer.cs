using _2_Scripts.Game.BackEndData.MainCharacter;
using _2_Scripts.Game.ScriptableObject.Character;
using _2_Scripts.Game.Sound;
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
        [SerializeField] private Button mProductDetailButton;
        [SerializeField] private ProductDetailsKey mProductDetailsKey;
        private GameMessage<List<Define.RewardEvent>> mRewardEventMessage;
        private GameMessage<ProductDetailsData> mProductDetailOpenMessage;
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
            mProductDetailOpenMessage =
                new GameMessage<ProductDetailsData>(EGameMessage.ProductDetailPopUp, mProductDetailsKey.GetData);
            BackEndManager.Instance.UserCurrency[ECurrency.Ticket].Subscribe(ticket =>
            {
                mDrawCountText.text = $"{ticket}개 보유";
            }).AddTo(this);
            mProductDetailButton.onClick.AddListener(() =>
            {
                MessageBroker.Default.Publish(mProductDetailOpenMessage);
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
                UI_Toast_Manager.Instance.Activate_WithContent_Func("뽑기권이 부족합니다.");
                return;
            }
            if (!RandomDraw())
            {
                UI_Toast_Manager.Instance.Activate_WithContent_Func("모든 뽑기를 완료하였습니다!\n 다음 업데이트를 기대해주세요."); 
                return;
            }
            SoundManager.Instance.Play2DSound(AddressableTable.Sound_Get_Item);
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
            if (!RandomDraw())
            {
                UI_Toast_Manager.Instance.Activate_WithContent_Func("모든 뽑기를 완료하였습니다!\n 다음 업데이트를 기대해주세요."); 
                return;
            }
            SoundManager.Instance.Play2DSound(AddressableTable.Sound_Get_Item);
            BackEndManager.Instance.AddCurrencyData(ECurrency.Diamond,-DRAW_COST_DIA);
            RandomDraw();
            MessageBroker.Default.Publish(mRewardEventMessage);
        }

        private bool RandomDraw()
        {
            var availableCharacters = BackEndManager.Instance.UserMainCharacterData.Where(x => x.Value.rank != 3)
                .Select(x => new
                {
                    Key = x.Key,
                    Info = GameManager.Instance.MainCharacterList.FirstOrDefault(m => m.name == x.Key)
                }).ToList();

            if (!availableCharacters.Any())
            {
                return false;
            }

            float totalWeight = availableCharacters.Sum(x => x.Info.DrawProbability);
            float randomValue = Random.Range(0, totalWeight);
            float cumulativeWeight = 0;
            MainCharacterInfo selectedCharacter = null;
            foreach (var character in availableCharacters)
            {
                cumulativeWeight += character.Info.DrawProbability;
                if (cumulativeWeight >= randomValue)
                {
                    selectedCharacter = character.Info;
                    break;
                }
            }

            var characterData = selectedCharacter.CharacterEvolutions[1].GetData;
            mRewardEvent.name = characterData.GetCharacterName();
            mRewardEvent.sprite = characterData.Icon;
            
            var mainCharacter = BackEndManager.Instance.UserMainCharacterData[selectedCharacter.name];
            
            mRewardEvent.rewardEvent = () =>
            {
                if (mainCharacter.isGetType == EGetType.Lock)
                {
                    mainCharacter.isGetType = EGetType.Unlock;
                }
                mainCharacter.amount++;
                BackEndManager.Instance.SaveCharacterData();
            };
            return true;
        }
    }
}