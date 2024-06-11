using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
#if Spine_C
using Spine.Unity;

namespace Cargold.ExternalAsset.Spine_C
{
    [RequireComponent(typeof(SkeletonGraphic))]
    public class SpineControler_UI : SpineController<SkeletonGraphic>
    {
        public override SpineClassType GetSpineClassType => SpineClassType.SkeletonGraphic;

        public void Activate_Func(SkeletonDataAsset _skeletonDataAsset = null)
        {
            if(_skeletonDataAsset != null)
            {
                this.spineClass.skeletonDataAsset = _skeletonDataAsset;
                this.spineClass.Initialize(true);
            }

            base.Activate_Func();
        }

        public Vector2 GetBonePos_Func(string _boneName)
        {
            // Loc Pos에 넣는게 좋음

            return base.GetBonePos_Func(_boneName, Spine_C.BonePosType.Local);
        }

#if UNITY_EDITOR
        protected override void CallEdit_Catching_Func()
        {
            base.CallEdit_Catching_Func();

            if (base.skeletonDataAsset != null)
            {
                if (base.spineClass is SkeletonGraphic == false)
                    Debug_C.Error_Func("?");

                base.spineClass.skeletonDataAsset = base.skeletonDataAsset;
                base.spineClass.Initialize(true);

                base.spineClass.enabled = true;
            }
        }

        [FoldoutGroup("Editor"), Button("애니 재생")]
        protected void CallEdit_PlayAni_Func(string _aniStr, bool _isLoop = true)
        {
            this.Activate_Func(base.spineClass.SkeletonDataAsset);
            this.PlayAni_Func(_aniStr, _isLoop);
        }
#endif
    }
}
#endif