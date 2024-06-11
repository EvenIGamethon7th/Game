using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using Cargold.FrameWork;

namespace Cargold.UI
{
    // 플레이하드 다닐 때 대표님께서 만들어 두신 라이브러리의 방식에서 착안
    public abstract class UI_Script : SerializedMonoBehaviour, GameSystem_Manager.IInitializer
    {
        private const string ActivateDoneStr = "ActivateDone";
        private const string DeactivateDoneStr = "DeactivateDone";

        [BoxGroup(CargoldLibrary_C.Mandatory), SerializeField, LabelText("그룹 객체")] private GameObject groupObj = null;
        [FoldoutGroup(CargoldLibrary_C.Optional), SerializeField, LabelText("애니메이션"), OnValueChanged("CallEdit_SetAni_Func")] protected Animation anim = null;
        [FoldoutGroup(CargoldLibrary_C.Optional), SerializeField, LabelText("활성화 애니 클립")] protected AniData_C activateAniData = new AniData_C(false);
        [FoldoutGroup(CargoldLibrary_C.Optional), SerializeField, LabelText("비활성화 애니 클립")] protected AniData_C deactivateAniData = new AniData_C(true);
        [FoldoutGroup(CargoldLibrary_C.Optional), SerializeField, LabelText("비활성화 연출 안정화 여부"), Tooltip("1. 활성화 중 비활성화 시도 시 무시" +
            "\n2. 비활성화 연출 중 재시도 시 무시")]
        private bool isSafeDeactivate = true;
        [FoldoutGroup(CargoldLibrary_C.Optional), SerializeField, LabelText("별도 활성화 호출 여부")]
        private bool isExtraActivateCall = false;
        [FoldoutGroup(CargoldLibrary_C.Optional), SerializeField, LabelText("별도 비활성화 호출 여부")]
        private bool isExtraDeactivateCall = false;
        [FoldoutGroup(CargoldLibrary_C.OptionalS + CargoldLibrary_C.Obsolete), SerializeField, LabelText("활성화 애니 클립")] private AnimationClip activateClip = null;
        [FoldoutGroup(CargoldLibrary_C.OptionalS + CargoldLibrary_C.Obsolete), SerializeField, LabelText("비활성화 애니 클립")] private AnimationClip deactivateClip = null;
        [FoldoutGroup(CargoldLibrary_C.OptionalS + CargoldLibrary_C.Obsolete), SerializeField, LabelText("애니 역재생 여부")] private bool isRewind = true;
        [ReadOnly, ShowInInspector, LabelText("활성화 상태")] private bool isActive = false;

        public bool IsActivate => this.isActive;
        public GameObject GetGroupObj => this.groupObj;
        protected virtual float GetActivateAniSpeed => 1f;
        protected virtual float GetDeactivateAniSpeed => 1f;
        public bool IsPlayAnim => this.anim.isPlaying;

        public virtual void Init_Func(int _layer)
        {
            if(_layer == 0)
            {
#if UNITY_EDITOR
                if (this.activateClip != null || this.deactivateClip != null)
                {
                    Debug_C.Warning_Func(this.transform.GetPath_Func() + "의 카라리 UI 활성화 및 비활성화 애니 클립은 2025년 1월 1일에 폐기 예정입니다. 대체된 기능으로 변경 바랍니다.");
                }
#endif

                this.activateAniData.SetAniSpeedModify_Func(() => this.GetActivateAniSpeed);
                this.deactivateAniData.SetAniSpeedModify_Func(() => this.GetDeactivateAniSpeed);

                this.DeactivateDone_Func(true);
            }
        }

        public virtual void Activate_Func()
        {
            if(this.IsAlwaysShow_Func() == false)
                this.groupObj.SetActive(true);

            bool _isDone = false;
            this.isActive = false;

            if (this.anim != null)
            {
                if(this.activateAniData.IsHave == true)
                {
                    if (this.anim.isPlaying == true)
                        this.anim.Stop();

                    this.anim.Play_Func(this.activateAniData);
                }
                else
                {
                    _isDone = true;
                }
            }
            else
            {
                _isDone = true;
            }
            
            if(_isDone == true && this.isExtraActivateCall == false)
                this.ActivateDone_Func();
        }
        protected virtual void ActivateDone_Func()
        {
#if UNITY_EDITOR
            if (this.groupObj.activeSelf == false)
                Debug_C.Warning_Func("Animation Play Automatically를 꺼주세요. 그로 인해 다음의 객체가 Activate 함수를 통하지 않고 ActivateDone을 호출하고 있습니다. "
                    + this.transform.GetPath_Func()); 
#endif

            this.isActive = true;
        }

        public virtual void Deactivate_Func()
        {
            if (this.isSafeDeactivate == true)
            {
                if(this.isActive == false)
                    return;
            }
            
            bool _isDone = false;

            if (this.anim != null)
            {
                if (this.deactivateAniData.IsHave == true)
                {
                    bool _isPlay = true;

                    if (this.anim.IsPlaying(this.deactivateAniData.GetClip.name) == false)
                    {
                        if (this.anim.isPlaying == true)
                            this.anim.Stop();
                    }
                    else
                    {
                        if(this.deactivateAniData.IsRewind == true)
                        {
                            AnimationState _aniState = this.anim[this.deactivateAniData.GetClip.name];
                            if(_aniState.speed < 0f)
                            {
                                _isPlay = false;
                            }
                            else
                            {
                                this.anim.Stop();
                            }
                        }
                    }

                    if(_isPlay == true)
                        this.anim.Play_Func(this.deactivateAniData);
                }
                else
                {
                    _isDone = true;
                }
            }
            else
            {
                _isDone = true;
            }

            if (_isDone == true && this.isExtraDeactivateCall == false)
                this.DeactivateDone_Func();
        }
        public virtual void DeactivateDone_Func(bool _isInit = false)
        {
            if (_isInit == false)
            {

            }

            this.isActive = false;

            if (this.IsAlwaysShow_Func() == false)
                this.groupObj.SetActive(false);
        }

        /// <summary>
        /// UI를 끄지 않고 언제나 활성화시킬 건가요?
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsAlwaysShow_Func() => false;

        private void CallAni_Activate_Func()
        {
            this.CallAni_Func(ActivateDoneStr);
        }
        private void CallAni_Deactivate_Func()
        {
            this.CallAni_Func(DeactivateDoneStr);
        }
        public virtual bool CallAni_Func(string _key)
        {
            switch (_key)
            {
                case ActivateDoneStr:
                    if(this.isActive == false) this.ActivateDone_Func();
                    return true;

                case DeactivateDoneStr:
                    if(this.isActive == true) this.DeactivateDone_Func();
                    return true;

                default:
                    return false;
            }
        }
        public void CallAni_Override_Func(string _key)
        {
            this.CallAni_Func(_key);
        }

        public static RectTransform SetChildRtrf_Func(Transform _parentTrf, string _NameStr = "Group", bool _isSiblingFirst = true)
        {
            RectTransform _groupRtrf = null;

            Transform _groupTrf = _parentTrf.Find(_NameStr);
            if (_groupTrf == null)
            {
                _groupRtrf = new GameObject(_NameStr).AddComponent<RectTransform>();

                _groupRtrf.SetParent(_parentTrf);
            }
            else
            {
                if (_groupTrf.TryGetComponent(out _groupRtrf) == false)
                {
                    _groupRtrf = _groupTrf.gameObject.AddComponent<RectTransform>();
                }
            }

            _groupRtrf.SetStretch_Func();

            if (_isSiblingFirst == true)
                _groupRtrf.SetAsFirstSibling();

            return _groupRtrf;
        }

#if UNITY_EDITOR
        [Button(CargoldLibrary_C.CatchingStr)]
        private void Reset()
        {
            this.groupObj = SetChildRtrf_Func(this.transform).gameObject;

            if (this.TryGetComponent(out Animation _anim) == true)
            {
                this.anim = _anim;

                this.CallEdit_SetAni_Func();
            }
        }

        private void CallEdit_SetAni_Func()
        {
            if (this.anim != null)
            {
                string _objName = this.gameObject.name;

                if (this.anim.playAutomatically == true)
                {
                    this.anim.playAutomatically = false;

                    Debug_C.Log_Func("애니메이션 자동 재생 비활성화 : " + _objName);
                }

                foreach (AnimationState item in this.anim)
                {
                    AnimationClip _clip = item.clip;
                    if(_clip == null)
                    {
                        Debug_C.Error_Func("다음 경로의 UI 애니메이션에 클립이 없습니다. : " + _objName);
                        break;
                    }

                    this.anim.clip = _clip;

                    if(this.activateAniData.GetClip == null)
                    {
                        Debug_C.Log_Func("활성화 애니 자동 삽입 : " + _objName);
                        this.activateAniData = new AniData_C(false, _clip, 1f);
                    }

                    if(this.deactivateAniData.GetClip == null)
                    {
                        Debug_C.Log_Func("비활성화 애니 자동 삽입 : " + _objName);
                        this.deactivateAniData = new AniData_C(true, _clip, 1f);
                    }

                    break;
                }
            }
        }
#endif
    }
}