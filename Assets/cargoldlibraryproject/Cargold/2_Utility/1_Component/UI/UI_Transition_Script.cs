using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
using Coffee.UIEffects;
using System;
using UnityEngine.UI;
using static Cargold.Loading.UI_Loading_Transition_Script;

namespace Cargold
{
    public class UI_Transition_Script : MonoBehaviour, IDirectionComponent
    {
        [LabelText("그룹 Obj"), SerializeField] private GameObject groupObj = null;
        [LabelText("이미지"), SerializeField, OnValueChanged("CallEdit_ImgChanged_Func")] private Image targetImg = null;
        [LabelText("진입 시간"), SerializeField] private float enterTime = 1f;
        [LabelText("퇴장 시간"), SerializeField] private float exitTime = 1f;
        [LabelText("Rtrf"), ReadOnly, SerializeField] private RectTransform targetRtft = null;
        [LabelText("트랜지션"), ReadOnly, SerializeField] private UITransitionEffect targetTransitionClass = null;
        private CoroutineData activateCorData;
        private CoroutineData deactivateCorData;

        public float GetEnterTime => this.enterTime;
        public float GetExitTime => this.exitTime;

        public void Init_Func()
        {
            if (this.targetRtft == null)
                this.targetRtft = this.targetImg.rectTransform;

            if (this.targetTransitionClass is null == true)
                this.targetTransitionClass = this.targetImg.GetComponent<UITransitionEffect>();

            this.DeactivateDone_Func(true);
        }

        public void Activate_Func(Action _activateDoneDel)
        {
            this.groupObj.SetActive(true);

            this.activateCorData.StartCoroutine_Func(this.Activate_Cor(_activateDoneDel));
        }
        private IEnumerator Activate_Cor(Action _activateDoneDel)
        {
            this.targetRtft.rotation = Quaternion.identity;

            this.targetTransitionClass.effectFactor = 0f;

            while (this.targetTransitionClass.effectFactor < 1f)
            {
                this.targetTransitionClass.effectFactor += Time.deltaTime / this.enterTime;

                yield return null;
            }

            this.targetTransitionClass.effectFactor = 1f;

            this.activateCorData.catchedCor = null;

            if (_activateDoneDel != null)
                _activateDoneDel();
        }

        [ShowInInspector]
        public bool IsActivate_Func()
        {
            return this.activateCorData.IsActivate;
        }

        public void Deactivate_Func(Action _deactivateDoneDel = null)
        {
            this.deactivateCorData.StartCoroutine_Func(this.Deactivate_Cor(_deactivateDoneDel));
        }
        private IEnumerator Deactivate_Cor(Action _deactivateDoneDel = null)
        {
            this.targetRtft.rotation = Quaternion.Euler(Vector3.forward * 180f);

            this.targetTransitionClass.effectFactor = 1f;

            while (0f < this.targetTransitionClass.effectFactor)
            {
                this.targetTransitionClass.effectFactor -= Time.deltaTime / this.exitTime;

                yield return null;
            }

            this.targetTransitionClass.effectFactor = 0f;

            if (_deactivateDoneDel != null)
                _deactivateDoneDel();

            this.DeactivateDone_Func();
        }

        public void DeactivateDone_Func(bool _isInit = false)
        {
            if (_isInit == false)
            {

            }

            this.activateCorData.StopCorountine_Func();
            this.deactivateCorData.StopCorountine_Func();

            this.groupObj.SetActive(false);
        }

#if UNITY_EDITOR
        private void CallEdit_ImgChanged_Func()
        {
            this.Init_Func();
        }
#endif
    } 
}