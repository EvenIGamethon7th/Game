using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using _2_Scripts.Game.ScriptableObject.Character;
using _2_Scripts.Game.Sound;
using _2_Scripts.Game.Unit;
using _2_Scripts.Utils;
using Cargold;
using Cargold.FrameWork.BackEnd;
using DG.Tweening;
using Sirenix.OdinInspector;
using Spine.Unity;
using TMPro;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using CharacterInfo = _2_Scripts.Game.ScriptableObject.Character.CharacterInfo;

namespace _2_Scripts.UI
{
    public class UI_SummonButton : SerializedMonoBehaviour
    {
        [Flags]
        public enum ESummonButtonState
        {
            Selected,
            Disable
        }

        [SerializeField]
        private LocalizeText mCharacterName;
        [SerializeField]
        private TextMeshProUGUI mCharacterCost;

        [SerializeField]
        private SkeletonGraphic mCharacterGraphic;

        [SerializeField]
        private Dictionary<ESummonButtonState, GameObject> mViewList = new();

        private ESummonButtonState mCurrentSummonButtonState = ESummonButtonState.Selected;

        private readonly Dictionary<int, string> mSummonProjectileDictionary = new()
        {
            {1,AddressableTable.Default_NormalProjectile},
            {2,AddressableTable.Default_RareProjectile},
            {3,AddressableTable.Default_UniqueProjectile},
        };

        [SerializeField]
        private readonly Dictionary<int, Sprite> mCardSpriteTable = new();


        private CharacterData mCharacterData;

        private bool mbIsLockRerollButton = false;

        private RectTransform uiRectTransform;

        [SerializeField]
        private Image mCardImage;

        private Button mSpawnButton;

        [SerializeField]
        private Image mColorImage;
        [SerializeField]
        private Image mIconImage;
        [SerializeField]
        private TextMeshProUGUI mClassText;
        [SerializeField]
        private readonly Dictionary<int, Sprite> mClassIcon = new();
        [SerializeField]
        private readonly Dictionary<int, UnityEngine.Color> mColor = new();

        public void SetInteract(bool canInteract)
        {
            mSpawnButton.interactable = canInteract;
            if (!canInteract)
            {
                mSpawnButton.onClick.RemoveAllListeners();
            }
        }

        public void AddClickEvent(Action action)
        {
            mSpawnButton.onClick.AddListener(action.Invoke);
            mSpawnButton.onClick.AddListener(OnSummonButton);
        }

        public void OnLockButton(bool isLock)
        {
            mbIsLockRerollButton = isLock;
        }

        private void Awake()
        {
            mSpawnButton = GetComponent<Button>();
        }

        private void Start()
        {
            uiRectTransform = GetComponent<RectTransform>();
            if (BackEndManager.Instance.IsUserTutorial || GameManager.Instance.IsTest)
            {
                UpdateCharacter();
                MessageBroker.Default.Receive<GameMessage<int>>().Where(message => message.Message == EGameMessage.StageChange && message.Value != 0)
                    .Subscribe(message =>
                    {
                        Reroll();
                    }).AddTo(this);
            }
        }

        public void OnSummonButton()
        {
            if (mCurrentSummonButtonState == ESummonButtonState.Disable)
            {
                return;
            }

            if (IngameDataManager.Instance.CurrentGold < mCharacterData.cost)
            {
                // 차후 Localize로 변경
                SoundManager.Instance.Play2DSound(AddressableTable.Sound_NotEnough_Money);
                UI_Toast_Manager.Instance.Activate_WithContent_Func("돈이 부족합니다");
                return;
            }

            //TODO 돈 뺴는거 넣어야 함UI의 현재 위치를 World 좌표
            var isCreateUnit = MapManager.Instance.CreateUnit(mCharacterData, spawnAction: (tilePos) =>
            {
                var uiWorldPos = global::Utils.GetUIWorldPosition(uiRectTransform);
                var projectile = ObjectPoolManager.Instance.CreatePoolingObject(mSummonProjectileDictionary[mCharacterData.rank], uiWorldPos);
                projectile.transform.DOMove(tilePos, 0.5f).OnComplete(() =>
                {
                    projectile.gameObject.SetActive(false);
                    var effect = ObjectPoolManager.Instance.CreatePoolingObject(Define.SpawnEffectDictionary[mCharacterData.rank], tilePos);
                });
            });
            if (isCreateUnit)
            {
                IngameDataManager.Instance.UpdateMoney(EMoneyType.Gold, -mCharacterData.cost);
                mCurrentSummonButtonState = ESummonButtonState.Disable;
                Tween_C.OnPunch_Func(this.transform);
                ShowChange();
            }
        }

        public void Reroll(CharacterInfo info = null, int grade = 1)
        {
            if (mbIsLockRerollButton)
                return;

            mCurrentSummonButtonState = ESummonButtonState.Selected;
            UpdateCharacter(info, grade);
            ShowChange();
        }

        private void ShowChange()
        {
            mViewList[mCurrentSummonButtonState].SetActive(true);
            mViewList[mCurrentSummonButtonState ^ ESummonButtonState.Disable].SetActive(false);
        }

        public void UpdateCharacter(CharacterInfo info = null, int grade = 1)
        {
            if (info == null)
            {
                info = IngameDataManager.Instance.RandomCharacterCardOrNull();
                mCharacterData = IngameDataManager.Instance.GetRandomCharacterData(info);
            }
            else
            {
                mCharacterData = IngameDataManager.Instance.GetCharacterData(info, grade);
            }

            global::Utils.CharacterSkeletonInit(mCharacterGraphic, mCharacterData.characterPack);
            mCharacterName.SetLocalizeKey(mCharacterData.nameKey);
            mCharacterCost.text = $"{mCharacterData.cost}$";
            UnityEngine.Color col = UnityEngine.Color.white;

            mIconImage.sprite = mClassIcon[mCharacterData.academyClass];
            mColorImage.color = mColor[mCharacterData.academyClass];
            mClassText.text = DataBase_Manager.Instance.GetLocalize.GetData_Func(mCharacterData.ClassType).ko;

            if (mCharacterData.rank == 3)
            {
                SoundManager.Instance.Vibrate();
            }
            CardChange(mCharacterData.rank);
        }

        private void CardChange(int rankNum)
        {
            mCardSpriteTable.TryGetValue(rankNum, out var image);
            mCardImage.sprite = image;
        }
    }
}