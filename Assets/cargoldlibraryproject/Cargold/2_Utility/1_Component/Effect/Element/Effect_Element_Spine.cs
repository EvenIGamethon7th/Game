using UnityEngine;
using UnityEditor;
using System;
using Sirenix.OdinInspector;
using Cargold;
#if Spine_C
using Spine.Unity;

namespace Cargold.Effect
{
    public class Effect_Element_Spine : Effect_Element_Script
    {
        [BoxGroup("캐싱 데이터")] [SerializeField] private MeshRenderer meshRenderer = null;

        [SerializeField] private SkeletonAnimation skeletonAnimation = null;
        [SerializeField] [SpineAnimation] private string effectAniName = null;
        private Spine.AnimationState spineAnimationState;
        private Spine.Animation effectAni;

        public override void Init_Func(Action<Effect_Element_Script> _playDoneDel)
        {
            base.Init_Func(_playDoneDel);

            this.spineAnimationState = skeletonAnimation.state;

            effectAni = this.skeletonAnimation.skeleton.Data.FindAnimation(effectAniName);
            if (effectAni == null)
                Debug_C.Error_Func("?");

            spineAnimationState.Complete += CallAni_Func;

            if (this.meshRenderer == null)
            {
                CallEditor_Catching_Func();
            }
        }

        public override void Activate_Func()
        {
            base.Activate_Func();

            this.spineAnimationState.SetAnimation(0, effectAni, false);
        }
        protected override void SetSortingOrderElem_Func(int _layer)
        {
            this.meshRenderer.sortingOrder = _layer;
        }
        public override void Deactivate_Func()
        {
            this.spineAnimationState.ClearTrack(0);

            base.Deactivate_Func();
        }

        private void CallAni_Func(Spine.TrackEntry trackEntry)
        {
            if (trackEntry.Animation == this.effectAni)
            {
                base.isDone = true;

                base.playDoneDel(this);
            }
            else
            {
                Debug_C.Error_Func("?");
            }
        }

        [Button("캐싱 ㄱㄱ~")]
        public override void CallEditor_Catching_Func()
        {
            this.meshRenderer = skeletonAnimation.GetComponent<MeshRenderer>();

            base.CallEditor_Catching_Func();
        }
    } 
}
#endif