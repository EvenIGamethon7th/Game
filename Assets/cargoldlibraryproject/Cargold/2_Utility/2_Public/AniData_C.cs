using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using System;

namespace Cargold
{
    [System.Serializable]
    public class AniData_C
    {
        public const string ActivateEventNameStr = "CallAni_Activate_Func";
        public const string DeactivateEventNameStr = "CallAni_Deactivate_Func";

        [HorizontalGroup("1")]
        [SerializeField, BoxGroup("1/클립"), OnValueChanged("CallEdit_SetClip_Func"), HideLabel] private AnimationClip clip = null;
        [SerializeField, BoxGroup("1/역재생 여부"), HideLabel] private bool isRewind = false;
        [SerializeField, BoxGroup("1/속도"), HideLabel] private float speed = 1f;
        [ShowInInspector, ReadOnly, LabelText("속도 일괄 조정 콜백")] private Func<float> aniSpeedModifyDel;

        public AniData_C()
        {
            this.clip = null;
            this.isRewind = false;
            this.speed = 1f;
            this.aniSpeedModifyDel = null;
        }
        public AniData_C(bool _isRewind, AnimationClip _clip = null, float _speed = 1f)
        {
            this.clip = _clip;
            this.isRewind = _isRewind;
            this.speed = _speed;
            this.aniSpeedModifyDel = null;
        }

        public AnimationClip GetClip => this.clip;
        public float GetSpeed => this.speed;
        public bool IsHave => this.clip != null;
        public bool IsRewind => this.isRewind;

        public void SetAniSpeedModify_Func(Func<float> _del)
        {
            this.aniSpeedModifyDel = _del;
        }

        public void Play_Func(Animation _anim, bool _isImmediatly = false)
        {
            if (this.clip != null)
            {
                float _speed = this.speed;
                if (this.aniSpeedModifyDel != null)
                    _speed = _speed * this.aniSpeedModifyDel();

                _anim.Play_Func(this.clip, this.isRewind, _isImmediatly, _speed);
            }
        }

#if UNITY_EDITOR
        void CallEdit_SetClip_Func()
        {
            if (this.clip == null)
                return;

            if (this.clip.legacy == false)
            {
                Debug_C.Warning_Func(this.clip.name + " 클립을 Legacy로 변경");
                this.clip.legacy = true;
            }

            //AnimationEvent[] _aniEventArr = this.clip.events;

            //foreach (var _aniEvent in _aniEventArr)
            //{
            /*

            [Button("1")]
    public void OnTest_Func()
    {
        var _evtArr = this.clip.events;

        
        foreach (AnimationEvent _evt in _evtArr)
        {
            Debug_C.Log_Func("Name : " + _evt.functionName + " / Time : " + _evt.time + " / Length : " + this.clip.length);
        }
    }

    public string name;
    public float time;

    [Button("2")]
    public void OnSave_Func()
    {
        var _arr = this.clip.events;

        AnimationEvent _evt = new AnimationEvent();
        _evt.functionName = this.name;
        _evt.time = this.time;
        _arr = _arr.GetAdd_Func(_evt);

        UnityEditor.AnimationUtility.SetAnimationEvents(this.clip, _arr);
    }
            */
            //}
        }
#endif
    }
}