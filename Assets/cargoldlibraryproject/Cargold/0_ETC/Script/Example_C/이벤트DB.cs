using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
namespace Cargold.Example {
    namespace 카골드
    {
        public partial class LibraryRemocon
        {        public static LibraryRemocon Instance;        public UtilityClassData utilityClassData;
            public partial class UtilityClassData
            {            public 이벤트DB eventSystemData;
                public partial class 이벤트DB
                {
                    [FoldoutGroup("카라리"), LabelText("이벤트 프리팹"), ReadOnly] public 이벤트스크립트[] evtClassArr = null;
                    [FoldoutGroup("카라리"), SerializeField, HideLabel] private 이벤트키드롭다운컨테이너 eventKeyDropdownContainer = new 이벤트키드롭다운컨테이너();

                    public 이벤트키드롭다운컨테이너 GetEventKeyDropdownContainer
                    {
                        get
                        {
                            if (this.eventKeyDropdownContainer == null)
                                this.eventKeyDropdownContainer = new 이벤트키드롭다운컨테이너();

                            return this.eventKeyDropdownContainer;
                        }
                    }

                    private 이벤트스크립트[] SetEventClassArr_Func()
                    {
                        이벤트스크립트[] _eventClassArr = Resources.LoadAll<이벤트스크립트>(Cargold.EventSystem.EventTool<이벤트스크립트>.EventDataPathStr);
                        this.evtClassArr = _eventClassArr;
                        return _eventClassArr;
                    }

    #if UNITY_EDITOR
                    [FoldoutGroup("카라리"), SerializeField, LabelText("이벤트 템플릿")] private 이벤트스크립트 templateEventClass = null;

                    public 이벤트스크립트 GetTemplateEventClass
                    {
                        get
                        {
                            if(this.templateEventClass == null)
                                this.templateEventClass = Resources_C.Load<이벤트스크립트>(Cargold.LibraryRemocon.UtilityClassData.EventSystemData.TemplateNameStr);

                            return this.templateEventClass;
                        }
                    }

                    public 이벤트스크립트[] CallEdit_GetWholeEvtClass_Func()
                    {
                        return this.SetEventClassArr_Func();
                    }
                    public void CallEdit_OnCatchingEventPrefab_Func()
                    {
                        this.SetEventClassArr_Func();

                        



                        bool _isSave = false;

                        List<드롭다운아이템> _objDropdownItemList = this.GetEventKeyDropdownContainer.GetObjDropdownItemList;
                        if (this.evtClassArr.Length != _objDropdownItemList.Count)
                        {
                            _isSave = true;

                            this.GetEventKeyDropdownContainer.OnResetList_Func();

                            foreach (이벤트스크립트 _evtGroupClass in this.evtClassArr)
                                this.GetEventKeyDropdownContainer.CallEdit_AddItem_Func(_evtGroupClass);
                        }

                        for (int i = _objDropdownItemList.Count - 1; i >= 0; i--)
                        {
                            if (_objDropdownItemList[i] == null)
                            {
                                _isSave = true;

                                _objDropdownItemList.RemoveAt(i);
                            }
                        }

                        //if (_isSave == true)
                        //    Editor_C.SetSaveAsset_Func(default);
                    }
    #endif
                }
            }
        }
    }

    public class 이벤트키드롭다운컨테이너 : Cargold.ObjDropdownContainer<Cargold.Example.이벤트스크립트, 드롭다운아이템, 드롭다운키>
    {
        protected override 드롭다운아이템 GetInstance_Func(Cargold.Example.이벤트스크립트 _obj)
        {
            return new 드롭다운아이템(_obj);
        }
    }

    public class 드롭다운아이템 : Cargold.ObjDropdownItem<Cargold.Example.이벤트스크립트>
    {
        public 드롭다운아이템(Cargold.Example.이벤트스크립트 _obj) : base(_obj) { }
    }

    public class 드롭다운키 : Cargold.ObjDropdownKey<Cargold.Example.이벤트스크립트>
    {
        public 드롭다운키(Cargold.Example.이벤트스크립트 _key) : base(_key) { }

#if UNITY_EDITOR
        protected override IEnumerable CallEdit_GetKeyArr_Func()
        {
            return Cargold.Example.카골드.LibraryRemocon.Instance.utilityClassData.eventSystemData.GetEventKeyDropdownContainer.GetObjDropdownItemList;
        } 
#endif
    }
} // End