using _2_Scripts.Game.Unit;
using _2_Scripts.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI
{
    public class UI_UnitGroupCanvas : MonoBehaviour
    {
        public enum EButtonType
        {
            Academy,
            Sell,
            Fusion
        }

        private Button[] mButtons;
        public UnitGroup SelectUnitGroup { 
            set {
                mSelectUnitGroup = value;
                if (value == null) {
                    gameObject.SetActive(false);
                    return; 
                }
                gameObject.SetActive(true);
                SetFusionButton();
            } 
        }

        private UnitGroup mSelectUnitGroup;
        private CUnit mSelectUnit;

        private float mCostRate = 0.6f;

        private GameMessage<CUnit> mAcademyMessage;

        private void Start()
        {
            MessageBroker.Default.Receive<GameMessage<UnitGroup>>()
                .Where(message => message.Message == EGameMessage.SelectCharacter)
                .Subscribe(data =>
                {
                    SelectUnitGroup = data.Value;
                }).AddTo(this);

            MessageBroker.Default.Receive<GameMessage<bool>>()
                .Where(message => message.Message == EGameMessage.GoAcademy)
                .Subscribe(data =>
                {
                    if (data.Value) return;
                    mSelectUnitGroup.RemoveUnit(mSelectUnit);
                    mSelectUnit.Clear();
                    mSelectUnit = null;
                    if (mSelectUnitGroup != null)
                    {
                        SetFusionButton();
                    }
                }).AddTo(this);

            mButtons = GetComponentsInChildren<Button>();
            SubscribeAction(EButtonType.Academy, AcademyButton);
            SubscribeAction(EButtonType.Fusion, FusionButton);
            SubscribeAction(EButtonType.Sell, SellButton);

            gameObject.SetActive(false);
            mAcademyMessage = new GameMessage<CUnit>(EGameMessage.GoAcademy, mSelectUnit);
        }

        private void SubscribeAction(EButtonType button, Action action)
        {
            mButtons[(int)button].onClick.AddListener(() => action?.Invoke());
        }

        private void AcademyButton()
        {
            CUnit unit = mSelectUnitGroup.Units.OrderBy(x => x.CharacterDatas.isAlumni).LastOrDefault();
            mSelectUnit = unit;
            mAcademyMessage.SetValue(mSelectUnit);
            MessageBroker.Default.Publish(mAcademyMessage);
        }

        private void FusionButton()
        {
            mSelectUnitGroup.Fusion();
        }

        private void SellButton()
        {
            var unit = mSelectUnitGroup.Units.OrderBy(x => x.CharacterDatas.isAlumni)
                .FirstOrDefault();

            mSelectUnitGroup.RemoveUnit(unit);
            unit.Clear();
            if (mSelectUnitGroup != null)
                SetFusionButton();
            IngameDataManager.Instance.UpdateMoney(EMoneyType.Gold,(int)(unit.CharacterDatas.cost * mCostRate));
        }

        private void SetFusionButton()
        {
            if (mSelectUnitGroup.CanFusion())
                mButtons[(int)EButtonType.Fusion].interactable = true;

            else
                mButtons[(int)EButtonType.Fusion].interactable = false;
        }

        private void OnDestroy()
        {
            for (int i = 0; i <  mButtons.Length; ++i) 
            {
                mButtons[i].onClick.RemoveAllListeners();
            }
        }
    }
}
