using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

namespace Cargold.Remocon
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class Version_Script : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI versionTmp = null;

        public GameObject GetPrefabObj_Func()
        {
            return this.gameObject;
        }

        public void SetVersion_Func(string _verStr)
        {
            this.versionTmp.text = _verStr;
        }

#if UNITY_EDITOR
        [Button("리모콘에 캐싱 ㄱㄱ ~")]
        private void Reset()
        {
            if (this.TryGetComponent(out TextMeshProUGUI _tmp) == true)
            {
                this.versionTmp = _tmp;

                ProjectRemocon.Instance.buildSystem.CallEdit_Subscribe_Version_Func(this);
            }
            else
            {
                Debug_C.Error_Func("?");
            }
        } 
#endif
    }
}