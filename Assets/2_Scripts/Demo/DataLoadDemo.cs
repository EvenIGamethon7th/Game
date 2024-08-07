using System;
using Cargold.Observer;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.Demo
{
    /// <summary>
    /// 아이템 데이터 불러오기 예제
    /// </summary>
    public class DataLoadDemo : MonoBehaviour
    {
        [SerializeField] private ItemKey _itemKey;
        private ItemData _data;
        private void Start()
        {
            // 아이템의 설명을 갖고 오는 것 ( 로컬라이징 )
           //   _data = DataBase_Manager.Instance.GetItem.GetData_Func(_itemKey.GetKey);
           //   Debug.Log(LocalizeSystem_Manager.Instance.GetLcz_Func(_data.descKey));
           // // 아이템의 값어치를 가져오는 것
           //  Debug.Log($"해당 아이템의 값어치는 : {_data.value}");
             
        }
    }
    
    
    
}