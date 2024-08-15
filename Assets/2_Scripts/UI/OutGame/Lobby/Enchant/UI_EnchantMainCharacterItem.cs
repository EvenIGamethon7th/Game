using _2_Scripts.Game.BackEndData.MainCharacter;
using _2_Scripts.Game.ScriptableObject.Character;
using _2_Scripts.Game.Sound;
using _2_Scripts.Utils;
using Cargold.FrameWork.BackEnd;
using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.Enchant
{
    public class UI_EnchantMainCharacterItem : MonoBehaviour
    {
        [SerializeField] private Image mBorderImage;
        [SerializeField] private Sprite[] mBorderSpriteArr;
        [SerializeField] private Image mCharacterImage;
        [SerializeField] private TextMeshProUGUI mCharacterLevelText;
        [SerializeField] private Button mButton;
        [SerializeField] private TextMeshProUGUI mButtonText;
        [SerializeField] private GameObject mEnchantIcon;
        [SerializeField] private GameObject mBlindObject;
        [SerializeField] private Button mBorderButton;
        private MainCharacterData mMainCharacterData;
        private MainCharacterInfo mMainCharacterInfo;
        private GameMessage<Define.EnchantMainCharacterEvent> mOpenEnchantPopupMessage;
        private bool isInit = false;
        private void Start()
        {
            mButton.onClick.AddListener(OnClickButton);
            mBorderButton.onClick.AddListener(OnClickButton);
            mOpenEnchantPopupMessage = new GameMessage<Define.EnchantMainCharacterEvent>(EGameMessage.EnchantOpenPopUp,new Define.EnchantMainCharacterEvent
            {
                data = mMainCharacterData, infoData = mMainCharacterInfo
            });
            MessageBroker.Default.Receive<GameMessage<MainCharacterData>>().Where(message => message.Message == EGameMessage.MainCharacterLevelUp)
                .Subscribe(data =>
                {
                    UpdateSlot();
                }).AddTo(this);
        }

        private void OnClickButton()
        {
            MessageBroker.Default.Publish(mOpenEnchantPopupMessage);
        }

        public void InitItem(MainCharacterInfo data)
        {
            mMainCharacterInfo = data;
            BackEndManager.Instance.UserMainCharacterData.TryGetValue(data.name, out mMainCharacterData);
            isInit = true;
            UpdateSlot();
        }

        private void OnEnable()
        {
            UpdateSlot();
        }

        private void UpdateSlot()
        {
            if (isInit == false)
                return;
            
            mCharacterImage.sprite = mMainCharacterInfo.CharacterEvolutions[mMainCharacterData.rank].GetData.Icon;
            mCharacterLevelText.text = $"LV.{mMainCharacterData.rank}";
            mBorderImage.sprite = mBorderSpriteArr[mMainCharacterData.rank - 1];
            mBlindObject.SetActive(false);
            mEnchantIcon.SetActive(false);
            string buttonText = "강화완료";
            if (mMainCharacterData.rank < 3)
            {
                buttonText = $"{mMainCharacterData.amount}/{Define.MainCharacterEnchantAmountTable[mMainCharacterData.rank]}";
            }
            if (mMainCharacterData.isGetType == EGetType.Lock)
            {
                mBlindObject.SetActive(true);
                buttonText = "미획득";
            }

            if (mMainCharacterData.amount >= Define.MainCharacterEnchantAmountTable[mMainCharacterData.rank] && mMainCharacterData.rank < 3)
            {
                mEnchantIcon.SetActive(true);
            }
            mButtonText.text = buttonText;
        }
    }
}