using _2_Scripts.Game.Unit;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
                    transform.parent = null;
                    return; 
                }
                transform.parent = value.transform;
                transform.position = value.transform.position;
                gameObject.SetActive(true);
                SetFusionButton();
            } 
        }

        private UnitGroup mSelectUnitGroup;

        private float mCostRate = 0.8f;

        private void Awake()
        {
            mButtons = GetComponentsInChildren<Button>();
            GetComponent<Canvas>().worldCamera = Camera.main;
            SubscribeAction(EButtonType.Academy, AcademyButton);
            SubscribeAction(EButtonType.Fusion, FusionButton);
            SubscribeAction(EButtonType.Sell, SellButton);

            gameObject.SetActive(false);
        }

        private void SubscribeAction(EButtonType button, Action action)
        {
            mButtons[(int)button].onClick.AddListener(() => action?.Invoke());
        }

        private void AcademyButton()
        {
            CUnit unit = mSelectUnitGroup.Units.Where(x => x.CharacterDatas.isAlumni == false).FirstOrDefault();
            if (unit == null)
            {
                UI_Toast_Manager.Instance.Activate_WithContent_Func("이미 전부 졸업했습니다");
                return;
            }

            bool canEnterAcademy = MapManager.Instance.GoAcademy(mSelectUnitGroup, unit);
            if (!canEnterAcademy) return;

            mSelectUnitGroup.RemoveUnit(unit);
            unit.Clear();
        }

        private void FusionButton()
        {
            mSelectUnitGroup.Fusion();
        }

        private void SellButton()
        {
            var unit = mSelectUnitGroup.Units.OrderBy(x => !x.CharacterDatas.isAlumni)
                .FirstOrDefault();

            mSelectUnitGroup.RemoveUnit(unit);
            unit.Clear();
            GameManager.Instance.UpdateGold((int)(unit.CharacterDatas.cost * mCostRate));
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
