using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cargold.FrameWork
{ 
    [DefaultExecutionOrder(-100)] 
    public class GameSystem_Manager : SerializedMonoBehaviour
    {
        [BoxGroup(CargoldLibrary_C.GetLibraryKorStr), HideLabel, SerializeField] protected InitializeData[] iInitDataArr = new InitializeData[0];
        [SerializeField, LabelText("모바일 콘솔로그 에셋")] private Reporter reporterClass = null;
        [SerializeField, LabelText("자동 이니셜라이즈 여부")] private bool isAutoInit = true;
        [SerializeField, LabelText("Dont Destory 여부")] private bool isDontDestroy = true;
        private static GameSystem_Manager Instance;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]

        private void Awake()
        {
            bool b = true;

            if (Instance != null && Instance != this)
            {
                b = false;
                Destroy(gameObject);
            }
            else if(Instance == null)
            {
                Instance = this;
            }

            if (this.isAutoInit == true && b)
            {
                this.Init_Func();
            }
        }

        protected virtual void Init_Func()
        {
            // 문자열을 문화권에 구분하여 사용하지 않습니다.
            // https://learn.microsoft.com/ko-kr/dotnet/api/system.globalization.cultureinfo.invariantculture?view=net-7.0
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            // 프레임 통제
            Application.targetFrameRate = 60;

            // 시간 활성화
            TimeSystem_Manager.Instance.Init_Func();

            // 0 : 스스로를 초기화할 때
            // 1 : 외부 매니저 클래스에 접근할 때
            // 2 : 유저 데이터에 접근할 때
            for (int _layer = 0; _layer < 3; _layer++)
            {
                foreach (var _iInitData in iInitDataArr)
                {
                    IInitializer _iInitializer = _iInitData.iInitializer;

                    if(_iInitializer != null)
                    {
                        _iInitializer.Init_Func(_layer);
                    }
                    else
                    {
                        Debug_C.Error_Func("다음 타입이 게임매니저의 초기화 목록에서 누락되었습니다. : " + _iInitData.GetTypeNameStr); 
                    }
                }
            }

#if !Test_Cargold || UNITY_EDITOR
            _DestoryReport_Func();
#endif

            if(this.isDontDestroy == true)
                GameObject.DontDestroyOnLoad(this.gameObject);

            void _DestoryReport_Func()
            {
                if (this.reporterClass != null)
                    GameObject.Destroy(this.reporterClass.gameObject);

                this.reporterClass = null;
            }
        }

#if UNITY_EDITOR
        [Button("추가")]
        public void CallEdit_AddInitializer_Func(IInitializer _initializer)
        {
            foreach (var _iInitData in this.iInitDataArr)
            {
                if(_iInitData.iInitializer == _initializer)
                {
                    Debug_C.Error_Func("다음의 이니셜라이저는 이미 등록되어 있습니다. : " + _initializer.GetType());

                    return;
                }
            }

            InitializeData _initData = new InitializeData();
            _initData.CallEdit_Add_Func(_initializer);

            this.iInitDataArr = this.iInitDataArr.GetAdd_Func(_initData);
        }
        [Button("캐싱 ㄱㄱ ~")]
        private void CallEdit_Catching_Func() 
        {
            for (int i = iInitDataArr.Length - 1; i >= 0; i--)
            {
                InitializeData _iInitData = this.iInitDataArr[i];
                if (_iInitData.iInitializer == null)
                {
                    Debug_C.Log_Func("GSM) 다음 이니셜라이저가 Null이므로 배열에서 제거했습니다. : " + _iInitData.GetTypeNameStr);
                    this.iInitDataArr = this.iInitDataArr.GetRemove_Func(_iInitData);
                }
            }

            IInitializer[] _iInittializerArr = this.gameObject.GetComponentsInChildren<IInitializer>();

            foreach (IInitializer _iInittializer in _iInittializerArr)
            {
                bool _isAdd = true;

                foreach (InitializeData _iInitData in this.iInitDataArr)
                {
                    if(_iInitData.iInitializer == _iInittializer)
                    {
                        _isAdd = false;
                        break;
                    }
                }

                if(_isAdd == true)
                    this.CallEdit_AddInitializer_Func(_iInittializer);
            }
        }
        void Reset()
        {
            this.gameObject.name = this.GetType().Name;
        }
#endif

        public interface IInitializer
        {
            void Init_Func(int _layer);
        }

        public class InitializeData
        {
            [HideLabel, OnValueChanged("CallEdit_SetType_Func")] public IInitializer iInitializer;
            [HideLabel, SerializeField, ReadOnly] private string typeNameStr;

            public string GetTypeNameStr => this.typeNameStr;

#if UNITY_EDITOR
            public void CallEdit_Add_Func(IInitializer _initer)
            {
                this.iInitializer = _initer;

                this.CallEdit_SetTypeName_Func(_initer);
            }

            private void CallEdit_SetType_Func()
            {
                if (this.iInitializer != null)
                {
                    this.CallEdit_SetTypeName_Func(this.iInitializer);
                }
            }
            public void CallEdit_SetTypeName_Func(IInitializer _initer)
            {
                System.Type _type = _initer.GetType();
                string _typeStr = _type.Name;
                this.typeNameStr = _typeStr;

                Debug_C.Log_Func("GSM : " + _typeStr);
            }
#endif
        }
    } 
}