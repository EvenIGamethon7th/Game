using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold.DataStructure;
using static Cargold.FrameWork.Sound_C;
using System;

namespace Cargold.FrameWork
{
    public abstract class SoundSystem_Manager : SerializedMonoBehaviour, GameSystem_Manager.IInitializer
    {
        public static SoundSystem_Manager Instance;

        [ReadOnly, ShowInInspector] protected Dictionary<BgmType, BgmData> bgmDataDic = null;
        [SerializeField] private SoundSystem_Bgm_Script[] bgmClassArr = null;
        private CirculateQueue<SoundSystem_Bgm_Script> bgmCQ;
        private SoundSystem_Bgm_Script currentBgmClass;

        [ReadOnly, ShowInInspector] protected Dictionary<SfxType, SfxData> sfxDataDic = null;
        [SerializeField] private SoundSystem_Sfx_Script[] sfxClassArr = null;
        private CirculateQueue<SoundSystem_Sfx_Script> sfxCQ;

        public BgmType CurrentBgmType => this.currentBgmClass.GetBgmType;

        public virtual void Init_Func(int _layer)
        {
            if (_layer == 0)
            {
                Instance = this;

                this.OnGenerateSoundComponent_Func(true);

                // Bgm
                foreach (SoundSystem_Bgm_Script _bgmClass in this.bgmClassArr)
                    _bgmClass.Init_Func();

                this.bgmCQ = new CirculateQueue<SoundSystem_Bgm_Script>();
                this.bgmCQ.Enqueue_Func(this.bgmClassArr);

                this.currentBgmClass = this.bgmCQ.Dequeue_Func();

                this.bgmDataDic = new Dictionary<BgmType, BgmData>();

                // Sfx
                this.sfxDataDic = new Dictionary<SfxType, SfxData>();
            }
            else if(_layer == 1)
            {
                if(DataBase_Manager.Instance != null)
                {
                    foreach (BgmData _bgmData in DataBase_Manager.Instance.GetSound_C.bgmDataArr)
                        this.bgmDataDic.Add(_bgmData.sourceKey, _bgmData);

                    foreach (SfxData _sfxData in DataBase_Manager.Instance.GetSound_C.sfxDataArr)
                        this.sfxDataDic.Add(_sfxData.sourceKey, _sfxData);
                }

                foreach (SoundSystem_Sfx_Script _sfxClass in this.sfxClassArr)
                    _sfxClass.Init_Func();

                this.sfxCQ = new CirculateQueue<SoundSystem_Sfx_Script>();
                this.sfxCQ.Enqueue_Func(this.sfxClassArr);
            }
            else if (_layer == 2)
            {
                bool _isBgm = true;
                bool _isSfx = true;
                if (UserSystem_Manager.Instance != null)
                {
                    _isBgm = UserSystem_Manager.Instance.GetCommon.GetBgm_Func();
                    _isSfx = UserSystem_Manager.Instance.GetCommon.GetSfx_Func();

                    UserSystem_Manager.Instance.GetCommon.Subscribe_Bgm_Func(this.CallDel_BgmChanged_Func);
                }
                
                this.SetBgm_Func(_isBgm);
                this.SetSfx_Func(_isSfx);
            }
        }
        private void OnGenerateSoundComponent_Func(bool _isCallRuntime)
        {
            Set_Func(ref this.bgmClassArr, 2);
            Set_Func(ref this.sfxClassArr, 10);

            void Set_Func<T>(ref T[] _thisArr, int _minNum) where T : SoundSystem_Script
            {
                if (_thisArr.IsHave_Func(_minNum) == false)
                {
                    T[] _classArr = this.GetComponentsInChildren<T>();
                    if (_classArr.IsHave_Func(_minNum) == true)
                    {
                        _thisArr = _classArr;
                    }
                    else
                    {
                        for (int i = _classArr.Length - 1; i >= 0; i--)
                        {
                            T _class = _classArr[i];
                            GameObject.DestroyImmediate(_class.gameObject);
                        }

                        if (_isCallRuntime == true)
                            Debug_C.Error_Func("?");

                        _thisArr = new T[_minNum];
                        string _typeNameStr = typeof(T).Name + "_";
                        _typeNameStr = _typeNameStr.Replace("SoundSystem_", "");
                        _typeNameStr = _typeNameStr.Replace("_Script", "");

                        for (int i = 0; i < _minNum; i++)
                        {
                            string _name = StringBuilder_C.Append_Func(_typeNameStr, i.ToString());
                            GameObject _bgmObj = new GameObject(_name);
                            _thisArr[i] = _bgmObj.AddComponent<T>();
                            _bgmObj.transform.SetParent(this.transform);
                        }
                    }
                }
            }
        }

        public void PlayBgm_Func(BgmType _bgmType, bool _isLooping = true)
        {
            BgmData _bgmData = this.bgmDataDic.GetValue_Func(_bgmType);

            this.PlayBgm_Func(_bgmData, _isLooping);
        }
        private void PlayBgm_Func(BgmData _bgmData, bool _isLooping = true)
        {
            this.PlayBgm_Func(true, (_bgmClass) =>
            {
                _bgmClass.Activate_Func(_bgmData, _isLooping);
            });
        }
        public void PlayBgmBefore_Func()
        {
            this.PlayBgm_Func(true, (_bgmClass) =>
            {
                _bgmClass.Activate_Func();
            });
        }

        private void PlayBgm_Func(bool _isReverse, Action<SoundSystem_Bgm_Script> _del)
        {
            this.currentBgmClass?.Deactivate_Func();

            SoundSystem_Bgm_Script _bgmClass = this.bgmCQ.Dequeue_Func(_isReverse);
            _del(_bgmClass);

            this.currentBgmClass = _bgmClass;
        }

        /// <summary>
        /// 현재 재생 중인 Bgm을 멈춥니다. (페이드-아웃)
        /// </summary>
        public void OffCurrentBgm_Func()
        {
            this.currentBgmClass?.Deactivate_Func();

            this.currentBgmClass = null;
        }

        public void PlaySfx_Func(SfxType _sfxType)
        {
            if (UserSystem_Manager.Instance.GetCommon.GetSfx_Func() == false)
                return;

            if(this.sfxDataDic.TryGetValue(_sfxType, out SfxData _sfxData) == true)
            {
                SoundSystem_Sfx_Script _sfxClass = this.sfxCQ.Dequeue_Func();
                _sfxClass.Activate_Func(_sfxData);
            }
            else
            {
                Debug_C.Log_Func("다음 효과음키는 없습니다. : " + _sfxType.ToString());
            }
        }

        public void SetBgm_Func(bool _isOn)
        {
            foreach (SoundSystem_Bgm_Script _bgmClass in bgmClassArr)
                _bgmClass.SetOn_Func(_isOn);
        }
        public void SetSfx_Func(bool _isOn)
        {
            foreach (SoundSystem_Sfx_Script _sfxClass in this.sfxClassArr)
                _sfxClass.SetOn_Func(_isOn);
        }

        public void SetSound_Func(bool _isOn)
        {
            bool _isBgm = UserSystem_Manager.Instance.GetCommon.GetBgm_Func();
            bool _isSfx = UserSystem_Manager.Instance.GetCommon.GetSfx_Func();

            if (_isOn == true)
            {
                if (_isBgm == true)
                    this.SetBgm_Func(true);

                if (_isSfx == true)
                    this.SetSfx_Func(true);
            }
            else
            {
                if (_isBgm == true)
                    this.SetBgm_Func(false);

                if (_isSfx == true)
                    this.SetSfx_Func(false);
            }
        }

#if UNITY_EDITOR
        void Reset()
        {
            this.gameObject.name = this.GetType().Name;
            this.OnGenerateSoundComponent_Func(false);
        }
#endif

        private void CallDel_BgmChanged_Func(bool _isOn)
        {
            foreach (var _bgmClass in this.bgmClassArr)
            {
                _bgmClass.SetOn_Func(_isOn);
            }
        }

#if UNITY_EDITOR
        [Button("캐싱 ㄱㄱ ~")]
        private void CallEdit_Catch_Func()
        {
            this.OnGenerateSoundComponent_Func(false);
        }

        public static void CallEdit_PlayBgm_Func(BgmData _bgmData)
        {
            if (Instance == null)
                Instance = FindObjectOfType<SoundSystem_Manager>();

            if (Instance == null)
            {
                Debug.LogError("Sound 매니저 없음");
                return;
            }

            SoundSystem_Bgm_Script _bgmClass = Instance.bgmClassArr[0];
            _bgmClass.Activate_Func(_bgmData);
        }
        public static void CallEdit_EndBgm_Func()
        {
            if (Instance == null)
                Instance = FindObjectOfType<SoundSystem_Manager>();

            if (Instance == null)
            {
                Debug.LogError("Sound 매니저 없음");
                return;
            }

            SoundSystem_Bgm_Script _bgmClass = Instance.bgmClassArr[0];
            _bgmClass.Deactivate_Func();
        }
        public static void CallEdit_PlaySfx_Func(SfxData _sfxData)
        {
            if (Instance == null)
                Instance = FindObjectOfType<SoundSystem_Manager>();

            if (Instance == null)
            {
                Debug.LogError("Sound 매니저 없음");
                return;
            }

            SoundSystem_Sfx_Script _sfxClass = Instance.sfxClassArr[0];
            _sfxClass.Activate_Func(_sfxData);
        } 
#endif
    }
}