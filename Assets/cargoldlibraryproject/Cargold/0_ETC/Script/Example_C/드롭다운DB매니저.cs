using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold.Example
{
    public class 드롭다운DB매니저
    {
        public static 드롭다운DB매니저 Instance;

        public Example GetExample;

        public class Example : DB.TableImporter.DataGroup_C<ExampleData>
        {
            [SerializeField] private string[] keyArr;

            public string[] GetKeyArr => this.keyArr;
        }
    }

    public class ExampleData : DB.TableImporter.Data_C
    {
#if UNITY_EDITOR
        public override void CallEdit_OnDataImport_Func(string[] _cellDataArr)
        {
            throw new System.NotImplementedException();
        } 
#endif
    }
}