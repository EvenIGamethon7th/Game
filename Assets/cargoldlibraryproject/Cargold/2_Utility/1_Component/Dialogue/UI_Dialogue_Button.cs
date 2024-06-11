using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using TMPro;
using UnityEngine.UI;

namespace Cargold.Dialogue
{
    public abstract class UI_Dialogue_Button : MonoBehaviour, Example.IPropertyAdapter
    {
        [SerializeField, HideIf("@propertyAdapterClass != null")] private PropertyAdapter_UI_Dialogue_Button propertyAdapterClass;

        [ShowInInspector] private int id { get => propertyAdapterClass.id; set => propertyAdapterClass.id = value; }
        [ShowInInspector] private Image img { get => propertyAdapterClass.img; set => propertyAdapterClass.img = value; }
        [ShowInInspector] private TextMeshProUGUI tmp { get => propertyAdapterClass.tmp; set => propertyAdapterClass.tmp = value; }
        [ShowInInspector, ReadOnly] private UI_Dialogue_Script uiDialougeClass;
        [ShowInInspector, ReadOnly] private string callbackKey;

        public void Init_Func(UI_Dialogue_Script _uiDialougeClass)
        {
            this.uiDialougeClass = _uiDialougeClass;

            this.Deactivate_Func(true);
        }

        public void Activate_Func(Dialogue.DialogueButtonData _btnData)
        {
            this.img.SetFade_Func(1f);
            this.img.sprite = Cargold.FrameWork.DataBase_Manager.Instance.GetUi_C.GetBtnSprite_Func(_btnData.GetBtnType);

            if(_btnData.GetLczKey.IsNullOrWhiteSpace_Func() == false)
            {
                string _lczKey = _btnData.GetLczKey;
                string _lczStr = Cargold.FrameWork.LocalizeSystem_Manager.Instance.GetLcz_Func(_lczKey);
                this.tmp.text = _lczStr;
            }
            else
                this.tmp.text = "Next...";

            this.callbackKey = _btnData.GetCallbackKey;

            this.ActivateDone_Func();
        }
        public void Activate_Func()
        {
            this.img.SetFade_Func(1f);
            this.tmp.text = "Next...";

            this.ActivateDone_Func();
        }
        private void ActivateDone_Func()
        {
            this.gameObject.SetActive(true);
        }

        public void OnHide_Func()
        {
            this.gameObject.SetActive(false);
        }
        public void OnBtn_Func()
        {
            this.uiDialougeClass.OnBtn_Func(this.id, this.callbackKey);
        }
        public void Deactivate_Func(bool _isInit = false)
        {
            if (_isInit == false)
            {

            }

            this.callbackKey = null;
            this.gameObject.SetActive(false);
        }

        public void CallBtn_Func()
        {
            this.OnBtn_Func();
        }

#if UNITY_EDITOR
        bool CallEdit_IsVaild_Func()
        {
            return this.propertyAdapterClass is null == false;
        }

        void Example.IPropertyAdapter.CallEdit_AddComponent_Func<T>(T _exampleData)
        {
            this.propertyAdapterClass = _exampleData as PropertyAdapter_UI_Dialogue_Button;
        }
#endif
    }
}