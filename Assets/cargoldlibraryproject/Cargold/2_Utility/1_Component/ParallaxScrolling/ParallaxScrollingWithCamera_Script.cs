using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold.ParallaxScrolling
{
    public abstract class ParallaxScrollingWithCamera_Script : ParallaxScrolling_Script
    {
        [ShowInInspector, ReadOnly] private CoroutineData followCameraCorData;
        [ShowInInspector, ReadOnly] private Vector2 initCameraPos;

        public override void Activate_Func()
        {
            base.Activate_Func();

            System.Func<Vector2> _getCameraPosDel = this.GetCameraPosDel_Func();

#if UNITY_EDITOR
            if (_getCameraPosDel == null)
            {
                return;
            }
#endif

            this.initCameraPos = _getCameraPosDel();

            this.followCameraCorData.StartCoroutine_Func(this.OnFollowCamera_Cor(_getCameraPosDel));
        }
        protected virtual System.Func<Vector2> GetCameraPosDel_Func()
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                return null;
            } 
#endif

            return CameraSystem_Manager.Instance.GetCameraPos_Func;
        }

        private IEnumerator OnFollowCamera_Cor(System.Func<Vector2> _getCameraPosDel)
        {
            Vector2 _beforeCameraPos = default;

            while (true)
            {
                yield return null;

                Vector2 _currentCameraPos = _getCameraPosDel();
                if (_beforeCameraPos == _currentCameraPos)
                    continue;

                _beforeCameraPos = _currentCameraPos;

                Vector2 _scrollPos = _currentCameraPos - this.initCameraPos;
                base.OnScrolling_Func(_scrollPos);
            }
        }

        public override void Deactivate_Func(bool _isInit = false)
        {
            this.followCameraCorData.StopCorountine_Func();

            base.Deactivate_Func(_isInit);
        }
    } 
}