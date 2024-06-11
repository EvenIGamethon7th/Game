using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using Cargold.FrameWork;
using System;
using System.Collections.Generic;
using Cargold.WhichOne;
#if DoTween_C
using DG.Tweening; 
#endif

namespace Cargold.UI
{
#if DoTween_C
    [RequireComponent(typeof(Button))]
    public abstract class UI_BaseButton_Script : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        public const string InStr = "누를 때";
        public const string OutStr = "뗄 때";

        [BoxGroup(CargoldLibrary_C.Mandatory), SerializeField, LabelText("연출 Target Transform")] private Transform[] punchTrfArr = null;
        [BoxGroup(CargoldLibrary_C.Mandatory), SerializeField, LabelText("버튼")] private Button btnClass = null;
        [BoxGroup(CargoldLibrary_C.Mandatory), SerializeField, LabelText("효과음")] private SfxType sfxType = SfxType.UI_Normal;
        [BoxGroup(CargoldLibrary_C.Mandatory), SerializeField, LabelText("애니메이션")] private Animation anim = null;
        [BoxGroup(CargoldLibrary_C.Mandatory), SerializeField, ShowIf("@CallEdit_IsAni_Func()"), LabelText("선택 시 자동 애니 여부")]
        private bool isAutoAnim = true;
        [BoxGroup(CargoldLibrary_C.Mandatory), SerializeField, ShowIf("@CallEdit_IsAni_Func()"), LabelText("활성화 애니 데이터")]
        private AniData_C activateAniData = new AniData_C(false);

        [FoldoutGroup(CargoldLibrary_C.Optional), SerializeField, ShowIf("@CallEdit_IsAni_Func() && isAutoAnim == false")]
        [LabelText("비활성화 애니 데이터")] private AniData_C deactivateAniData = new AniData_C(true);

        [FoldoutGroup(CargoldLibrary_C.Optional), FoldoutGroup(CargoldLibrary_C.OptionalS + InStr), ReadOnly, ShowInInspector]
        [LabelText("반복 선택 코루틴")] private CoroutineData continuousBtnCorData;
        [FoldoutGroup(CargoldLibrary_C.Optional), FoldoutGroup(CargoldLibrary_C.OptionalS + InStr), ReadOnly, ShowInInspector]
        [LabelText("선택 횟수")] private int stackBtnCnt;
        [FoldoutGroup(CargoldLibrary_C.Optional), FoldoutGroup(CargoldLibrary_C.OptionalS + InStr), ReadOnly, ShowInInspector]
        [LabelText("Twn")] private Tween[] inTwnArr;
        [FoldoutGroup(CargoldLibrary_C.Optional), FoldoutGroup(CargoldLibrary_C.OptionalS + InStr), SerializeField]
        [LabelText("연속 선택 기능 여부")] private bool isContinuousBtn = false;
        
        [FoldoutGroup(CargoldLibrary_C.Optional), FoldoutGroup(CargoldLibrary_C.OptionalS + OutStr), SerializeField, ShowIf("@!CallEdit_IsAni_Func()")]
        [LabelText("Twn 여부")] private bool isPunchable = true;
        [FoldoutGroup(CargoldLibrary_C.Optional), FoldoutGroup(CargoldLibrary_C.OptionalS + OutStr), SerializeField, ShowIf("@!CallEdit_IsAni_Func() && isPunchable")]
        [LabelText("Twn 효과만 받을 Trf")] private Transform[] punchTargetTrfArr = null;
        [FoldoutGroup(CargoldLibrary_C.Optional), FoldoutGroup(CargoldLibrary_C.OptionalS + OutStr), SerializeField, ShowIf("@!CallEdit_IsAni_Func() && isPunchable")]
        [LabelText("Twn Ease")] private Ease outEase = Ease.Unset;
        [FoldoutGroup(CargoldLibrary_C.Optional), FoldoutGroup(CargoldLibrary_C.OptionalS + OutStr), ReadOnly, ShowInInspector, ShowIf("@!CallEdit_IsAni_Func() && isPunchable")]
        [LabelText("Twn")] private Tween[] outTwnArr;

        private IEnumerator Start()
        {
            for (float i = 0f;;)
            {
                yield return null;

                i += Time.deltaTime;

                if (Cargold.FrameWork.DataBase_Manager.Instance is null == false)
                    break;

                if (10f < i)
                    Debug.LogError("?");
            }

            this.Reset_Func(true);

            if (this.btnClass is null == false)
            {
                Ease _outEase = this.outEase == Ease.Unset ? FrameWork.DataBase_Manager.Instance.GetUi_C.btnOutEase : this.outEase;

                int _length = this.punchTrfArr.Length;
                if (0 < _length)
                {
                    this.inTwnArr = new Tween[_length];
                    int _targetLength = 0;
                    if (this.anim == null && this.isPunchable == true)
                    {
                        _targetLength = this.punchTargetTrfArr.Length;
                        int _totalLength = _length + _targetLength;

                        this.outTwnArr = new Tween[_totalLength];
                    }

                    for (int i = 0; i < _length; i++)
                    {
                        Transform _punchTrf = this.punchTrfArr[i];

                        if (_punchTrf == null)
                            Debug_C.Error_Func("버튼 Transform Target Null : " + this.transform.GetPath_Func());

                        Vector3 _localScale = _punchTrf.localScale;
                        if (_localScale.x <= 0.01f && _localScale.y <= 0.01f)
                            _localScale = Vector3.one;

                        Tween _scaleDownTwn = _punchTrf.DOScale(_localScale - (_localScale * Cargold.FrameWork.DataBase_Manager.Instance.GetUi_C.btnScaleDown)
                            , Cargold.FrameWork.DataBase_Manager.Instance.GetUi_C.btnScaleDownTime).SetAutoKill(false).Pause();

#if UNITY_EDITOR && Test_Cargold
                        if (_scaleDownTwn == null)
                        {
                            string _pathStr = this.transform.GetPath_Func();
                            Debug_C.Error_Func("버튼, Tween Null, Path : " + _pathStr);
                        }
#endif

                        this.inTwnArr[i] = _scaleDownTwn;

                        if (this.anim == null && this.isPunchable == true)
                        {
                            Tween _punchTwn = _punchTrf.DOScale(_localScale, FrameWork.DataBase_Manager.Instance.GetUi_C.btnPunchTime)
                            .SetEase(_outEase).SetAutoKill(false).Pause();

#if UNITY_EDITOR && Test_Cargold
                            if (_punchTwn == null)
                            {
                                string _pathStr = this.transform.GetPath_Func();
                                Debug_C.Error_Func("버튼, Tween Null, Path : " + _pathStr);
                            }
#endif

                            this.outTwnArr[i] = _punchTwn;
                        }
                    }

                    if (this.anim == null && this.isPunchable == true)
                    {
                        for (int i = 0; i < _targetLength; i++)
                        {
                            Transform _punchTrf = this.punchTargetTrfArr[i];

                            Vector3 _localScale = _punchTrf.localScale;
                            if (_localScale.x <= 0.01f && _localScale.y <= 0.01f)
                                _localScale = Vector3.one;

                            Tween _twn = this.punchTargetTrfArr[i].DOScale(_localScale, Cargold.FrameWork.DataBase_Manager.Instance.GetUi_C.btnPunchTime)
                                .SetEase(_outEase).SetAutoKill(false).Pause();
#if UNITY_EDITOR && Test_Cargold
                            if (_twn == null)
                            {
                                string _pathStr = this.transform.GetPath_Func();
                                Debug_C.Error_Func("버튼, Tween Null, Path : " + _pathStr);
                            }
#endif

                            this.outTwnArr[i + _length] = _twn;
                        }
                    }
                }
            }
            else
            {
                string _pathStr = this.transform.GetPath_Func();
                Debug_C.Error_Func("BaseButton, Tween Null, Path : " + _pathStr);
            }
        }
        private void Reset()
        {
            this.Reset_Func(false);
        }
        private void Reset_Func(bool _isInit)
        {
            if (this.btnClass == null)
                this.btnClass = this.gameObject.GetComponent<Button>();
            this.btnClass.transition = Selectable.Transition.None;
            this.btnClass.interactable = true;

            Text _txt = null;
            if (this.gameObject.TryGetComponent<UnityEngine.UI.Graphic>(out Graphic _graphic) == false)
            {
                if (this.gameObject.TryGetComponent<UnityEngine.UI.Text>(out _txt) == false)
                    _txt = this.gameObject.AddComponent<UnityEngine.UI.Text>();

                _graphic = _txt;
            }
            else
            {
                _txt = _graphic as Text;
            }

            _graphic.raycastTarget = true;

            if (_txt is null == false)
                _txt.maskable = false;

            if (_isInit == false)
            {
                this.punchTrfArr = new Transform[1] { this.transform.parent };

                this.transform.GetRtrf_Func().SetStretch_Func();
            }
        }

        private void OnPointerDown_Func()
        {
            this.OnBtnTwn_Func();

            if (this.isContinuousBtn == true)
            {
                if (this.continuousBtnCorData.IsActivate == false)
                {
                    this.continuousBtnCorData.StartCoroutine_Func(ContinuousButton_Cor());

                    ++this.stackBtnCnt;

                    if (Cargold.FrameWork.DataBase_Manager.Instance.GetUi_C.continuousBtn_StackCount <= this.stackBtnCnt)
                    {
                        this.stackBtnCnt = 0;

                        this.OnNotify_EnableContinuousButton_Func();
                    }
                    else
                    {
                        Coroutine_C.Invoke_Func(() =>
                        {
                            if (0 < this.stackBtnCnt)
                                --this.stackBtnCnt;

                        }, Cargold.FrameWork.DataBase_Manager.Instance.GetUi_C.continuousBtn_StackDelay);
                    }
                }
            }
        }
        protected virtual void OnNotify_EnableContinuousButton_Func()
        {
            UI_Toast_Manager.Instance.Activate_LibraryToast_Func(ToastKey.BtnContinuousAlarm);
        }

        protected void OnPointerUp_Func()
        {
            if (this.inTwnArr != null)
            {
                foreach (Tween _scaleDownTwn in this.inTwnArr)
                    _scaleDownTwn.PlayBackwards();

                if (this.isContinuousBtn == true)
                    this.continuousBtnCorData.StopCorountine_Func();
            }
        }

        public void OnPointerClick_Func(bool _isInvokeBtn = false)
        {
            if(this.anim == null)
            {
                if (this.isPunchable == true && this.outTwnArr.IsHave_Func() == true)
                {
                    foreach (Tween _punchTwn in this.outTwnArr)
                        _punchTwn.Restart();
                }
            }
            else
            {
                if (this.isAutoAnim == true)
                    this.anim.Play_Func(this.activateAniData);
            }

            this.OnPlaySfx_Func();

            if (_isInvokeBtn == true)
                this.btnClass.onClick.Invoke();

            this.OnPointClick_Func();
        }
        protected virtual void OnPointClick_Func()
        {

        }

        private void OnBtnTwn_Func()
        {
            if (this.inTwnArr != null)
            {
                foreach (Tween _scaleDownTwn in this.inTwnArr)
                    _scaleDownTwn.Restart();
            }
        }
        protected void OnPlaySfx_Func()
        {
            SfxType _sfxType = this.GetSfxType_Func();
            if (0 <= _sfxType)
            {
#if UNITY_EDITOR
                if (Cargold.FrameWork.SoundSystem_Manager.Instance is null == true)
                    return;
#endif
                Cargold.FrameWork.SoundSystem_Manager.Instance.PlaySfx_Func(_sfxType);
            }
        }
        protected virtual SfxType GetSfxType_Func()
        {
            return this.sfxType;
        }
        public void OnBtnState_Func(bool _isActivate, bool _isImmediatly = false, float _speed = 0f)
        {
            AniData_C _aniData = _isActivate == true ? this.activateAniData : this.deactivateAniData;

            if(_speed <= 0f)
                this.anim.Play_Func(_aniData, _isImmediatly);
            else
                this.anim.Play_Func(_aniData.GetClip, _isImmediatly, _speed);
        }

        private IEnumerator ContinuousButton_Cor()
        {
            float _delay = Cargold.FrameWork.DataBase_Manager.Instance.GetUi_C.continuousBtn_BeginDelay;
            yield return Coroutine_C.GetWaitForSeconds_Cor((float)_delay);

            while (this.isContinuousBtn == true)
            {
                this.btnClass.onClick.Invoke();
                this.OnBtnTwn_Func();
                this.OnPointClick_Func();

                _delay -= Cargold.FrameWork.DataBase_Manager.Instance.GetUi_C.continuousBtn_DecreaseInterval;

                if (_delay < Cargold.FrameWork.DataBase_Manager.Instance.GetUi_C.continuousBtn_MaxInterval)
                    _delay = Cargold.FrameWork.DataBase_Manager.Instance.GetUi_C.continuousBtn_MaxInterval;

                this.OnPlaySfx_Func();

                yield return Coroutine_C.GetWaitForSeconds_Cor(_delay);
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            UI_BaseButton_Manager.OnPointerDown_Func(this);

            this.OnPointerDown_Func();
        }
        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            this.OnPointerUp_Func();
        }
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            this.OnPointerClick_Func();
        }

#if UNITY_EDITOR
        private bool CallEdit_IsAni_Func()
        {
            return this.anim != null;
        }
#endif
    }

    public static class UI_BaseButton_Manager
    {
        private static UI_BaseButton_Script currentBaseBtn;

        public static void OnPointerDown_Func(UI_BaseButton_Script _currentBaseBtn)
        {
            currentBaseBtn = _currentBaseBtn;
        }
        public static void OnPointerUp_Func(UI_BaseButton_Script _currentBaseBtn)
        {
            if (currentBaseBtn == _currentBaseBtn)
                currentBaseBtn = null;
            else
            {
                Debug_C.Error_Func("currentObj : " + currentBaseBtn.name + " / _obj : " + _currentBaseBtn.name);
            }
        }
        public static void OPointerCancle_Func()
        {
            if (currentBaseBtn is null == false)
            {
                UI_BaseButton_Script _currentBaseBtn = currentBaseBtn;

                OnPointerUp_Func(currentBaseBtn);

                PointerEventData _pointer = new PointerEventData(UnityEngine.EventSystems.EventSystem.current);
                ExecuteEvents.Execute(_currentBaseBtn.gameObject, _pointer, ExecuteEvents.pointerUpHandler);
            }
        }
    } 
#endif
    }