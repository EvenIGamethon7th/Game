using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cargold
{
    public static class Coroutine_C
    {
        public static MonoBehaviour GetMonoBehaviour => CoroutineClass_C.Instance;

        private class CoroutineClass_C : Singleton.Singleton_Func_Mono_DontDestroy<CoroutineClass_C>
        {
            private bool isInitialize = false;
            private Dictionary<string, Coroutine> coroutineDic;

            public bool IsInitialize { get { return isInitialize; } }

            public override void Init_Func()
            {
                if (isInitialize == false)
                {
                    isInitialize = true;

                    coroutineDic = new Dictionary<string, Coroutine>();
                }
            }

            public Coroutine StartCoroutine_Func(IEnumerator _enumerator, string _key = "")
            {
                Coroutine _cor = this.StartCoroutine_Func(_enumerator);
                this.coroutineDic.Add_Func(_key, _cor);

                return _cor;
            }
            public Coroutine StartCoroutine_Func(IEnumerator _enumerator)
            {
                return this.StartCoroutine(_enumerator);
            }
            public void StopCoroutine_Func(string _key)
            {
                Coroutine _cor = null;
                if (this.coroutineDic.TryTake_Func(_key, out _cor) == true)
                    StopCoroutine(_cor);
            }
        }

        private static WaitForFixedUpdate waitForFixedUpdate;
        public static WaitForFixedUpdate WaitForFixedUpdate
        {
            get
            {
                if (waitForFixedUpdate == null) waitForFixedUpdate = new WaitForFixedUpdate();

                return waitForFixedUpdate;
            }
        }

        private static WaitForEndOfFrame waitForEndOfFrame;
        public static WaitForEndOfFrame WaitForEndOfFrame
        {
            get
            {
                if (waitForEndOfFrame == null) waitForEndOfFrame = new WaitForEndOfFrame();

                return waitForEndOfFrame;
            }
        }

        public static IEnumerator GetWaitForSeconds_Cor(float _time = 0.02f, bool _isUnscaledTime = false)
        {
            if (_isUnscaledTime == false)
            {
                float _loopBeginTime = Time.time;

                while (Time.time < _loopBeginTime + _time)
                {
                    yield return null;
                }
            }
            else
            {
                float _loopBeginTime = Time.unscaledTime;

                while (Time.unscaledTime < _loopBeginTime + _time)
                {
                    yield return null;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_progressTimeDel">시작 시점으로부터 경과한 초가 반환됩니다.</param>
        /// <param name="_time"></param>
        /// <returns></returns>
        public static IEnumerator GetWaitForSeconds_Cor(Action<float> _progressTimeDel, float _time = 0.02f)
        {
            float _loopBeginTime = Time.time;

            while (Time.time < _loopBeginTime + _time)
            {
                if (_progressTimeDel != null)
                    _progressTimeDel(Time.time - _loopBeginTime);

                yield return null;
            }
        }

        public static Coroutine StartCoroutine_Func(IEnumerator _enumerator, string _key = default)
        {
            if (CoroutineClass_C.Instance.IsInitialize == false)
                CoroutineClass_C.Instance.Init_Func();

            return CoroutineClass_C.Instance.StartCoroutine_Func(_enumerator, _key);
        }
        public static Coroutine StartCoroutine_Func(IEnumerator _enumerator)
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
#if UNITY_2018_1_OR_NEWER // https://docs.unity3d.com/Packages/com.unity.editorcoroutines@1.0/manual/index.html#:~:text=2018.1%20and%20later%20(recommended)
                Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutine(_enumerator, LibraryRemocon.Instance);
                return null;
#endif
            }
#endif

            return CoroutineClass_C.Instance.StartCoroutine_Func(_enumerator);
        }
        public static void StopCoroutine_Func(Coroutine _cor)
        {
            CoroutineClass_C.Instance.StopCoroutine(_cor);
        }
        public static void StopCoroutine_Func(string _key)
        {
            if (CoroutineClass_C.Instance.IsInitialize == false)
                CoroutineClass_C.Instance.Init_Func();

            CoroutineClass_C.Instance.StopCoroutine_Func(_key);
        }
        public static void StopAllCoroutine_Func()
        {
            CoroutineClass_C.Instance.StopAllCoroutines();
        }

        public static Coroutine Invoke_Func(Action _del, float _delay = 0f, bool _isUnscaledTime = false)
        {
            return Coroutine_C.StartCoroutine_Func(Coroutine_C.Invoke_Cor(_del, _delay, _isUnscaledTime));
        }
        private static IEnumerator Invoke_Cor(Action _del, float _delay = 0f, bool _isUnscaledTime = false)
        {
            if (_delay <= 0f)
                yield return null;
            else
                yield return Coroutine_C.GetWaitForSeconds_Cor(_delay, _isUnscaledTime);

            _del();
        }

        public static Coroutine InvokeRepeat_Func(Action<int> _del, float _duration = 0f, float _repeatInterval = 0f, float _delay = -1f, bool _isUnscaledTime = false, Action _doneDel = null)
        {
            int _repeatCnt = 0;
            if(0f < _duration)
                _repeatCnt = Mathf.RoundToInt(_duration / _repeatInterval);

            return Coroutine_C.InvokeRepeat_Func(_del, _repeatCnt, _repeatInterval, _delay, _isUnscaledTime, _doneDel);
        }
        public static Coroutine InvokeRepeat_Func(Action<int> _del, int _repeatCnt = 0, float _repeatInterval = 0f, float _delay = -1f, bool _isUnscaledTime = false, Action _doneDel = null)
        {
            return Coroutine_C.StartCoroutine_Func(Coroutine_C.InvokeRepeat_Cor(_del, _repeatCnt, _repeatInterval, _delay, _isUnscaledTime, _doneDel));
        }
        public static Coroutine InvokeRepeat_Func(Action _del, float _repeatInterval = 0f, float _delay = -1f, bool _isUnscaledTime = false, Action _doneDel = null)
        {
            return Coroutine_C.StartCoroutine_Func(Coroutine_C.InvokeRepeat_Cor((_cnt)=> _del(), 0, _repeatInterval, _delay, _isUnscaledTime, _doneDel));
        }
        private static IEnumerator InvokeRepeat_Cor(Action<int> _del, int _repeatCnt, float _repeatInterval = 0f, float _delay = -1f, bool _isUnscaledTime = false, Action _doneDel = null)
        {
            if (_delay == 0f)
                yield return null;
            else if (0f < _delay)
                yield return Coroutine_C.GetWaitForSeconds_Cor(_delay, _isUnscaledTime);

            if(0 < _repeatCnt)
            {
                for (int i = 0; i < _repeatCnt;)
                {
                    _del(i);

                    if (0 <= _repeatCnt)
                    {
                        i++;
                    }

                    if (0f < _repeatInterval)
                        yield return Coroutine_C.GetWaitForSeconds_Cor(_repeatInterval, _isUnscaledTime);
                    else
                        yield return null;
                }
            }
            else
            {
                for (int i = 0;; i++)
                {
                    _del(i);

                    yield return Coroutine_C.GetWaitForSeconds_Cor(_repeatInterval, _isUnscaledTime);
                }
            }

            if (_doneDel != null)
                _doneDel();
        }
    }
    public struct CoroutineData
    {
        [ShowInInspector, ReadOnly, LabelText("코루틴 캐싱")] public Coroutine catchedCor;
        [ShowInInspector, ReadOnly] private Action invokeDel;

        [ShowInInspector, ReadOnly] public bool IsActivate => this.catchedCor != null;

        public void StartCoroutine_Func(IEnumerator _enumer, CoroutineStartType _coroutineStartType = CoroutineStartType.StopAndStart)
        {
            bool _isStart = true;

            if(_coroutineStartType == CoroutineStartType.StopAndStart)
            {
                if (this.catchedCor != null)
                    Coroutine_C.StopCoroutine_Func(this.catchedCor);
            }
            else if(_coroutineStartType == CoroutineStartType.StartWhenStop)
            {
                if (this.catchedCor != null)
                    _isStart = false;
            }
            else
            {
                Debug_C.Error_Func("_coroutineStartType : " + _coroutineStartType);
            }

            if (_isStart == true)
                this.catchedCor = Coroutine_C.StartCoroutine_Func(_enumer);
        }
        public void StopCorountine_Func()
        {
            if (this.catchedCor != null)
                Coroutine_C.StopCoroutine_Func(this.catchedCor);

            this.catchedCor = null;
        }
        public void Invoke_Func(Action _del, float _delay, bool _isUnscaledTime = true)
        {
            if (this.catchedCor != null)
                Coroutine_C.StopCoroutine_Func(this.catchedCor);

            this.invokeDel = _del;

            this.catchedCor = Coroutine_C.Invoke_Func(this.Invoke_Func, _delay, _isUnscaledTime);
        }
        private void Invoke_Func()
        {
            this.invokeDel();

            this.catchedCor = null;
        }
    }

    public enum CoroutineStartType
    {
        None = 0,

        StopAndStart,
        StartWhenStop,
    }
}