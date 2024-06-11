using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cargold.EventSystem
{
    [CreateAssetMenu(fileName = "Test", menuName = "Test")]
    public class EventGroupSobj : SerializedScriptableObject
    {
        public List<Cargold.EventSystem.Event_Script> evtClassList;

        public EventGroupSobj()
        {
            this.evtClassList = new List<Event_Script>();
        }

#if UNITY_EDITOR
        public void CallEdit_UnitTest_Func()
        {
            bool _isSave = false;
            for (int i = this.evtClassList.Count - 1; i >= 0; i--)
            {
                if (this.evtClassList[i] == null)
                {
                    _isSave = true;

                    this.evtClassList.RemoveAt(i);

                    Debug_C.Log_Func(this.name + "이그젝에 Null 이벤트 제거");
                }
                else
                {
                    this.evtClassList[i].CallEdit_UnitTest_Func();
                }
            }

            if(_isSave == true)
                Editor_C.SetSaveAsset_Func(this);
        }

        public void CallEdit_AddEvt_Func(Cargold.EventSystem.Event_Script _evtClass)
        {
            this.evtClassList.AddNewItem_Func(_evtClass);

            Editor_C.SetSaveAsset_Func(this);
        }

        public bool CallEdit_TryRemoveEvt_Func(Cargold.EventSystem.Event_Script _evtClass)
        {
            bool _isResult = this.evtClassList.TryRemove_Func(_evtClass);

            Editor_C.SetSaveAsset_Func(this);

            return _isResult;
        } 
#endif
    }
}