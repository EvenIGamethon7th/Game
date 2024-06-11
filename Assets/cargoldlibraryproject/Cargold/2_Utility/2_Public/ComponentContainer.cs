using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold
{
    [System.Serializable]
    public abstract class ComponentContainer<T, tData> where T : Component, ComponentContainer<T, tData>.IComponentData where tData : struct
    {
        [SerializeField, LabelText("Obj")] private GameObject targetObj;
        [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = false, Expanded = true
                , OnBeginListElementGUI = "CallEdit_OnBeginListElementGUI_Func"
                , CustomRemoveElementFunction = "CallEdit_RemoveElem_Func"
                , OnEndListElementGUI = "CallEdit_EndDrawListElement_Func")]
        [SerializeField, LabelText("목록")] private T[] componentArr;
        private System.Action elemChangedDel;

        public T[] GetComponentArr => this.componentArr;

        public ComponentContainer(GameObject _obj, System.Action _elemChangedDel = null)
        {
            this.Init_Func(_obj, _elemChangedDel);
        }

        public void Init_Func(GameObject _obj, System.Action _elemChangedDel = null)
        {
            this.targetObj = _obj;
            this.elemChangedDel = _elemChangedDel;
        }

        protected abstract string GetComponentNameStr_Func(tData _typeData);
        protected abstract bool TryGetType_Func(tData _typeData, out System.Type _type);

#if UNITY_EDITOR
        private void CallEdit_OnBeginListElementGUI_Func(int _id)
        {
            T _component = this.componentArr[_id];
            string _str = this.GetComponentNameStr_Func(_component.GetTypeData);
            Sirenix.Utilities.Editor.SirenixEditorGUI.BeginBox(_str);
        }
        private void CallEdit_RemoveElem_Func(T _component)
        {
            string _str = this.GetComponentNameStr_Func(_component.GetTypeData);

            this.componentArr = this.componentArr.GetRemove_Func(_component);
            Component.DestroyImmediate(_component, true);

            Debug_C.Log_Func(_str + " 제거");

            this.elemChangedDel?.Invoke();
        }
        private void CallEdit_EndDrawListElement_Func(int _id)
        {
            Sirenix.Utilities.Editor.SirenixEditorGUI.EndBox();
        }
        public void CallEdit_RemoveAll_Func()
        {
            for (int i = componentArr.Length - 1; i >= 0; i--)
            {
                T _component = this.componentArr[i];
                string _str = this.GetComponentNameStr_Func(_component.GetTypeData);
                Component.DestroyImmediate(_component, true);

                Debug_C.Log_Func(_str + " 제거");
            }

            this.componentArr = null;
        }

        [PropertySpace(20f), Button("컴포넌트 추가", Style = ButtonStyle.Box)]
        private void CallEdit_AddComponent_Func(tData _typeData)
        {
            if(this.TryGetType_Func(_typeData, out System.Type _type) == false)
            {
                Debug_C.Error_Func("?");
                return;
            }

            T _component = this.targetObj.AddComponent(_type) as T;
            this.componentArr = this.componentArr.GetAdd_Func(_component);

            this.elemChangedDel?.Invoke();

            string _str = this.GetComponentNameStr_Func(_component.GetTypeData);
            Debug_C.Log_Func(_str + " 추가");
        }
#endif

        public interface IComponentData
        {
            public tData GetTypeData { get; }
        }
    }
}