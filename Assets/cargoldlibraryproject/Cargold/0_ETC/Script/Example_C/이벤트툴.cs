using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;
namespace Cargold.Example {
public class 이벤트툴 : Cargold.EventSystem.EventTool<이벤트스크립트>
{
    public LibraryRemocon.UtilityClassData.EventSystemData GetEventSystemData => LibraryRemocon.Instance.utilityClassData.eventSystemData;

#if UNITY_EDITOR
    [SerializeField, HideInInspector] private 이벤트스크립트 templateEventClass = null;

    //protected override Cargold.EventSystem.Event_Script[] CallEdit_GetWholeEvtClass_Func()
    //{
    //    return Cargold.Example.카골드.LibraryRemocon.Instance.utilityClassData.eventSystemData.CallEdit_GetWholeEvtClass_Func();
    //}
    protected override 이벤트스크립트 CallEdit_GetTemplate_Func()
    {
        return Cargold.Example.카골드.LibraryRemocon.Instance.utilityClassData.eventSystemData.GetTemplateEventClass;
    }
    protected override void CallEdit_OnCatching_Func()
    {
        Cargold.Example.카골드.LibraryRemocon.Instance.utilityClassData.eventSystemData.CallEdit_OnCatchingEventPrefab_Func();
    }
#endif
}
} // End