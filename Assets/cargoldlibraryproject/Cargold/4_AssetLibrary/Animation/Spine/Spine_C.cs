using System;
using UnityEngine;
using System.Collections.Generic;
using Cargold;
using Cargold.Observer;
using Sirenix.OdinInspector;
#if Spine_C
using Spine;
using Spine.Unity;

namespace Cargold.ExternalAsset.Spine_C
{
    #region SpineController
    [System.Serializable]
    public abstract class SpineController : MonoBehaviour
    {
        [ShowInInspector, ReadOnly] private Dictionary<string, Bone> strByBoneDic;

        public abstract Skeleton GetSkeleton { get; }
        public abstract SpineClassType GetSpineClassType { get; }
        public bool IsActivate => this.gameObject.activeSelf;

        public virtual void Init_Func()
        {
            this.strByBoneDic = new Dictionary<string, Bone>();

            this.Deactivate_Func(true);
        }

        public virtual void Activate_Func()
        {
            this.gameObject.SetActive(true);

            this.strByBoneDic.Clear();

            Skeleton _skeleton = this.GetSkeleton;
            if (_skeleton != null)
            {
                ExposedList<Bone> _boneList = _skeleton.Bones;
                foreach (Bone _bone in _boneList)
                    this.strByBoneDic.Add_Func(_bone.Data.Name, _bone);
            }
        }

        public abstract void PlayAni_Func(string _aniNameStr, bool _isLoop = false, float _aniSpeed = 1f);

        public abstract Spine.Animation GetSpineAni_Func(string _aniNameStr);
        public abstract Skin GetSpineSkin_Func(string _skinNameStr);
        public abstract void SetAniSpeed_Func(float _aniSpeed);
        public abstract void SetSkin_Func(string _skinNameStr);
        public bool TryGetBone_Func(string _boneNameStr, out Bone _bone)
        {
            return this.strByBoneDic.TryGetValue(_boneNameStr, out _bone);
        }

        public virtual void Deactivate_Func(bool _isInit = false)
        {
            if (_isInit == false)
            {

            }

            this.gameObject.SetActive(false);
        }

        public abstract void Subscribe_AniComplete_Func(Action<string> _del);
        public abstract void Unsubscribe_AniComplete_Func(Action<string> _del);
        public abstract void UnsubscribeAll_Func();

        public abstract void Subscribe_AniEvt_Func(Action<string> _del);
        public abstract void Unsubscribe_AniEvt_Func(Action<string> _del);

#if UNITY_EDITOR
        private void Reset()
        {
            this.CallEdit_Catching_Func();
        }

        [AssetSelector, OnValueChanged("CallEdit_Catching_Func"), ShowInInspector, FoldoutGroup("Editor"), LabelText("에셋")] protected SkeletonDataAsset skeletonDataAsset;

        [FoldoutGroup("Editor"), Button(CargoldLibrary_C.CatchingStr)]
        protected abstract void CallEdit_Catching_Func();

        [FoldoutGroup("Editor"), Button("강제 이니셜라이즈")]
        private void CallEdit_Init_Func()
        {
            this.Init_Func();
        }

        [FoldoutGroup("Editor"), Button("비활성화")]
        private void CallEdit_Deactivate_Func()
        {
            this.Deactivate_Func();
        }
#endif
    }
    #endregion
    #region SpineController<T>
    [System.Serializable]
    public abstract class SpineController<T> : SpineController where T : MonoBehaviour, ISkeletonComponent, IHasSkeletonDataAsset, IAnimationStateComponent
    {
        [SerializeField, LabelText("스파인 클래스")] protected T spineClass;
        [ReadOnly, ShowInInspector, LabelText("스켈레톤")] protected Skeleton skeleton = null;
        [ReadOnly, ShowInInspector, LabelText("애니스테이트")] protected Spine.AnimationState spineAnimationState;
        [ReadOnly, ShowInInspector, LabelText("애니메이션")] protected Dictionary<string, Spine.Animation> animationDic;
        [ReadOnly, ShowInInspector, LabelText("스킨")] protected Dictionary<string, Skin> skinDic;
        [ReadOnly, ShowInInspector] private Observer_Action<string> aniCompleteObs;
        [ReadOnly, ShowInInspector] private Observer_Action<string> aniEvtObs;

        public T GetSpineClass => this.spineClass;
        public Spine.AnimationState GetSpineAnimationState => this.spineAnimationState;
        public override Skeleton GetSkeleton => this.skeleton;
        public override SpineClassType GetSpineClassType => SpineClassType.Etc;

        public override void Init_Func()
        {
            this.animationDic = new Dictionary<string, Spine.Animation>();
            this.aniCompleteObs = new Observer_Action<string>();
            this.aniEvtObs = new Observer_Action<string>();
            this.skinDic = new Dictionary<string, Skin>();

            base.Init_Func();
        }

        public override void Activate_Func()
        {
            this.animationDic.Clear();

            Skeleton _skeleton = this.spineClass.Skeleton;
            this.skeleton = _skeleton;

            Spine.AnimationState _animationState = this.spineClass.AnimationState;
            this.spineAnimationState = _animationState;
            _animationState.Event += this.CallDel_Notify_Evt_Func;
            _animationState.Complete += this.CallDel_Notify_AniComplete_Func;

            base.Activate_Func();
        }

        public override void PlayAni_Func(string _aniName, bool _isLoop = false, float _aniSpeed = 1f)
        {
            Spine.Animation _spineAni = this.GetSpineAni_Func(_aniName);

            this.spineAnimationState.SetAnimation(0, _spineAni, _isLoop);

            this.SetAniSpeed_Func(_aniSpeed);
        }

        public override Spine.Animation GetSpineAni_Func(string _aniNameStr)
        {
            if (this.animationDic.TryGetValue(_aniNameStr, out Spine.Animation _spineAni) == false)
            {
                _spineAni = this.skeleton.Data.FindAnimation(_aniNameStr);
                if (_spineAni == null)
                {
                    Debug_C.Error_Func($"스파인에서 다음의 애니를 찾을 수 없습니다. : '{_aniNameStr}'");
                }

                this.animationDic.Add(_aniNameStr, _spineAni);
            }

            return _spineAni;
        }
        public override Skin GetSpineSkin_Func(string _skinNameStr)
        {
            if (this.skinDic.TryGetValue(_skinNameStr, out Skin _skin) == false)
            {
                _skin = this.skeleton.Data.FindSkin(_skinNameStr);
                if (_skin == null)
                {
                    Debug_C.Error_Func("?");
                }

                this.skinDic.Add(_skinNameStr, _skin);
            }

            return _skin;
        }
        public Transform GetSpineTrf_Func()
        {
            return this.spineClass.transform;
        }
        public Vector2 GetBonePos_Func(string _boneName, Spine_C.BonePosType _bonePosType, bool _isLog = true)
        {
            if (base.TryGetBone_Func(_boneName, out Bone _bone) == true)
            {
                switch (_bonePosType)
                {
                    case Spine_C.BonePosType.World: return _bone.GetWorldPosition(this.spineClass.transform);
                    case Spine_C.BonePosType.Local: return _bone.GetLocalPosition();
                    case Spine_C.BonePosType.Skeleton: return _bone.GetSkeletonSpacePosition();
                }
            }
            else
            {
                if(_isLog == true)
                    Debug_C.Warning_Func($"본 없음 : {_boneName}");
            }

            return default;
        }
        [ShowInInspector]
        public string GetCurrentAniStr_Func()
        {
            if (this.spineAnimationState != null)
            {
                TrackEntry _trackEntry = this.spineAnimationState.GetCurrent(0);
                if (_trackEntry != null)
                {
                    Spine.Animation _ani = _trackEntry.Animation;
                    if (_ani != null)
                    {
                        return _ani.Name;
                    }
                }
            }

            return null;
        }
        public override void SetAniSpeed_Func(float _aniSpeed)
        {
            this.spineAnimationState.TimeScale = _aniSpeed;
        }
        public override void SetSkin_Func(string _skinNameStr)
        {
            Skin _skin = this.GetSpineSkin_Func(_skinNameStr);

            this.skeleton.SetSkin(_skin);
        }

        public override void Deactivate_Func(bool _isInit = false)
        {
            if(_isInit == false)
            {
                
            }

            this.animationDic.Clear();
            this.aniCompleteObs.UnsubscribeAll_Func();
            this.aniEvtObs.UnsubscribeAll_Func();

            base.Deactivate_Func(_isInit);
        }

        public override void Subscribe_AniComplete_Func(Action<string> _del)
        {
            this.aniCompleteObs.Subscribe_Func(_del);
        }
        public override void Unsubscribe_AniComplete_Func(Action<string> _del)
        {
            this.aniCompleteObs.Unsubscribe_Func(_del);
        }
        public override void UnsubscribeAll_Func()
        {
            this.aniCompleteObs.UnsubscribeAll_Func();
            this.aniEvtObs.UnsubscribeAll_Func();
        }

        public override void Subscribe_AniEvt_Func(Action<string> _del)
        {
            this.aniEvtObs.Subscribe_Func(_del);
        }
        public override void Unsubscribe_AniEvt_Func(Action<string> _del)
        {
            this.aniEvtObs.Unsubscribe_Func(_del);
        }

        private void CallDel_Notify_Evt_Func(TrackEntry _trackEntry, Spine.Event _event)
        {
            if (this.aniEvtObs.HasSubscriber == true)
                this.aniEvtObs.Notify_Func(_event.Data.Name);
        }
        private void CallDel_Notify_AniComplete_Func(TrackEntry _trackEntry)
        {
            if(this.aniCompleteObs.HasSubscriber == true)
                this.aniCompleteObs.Notify_Func(_trackEntry.Animation.Name);
        }

#if UNITY_EDITOR
        protected override void CallEdit_Catching_Func()
        {
            if(this.spineClass == null)
                this.spineClass = this.gameObject.GetComponent<T>();
        }
#endif
    }
    #endregion

    public enum SpineClassType
    {
        None,
        SkeletonAnimation,
        SkeletonGraphic,
        Etc,
    }

    public static class Spine_C
    {
        public static void SetSkeletonSlotColor_Func(this Skeleton _skeleton, string _slotName, Color _color)
        {
#if UNITY_EDITOR
            try
            {
                _Set_Func();
            }
            catch (Exception _e)
            {

                Debug_C.Error_Func("Msg : " + _e.Message + " / _skeleton : " + _skeleton + " / _slotName : " + _slotName);
            }
#else
            _Set_Func();
#endif

            void _Set_Func()
            {
                Slot _slot = _skeleton.FindSlot(_slotName);
                SkeletonExtensions.SetColor(_slot, _color);
            }
        }
        public static void SetSkeletonSlotAttach_Func(this Skeleton _skeleton, string _slotName, string _attachmentName)
        {
            _skeleton.SetAttachment(_slotName, _attachmentName);
        }
        public static void SetAni_Func(this SkeletonGraphic _sg, Spine.Animation _playAni, Spine.Animation _nextAni)
        {
            Spine.AnimationState _aniState = _sg.AnimationState;
            _aniState.SetAnimation(0, _playAni, false);
            _aniState.AddAnimation(0, _nextAni, true, 0f);
        }

        public enum BonePosType
        {
            World,
            Local,
            Skeleton
        }
    }
}
#endif