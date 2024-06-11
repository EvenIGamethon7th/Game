using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
#if Spine_C
using Spine.Unity;

namespace Cargold.ExternalAsset.Spine_C
{
    [RequireComponent(typeof(SkeletonAnimation))]
    public class SpineControler_Ingame : SpineController<SkeletonAnimation>
    {
        [ReadOnly, ShowInInspector, LabelText("메쉬렌더러")] protected MeshRenderer meshRenderer = null;

        public override SpineClassType GetSpineClassType => SpineClassType.SkeletonAnimation;

        public void Activate_Func(SkeletonDataAsset _skeletonDataAsset = null)
        {
            if(_skeletonDataAsset != null)
            {
                this.spineClass.skeletonDataAsset = _skeletonDataAsset;
                this.spineClass.Initialize(true);
            }

            if (this.TryGetComponent(out this.meshRenderer) == false)
                Debug_C.Error_Func("다음 경로의 스파인 객체에 MeshRenderer를 찾을 수 없습니다. : " + this.transform.GetPath_Func());

            base.Activate_Func();
        }

        public Vector2 GetBonePos_Func(string _boneName)
        {
            return base.GetBonePos_Func(_boneName, Spine_C.BonePosType.World);
        }

        public void SetLayer_Func(int _sortingOrder)
        {
            this.meshRenderer.sortingOrder = _sortingOrder;
        }

#if UNITY_EDITOR
        protected override void CallEdit_Catching_Func()
        {
            base.CallEdit_Catching_Func();

            if (base.skeletonDataAsset != null)
            {
                if (base.spineClass is SkeletonAnimation == false)
                    Debug_C.Error_Func("?");

                base.spineClass.skeletonDataAsset = base.skeletonDataAsset;
                base.spineClass.Initialize(true, false);

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