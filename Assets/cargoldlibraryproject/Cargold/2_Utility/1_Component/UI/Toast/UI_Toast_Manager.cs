using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using UnityEngine.UI;

namespace Cargold.UI
{
    public abstract class UI_Toast_Manager : SerializedMonoBehaviour, Cargold.FrameWork.GameSystem_Manager.IInitializer
    {
        public static UI_Toast_Manager Instance;

        [ReadOnly, ShowInInspector] private List<ToastData> dataList;
        [SerializeField] private GameObject groupObj = null;
        [SerializeField] private Transform elemGroupTrf = null;
        [SerializeField] private GameObject touchBlockObj = null;

        public virtual void Init_Func(int _layer)
        {
            if(_layer == 0)
            {
                Instance = this;

                this.dataList = new List<ToastData>();

                this.groupObj.SetActive(true);
                this.elemGroupTrf.gameObject.SetActive(true);
                this.touchBlockObj.SetActive(false);

                this.Deactivate_Func(true);
            }
        }

        public void Activate_LibraryToast_Func(string _toastKey, Action _endCallbackDel = null, bool _isTouchBlock = false)
        {
            string _lczStr = this.GetLczStr_Func(_toastKey);
            this.Activate_WithContent_Func(_lczStr, _endCallbackDel, _isTouchBlock);
        }
        public void Activate_WithLcz_Func(string _lczKey, Action _endCallbackDel = null, bool _isTouchBlock = false)
        {
            UI_BaseToast_Script _elemClass = this.ActivateElem_Func(_isTouchBlock);

            _elemClass.Activate_WithLcz_Func(_lczKey, _endCallbackDel);
        }
        public void Activate_WithContent_Func(string _contentStr, Action _endCallbackDel = null, bool _isTouchBlock = false, bool isIgnoreTimeScale = false, float time = 1f)
        {
            UI_BaseToast_Script _elemClass = this.ActivateElem_Func(_isTouchBlock);

            _elemClass.Activate_WithContent_Func(_contentStr, _endCallbackDel, isIgnoreTimeScale: isIgnoreTimeScale, time);
        }
        private UI_BaseToast_Script ActivateElem_Func(bool _isTouchBlock)
        {
            UI_BaseToast_Script _elemClass = Cargold.FrameWork.PoolingSystem_Manager.Instance.Spawn_Func<UI_BaseToast_Script>(PoolingKey.ToastElem);
            _elemClass.transform.SetParent(this.elemGroupTrf);
            _elemClass.transform.localPosition = Vector3.zero;
            _elemClass.transform.localScale = Vector3.one;

            RectTransform _rtrf = _elemClass.transform as RectTransform;
            _rtrf.sizeDelta = new Vector2(Cargold.FrameWork.DataBase_Manager.Instance.GetUi_C.toast_sizeX,  _rtrf.sizeDelta.y);

            this.dataList.AddNewItem_Func(new ToastData(_elemClass, _isTouchBlock));

            if (_isTouchBlock == true)
                this.OnDim_Func(true);

            return _elemClass;
        }

        public void OnDim_Func(bool _isOn, bool _isForced = false)
        {
            if(_isOn == true)
            {
                this.touchBlockObj.SetActive(true);
            }
            else
            {
                bool _isOff = true;

                if(_isForced == false)
                {
                    foreach (ToastData _data in this.dataList)
                    {
                        if (_data.isTouchBlock == true)
                        {
                            _isOff = false;

                            break;
                        }
                    }
                }
                else
                {
                    _isOff = true;
                }

                if(_isOff == true)
                {
                    this.touchBlockObj.SetActive(false);
                }
            }
        }

        public void RemoveElem_Func(UI_BaseToast_Script _toastElemClass)
        {
            bool _isDimOff = false;
            for (int i = dataList.Count - 1; i >= 0; i--)
            {
                if(this.dataList[i].toastClass == _toastElemClass)
                {
                    if(this.dataList[i].isTouchBlock == true)
                        _isDimOff = true;

                    this.dataList.RemoveAt(i);

                    break;
                }
            }

            if(_isDimOff == true)
            {
                bool _isDimOffConfirm = true;
                foreach (ToastData _data in this.dataList)
                {
                    if(_data.isTouchBlock == true)
                    {
                        _isDimOffConfirm = false;
                        break;
                    }
                }

                if (_isDimOffConfirm == true)
                    this.OnDim_Func(false);
            }
        }
        public void RemoveElemAll_Func()
        {
            for (int i = this.dataList.Count - 1; i >= 0; i--)
            {
                UI_BaseToast_Script _toastElemClass = this.dataList[i].toastClass;
                _toastElemClass.Deactivate_Func(_isCallManager: true);
            }

            this.dataList.Clear();
        }

        public virtual void Deactivate_Func(bool _isInit = false)
        {
            if (_isInit == false)
            {
                this.RemoveElemAll_Func();
            }

            this.dataList.Clear();
        }

        protected virtual string GetLczStr_Func(string _toastKey)
        {
            switch (_toastKey)
            {
                case ToastKey.BtnContinuousAlarm:   return this.GetLczStr_BtnContinuousAlarm_Func();
                case ToastKey.AdsFail:              return this.GetLczStr_AdsFail_Func();
                case ToastKey.PurchaseFail:         return this.GetLczStr_PurchaseFail_Func();
                case ToastKey.AdsRemove:            return this.GetLczStr_AdsRemove_Func();
                case ToastKey.SameLangTypeSelected: return this.GetLczStr_SameLangTypeSelected_Func();

                default:
                    Debug_C.Error_Func("_toastKey : " + _toastKey);
                    return default;
            }
        }
        protected abstract string GetLczStr_BtnContinuousAlarm_Func();
        protected abstract string GetLczStr_AdsFail_Func();
        protected abstract string GetLczStr_PurchaseFail_Func();
        protected abstract string GetLczStr_AdsRemove_Func();
        protected abstract string GetLczStr_SameLangTypeSelected_Func();

        private void Reset()
        {
            this.gameObject.name = typeof(Cargold.UI.UI_Toast_Manager).Name;
            RectTransform _thisRtrf = this.transform as RectTransform;


            RectTransform _groupRtrf = UI_Script.SetChildRtrf_Func(this.transform);
            this.groupObj = _groupRtrf.gameObject;

            this.elemGroupTrf = UI_Script.SetChildRtrf_Func(_groupRtrf, "Elem_Group");

            Transform _touchBlockTrf = this.transform.Find("TouchBlockObj");
            if (_touchBlockTrf == null)
            {
                _touchBlockTrf = UI_Script.SetChildRtrf_Func(this.groupObj.transform, "TouchBlockObj");
            }

            if(_touchBlockTrf.TryGetComponent(out Text _txt) == false)
            {
                _txt = _touchBlockTrf.gameObject.AddComponent<Text>();
            }

            _txt.raycastTarget = true;
            _txt.text = null;

            this.touchBlockObj = _touchBlockTrf.gameObject;

            _groupRtrf.SetStretch_Func();
            _groupRtrf.gameObject.SetActive(false);
        }

        private struct ToastData
        {
            public UI_BaseToast_Script toastClass;
            public bool isTouchBlock;

            public ToastData(UI_BaseToast_Script _toastClass, bool _isTouchBlock)
            {
                this.toastClass = _toastClass;
                this.isTouchBlock = _isTouchBlock;
            }
        }

#if UNITY_EDITOR
        [Button("테스트 호출")]
        public void CallEdit_TestCall_Func(string _str)
        {
            this.Activate_WithContent_Func(_str);
        } 
#endif
    }

    public partial class ToastKey
    {
        public const string BtnContinuousAlarm = "BtnContinuousAlarm";
        public const string AdsFail = "AdsFail";
        public const string PurchaseFail = "PurchaseFail";
        public const string AdsRemove = "AdsRemove";
        public const string SameLangTypeSelected = "SameLangTypeSelected";
    }
}