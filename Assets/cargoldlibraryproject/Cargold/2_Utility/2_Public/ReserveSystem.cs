using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cargold.ReserveSystem
{
    [System.Serializable]
    public class ReserveSystem
    {
        private float timer;
        private bool isReserve;
        private float nextChangeTime;
        private Action callback;
        private Coroutine cor;
        private MonoBehaviour coroutineCallerObj;

        public ReserveSystem(float _nextChangeTime, Action _callback, MonoBehaviour _coroutineCallerObj = null)
        {
            this.nextChangeTime = _nextChangeTime;
            this.callback = _callback;
            this.coroutineCallerObj = _coroutineCallerObj is null == false ? _coroutineCallerObj : Coroutine_C.GetMonoBehaviour;
        }

        public void Activate_Func()
        {
            this.timer = Time.unscaledTime;
            this.isReserve = false;

            this.cor = coroutineCallerObj.StartCoroutine(this.Reserve_Cor());
        }
        public void OnReserve_Func(bool _isImmediate = false)
        {
            if (_isImmediate == false)
                this.isReserve = true;
            else
                this.callback();
        }
        public IEnumerator Reserve_Cor()
        {
            while (true)
            {
                if (this.isReserve == true)
                {
                    if (this.timer <= Time.unscaledTime)
                    {
                        this.OnImmediately_Func();
                    }
                }

                yield return null;
            }
        }

        private void OnImmediately_Func()
        {
            this.timer = Time.unscaledTime + this.nextChangeTime;
            this.isReserve = false;

            this.callback();
        }

        public void Deactivate_Func()
        {
            this.coroutineCallerObj.StopCoroutine(this.cor);
            this.cor = null;
        }
    }
}