using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using Cargold.Example;
using UnityEngine.UI;
using Cargold.FrameWork;
using System;

namespace Cargold.UI.Focus
{
    public abstract class UI_Focus_Manager : MonoBehaviour, Cargold.Example.IPropertyAdapter, GameSystem_Manager.IInitializer
    {
        public static UI_Focus_Manager Instance;

        [SerializeField, HideIf("@propertyAdapterClass != null")] private PropertyAdapter_UI_Focus_Script propertyAdapterClass;

        [ShowInInspector] private GameObject groupObj { get => propertyAdapterClass.groupObj; set => propertyAdapterClass.groupObj = value; }
        [ShowInInspector] private RectTransform focusRtrf { get => propertyAdapterClass.focusRtrf; set => propertyAdapterClass.focusRtrf = value; }
        [ShowInInspector] private Animation focusAnim { get => propertyAdapterClass.focusAnim; set => propertyAdapterClass.focusAnim = value; }
        [ShowInInspector] private Button btn { get => this.propertyAdapterClass.btn; set => this.propertyAdapterClass.btn = value; }
        private Cargold.Observer.Observer_Action selectedObs;
        private CoroutineData followCorData;
        private CoroutineData durationCorData;
        private ButtonHierachy btnHierachy;

        public bool IsActivate => this.groupObj.activeSelf;

        public virtual void Init_Func(int _layer)
        {
            if(_layer == 0)
            {
                Instance = this;

                this.selectedObs = new Observer.Observer_Action();

                if (this.btn.onClick.GetPersistentEventCount() <= 0)
                    this.btn.onClick.AddListener(this.CallBtn_Func);

                this.btnHierachy = ButtonHierachy.None;

                this.Deactivate_Func(true);
            }
        }

        [Button("테스트 활성화(인게임)")]
        public void ActivateByWorld_Func(Transform _targetTrf
            , Vector2 _extraSize = default, float _durationTime = 0f, bool _isFollow = false, Action _callbackDel = null, ButtonHierachy _btnHierachy = ButtonHierachy.Whole)
        {
            this.ActivateByWorld_Func(_targetTrf.position, _extraSize, _durationTime, _isFollow, _callbackDel, _btnHierachy);

            if(_isFollow == true)
                this.followCorData.StartCoroutine_Func(this.OnFollowWorldTrf_Cor(_targetTrf));
        }
        [Button("테스트 활성화(인게임)")]
        public void ActivateByWorld_Func(Vector2 _worldPos, Vector2 _extraSize = default, float _durationTime = 0f, bool _isFollow = false
            , Action _callbackDel = null, ButtonHierachy _btnHierachy = ButtonHierachy.Whole)
        {
            Vector2 _screenPos = this.GetScreenPos_Func(_worldPos);
            this.ActivateByUI_Func(_screenPos, _extraSize, _durationTime, _callbackDel, _btnHierachy);

            if (_isFollow == true)
                this.followCorData.StartCoroutine_Func(this.OnFollowWorldPos_Cor(_worldPos));
        }

        [Button("테스트 활성화(UI)")]
        public void ActivateByUI_Func(RectTransform _targetRtrf, Vector2 _extraSize = default, float _durationTime = 0f, bool _isFollow = false
            , Action _callbackDel = null, ButtonHierachy _btnHierachy = ButtonHierachy.Whole)
        {
            this.focusRtrf.SetParent(_targetRtrf);

            this.Activate_Func(_extraSize, true, _durationTime, _callbackDel);

            this.focusRtrf.SetParent(this.groupObj.transform);

            this.OnBtn_Func(_btnHierachy);

            if (_isFollow == true)
                this.followCorData.StartCoroutine_Func(this.OnFollowUiRTrf_Cor(_targetRtrf));
        }
        [Button("테스트 활성화(UI)")]
        public void ActivateByUI_Func(Vector2 _screenPos
            , Vector2 _extraSize = default, float _durationTime = 0f, Action _callbackDel = null, ButtonHierachy _btnHierachy = ButtonHierachy.Whole)
        {
            this.Activate_Func(_extraSize, false, _durationTime, _callbackDel);

            this.OnBtn_Func(_btnHierachy);

            this.focusRtrf.anchoredPosition = _screenPos;
        }
        protected virtual void Activate_Func(Vector2 _extraSize, bool _isUI, float _durationTime
            , Action _callbackDel = null)
        {
            this.groupObj.SetActive(true);

            if(_isUI == true)
            {
                this.focusRtrf.anchorMin = Vector2.zero;
                this.focusRtrf.anchorMax = Vector2.one;

                this.focusRtrf.anchoredPosition = Vector2.zero;
            }
            else
            {
                this.focusRtrf.anchorMin = Vector2.zero;
                this.focusRtrf.anchorMax = Vector2.zero;
            }

            this.focusRtrf.sizeDelta = Vector2.zero + _extraSize;

            this.focusAnim.Play_Func(this.focusAnim.clip);

            if(0f < _durationTime)
            {
                this.durationCorData.StartCoroutine_Func(this.OnDuration_Cor(_durationTime));
            }

            if (_callbackDel != null)
                this.Subscribe_Selected_Func(_callbackDel);
        }
        private void OnBtn_Func(ButtonHierachy _btnHierachy)
        {
            if(this.btnHierachy != _btnHierachy)
            {
                this.btnHierachy = _btnHierachy;

                if (_btnHierachy == ButtonHierachy.Whole)
                {
                    this.btn.transform.SetParent_Func(this.groupObj.transform);
                }
                else if (_btnHierachy == ButtonHierachy.Center)
                {
                    this.btn.transform.SetParent_Func(this.focusRtrf);
                }
                else
                {
                    Debug_C.Error_Func("_btnHierachy : " + _btnHierachy);
                }
            }

            this.btn.transform.SetAsLastSibling();
            this.btn.GetRtrf_Func().SetStretch_Func();
        }

        protected IEnumerator OnFollowWorldTrf_Cor(Transform _targetTrf)
        {
            while (_targetTrf != null)
            {
                this.focusRtrf.anchoredPosition = this.GetScreenPos_Func(_targetTrf.position);

                yield return null;
            }
        }
        protected IEnumerator OnFollowWorldPos_Cor(Vector2 _worldPos)
        {
            while (true)
            {
                this.focusRtrf.anchoredPosition = this.GetScreenPos_Func(_worldPos);

                yield return null;
            }
        }
        protected IEnumerator OnFollowUiRTrf_Cor(RectTransform _targetRtrf)
        {
            while (_targetRtrf != null)
            {
                this.focusRtrf.position = _targetRtrf.position;

                yield return null;
            }
        }

        private IEnumerator OnDuration_Cor(float _durationTime)
        {
            yield return Coroutine_C.GetWaitForSeconds_Cor(_durationTime);

            this.Deactivate_Func();
        }

        public void OnFollowDone_Func()
        {
            this.followCorData.StopCorountine_Func();
        }
        protected virtual Vector2 GetScreenPos_Func(Vector2 _worldPos)
        {
            return Cargold.CameraSystem_Manager.Instance.GetScreenPos_Func(_worldPos);
        }

        public virtual void Deactivate_Func(bool _isInit = false)
        {
            this.selectedObs.UnsubscribeAll_Func();

            this.durationCorData.StopCorountine_Func();
            this.followCorData.StopCorountine_Func();

            this.groupObj.SetActive(false);
        }

        public void Subscribe_Selected_Func(System.Action _del)
        {
            this.selectedObs.Subscribe_Func(_del);
        }
        public void Unsubscribe_Selected_Func(System.Action _del)
        {
            this.selectedObs.Unsubscribe_Func(_del);
        }

        public virtual void CallBtn_Func()
        {
            if (this.focusAnim.isPlaying == true)
                return;

            this.selectedObs.Notify_Func();

            this.Deactivate_Func();
        }

        [Button]
        public void Test_Func(Transform _trf)
        {
            Debug_C.Log_Func("01 : " + _trf.GetSiblingIndex());
            _trf.SetAsLastSibling();
            Debug_C.Log_Func("02 : " + _trf.GetSiblingIndex());
        }

#if UNITY_EDITOR
        void IPropertyAdapter.CallEdit_AddComponent_Func<T>(T _exampleData)
        {
            this.propertyAdapterClass = _exampleData as PropertyAdapter_UI_Focus_Script;
        }
#endif

        public enum ButtonHierachy
        {
            None = 0,

            Center = 10,
            Whole = 20,
        }
    }
}