using System;
using System.Collections.Generic;
using _2_Scripts.Game.BackEndData.MainCharacter;
using _2_Scripts.Game.ScriptableObject.Character;
using _2_Scripts.Utils;
using Cargold.FrameWork.BackEnd;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.Lobby.StartPopUp
{
    public class UI_MainCharacterSlot : SerializedMonoBehaviour
    {
        [SerializeField]
        private Image mBorderImage;
        [SerializeField]
        private Dictionary<EGetType,Sprite> mBorderSprite;
        [SerializeField]
        private Image mLockIcon;
        [SerializeField]
        private Image mCharacterImage;

        //Action 
        private Dictionary<EGetType, Action> mGetTypeAction;

        [SerializeField]
        private Button mButton;

        private MainCharacterData mMainCharacterData;
        
        private GameMessage<UI_MainCharacterSlot> mSelectCharacterMessage;
        private MainCharacterInfo mCharacterInfo;
        private void LockAction()
        {
            mLockIcon.gameObject.SetActive(true);
            mCharacterImage.color = Color.gray;
        }

        private void UnLockAction()
        {
            mLockIcon.gameObject.SetActive(false);
            mCharacterImage.color = Color.white;
            mBorderSprite.TryGetValue(EGetType.Unlock, out var sprite);
            mBorderImage.sprite = sprite;
        }

        private void SelectAction()
        {
            mLockIcon.gameObject.SetActive(false);
            mBorderSprite.TryGetValue(EGetType.Select, out var sprite);
            mBorderImage.sprite = sprite;
        }

        private void Start()
        {
            mSelectCharacterMessage = new GameMessage<UI_MainCharacterSlot>(EGameMessage.MainCharacterChange, this);
            MessageBroker.Default.Receive<GameMessage<UI_MainCharacterSlot>>()
                .Where(message => message.Message == EGameMessage.MainCharacterChange)
                .Subscribe(data =>
                {
                    OnUpdateBorder();
                }).AddTo(this);

            
        }

        public void Init(MainCharacterInfo characterInfo,Action<MainCharacterData> selectAction)
        {
            mGetTypeAction = new Dictionary<EGetType, Action>
            {
                {EGetType.Lock,LockAction},
                {EGetType.Unlock,UnLockAction},
                {EGetType.Select,SelectAction}
            };
            
            mCharacterInfo = characterInfo;
           BackEndManager.Instance.UserMainCharacterData.TryGetValue(characterInfo.name, out mMainCharacterData);
           mMainCharacterData ??= new MainCharacterData(characterInfo.name, 1, false, EGetType.Lock);
           mCharacterImage.sprite = mCharacterInfo.CharacterEvolutions[mMainCharacterData.rank].GetData.Icon;
            OnUpdateBorder();
            mButton.onClick.AddListener(()=>
            {
                selectAction.Invoke(mMainCharacterData);
                OnSelectButton();
            });

        }

        private void OnUpdateBorder()
        {
            EGetType borderType = EGetType.Lock;
            mButton.interactable = false;
            if(BackEndManager.Instance.UserMainCharacterData.TryGetValue(mCharacterInfo.name,out mMainCharacterData))
            {
                borderType = mMainCharacterData.isGetType;
                mButton.interactable = mMainCharacterData.isGetType != EGetType.Lock;
            }
            mGetTypeAction[borderType].Invoke();
        }

        private void OnSelectButton()
        {
            if (mMainCharacterData.isEquip)
                return;
            BackEndManager.Instance.UserMainCharacterData[mCharacterInfo.name].isEquip = true;
            BackEndManager.Instance.UserMainCharacterData[mCharacterInfo.name].isGetType = EGetType.Select;
            BackEndManager.Instance.UserMainCharacterData.Values
                .ForEach(data =>
                {
                    if (data.isEquip && data.key != mCharacterInfo.name)
                    {
                        data.isEquip = false;
                        data.isGetType = EGetType.Unlock;
                    }
                });
            MessageBroker.Default.Publish(mSelectCharacterMessage);
        }
    }
}