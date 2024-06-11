using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine.Events;
using Cargold;

public class UI_CallAni_Script : MonoBehaviour
{
    [SerializeField] private Data[] dataArr;

    public void CallAni_Func(string _key)
    {
        foreach (Data _data in this.dataArr)
        {
            if (_data.key.IsCompare_Func(_key) == true)
            {
                _data.unityEvent.Invoke();
                break;
            }
        }
    }

    [System.Serializable]
    private class Data
    {
        public string key;
        public UnityEvent unityEvent;
    }
}