using System.IO;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor.AddressableAssets;
#endif

namespace Cargold
{
    public partial class LibraryRemocon
    {
        public partial class UtilityClassData
        {
            [InlineProperty, HideLabel] public AddressableData addressableData = new AddressableData();
            [FoldoutGroup(AddressableData.KOR_STR), Indent(UtilityClassData.IndentLv)]
            public partial class AddressableData 
            {
                public const string KOR_STR = "어드레서블 이름 생성기";
                private const string CLASS_NAME = "AddressableTable";
                private const string  EXCEPT_GROUP = "Built In Data";
                [BoxGroup(Editor_C.Mandatory), LabelText("Script 생성 경로"), FolderPath]
                public string scriptPath = CargoldLibrary_C.GetInitError;
                
#if UNITY_EDITOR
                [Button("어드레서블 이름 스크립트 생성")]
                private void CreateScriptData()
                {
                    string dataKeyScriptPath = StringBuilder_C.Append_Func(scriptPath + "/", CLASS_NAME, ".cs");
                    string totalDataKeyStr = default;
                    var groups = AddressableAssetSettingsDefaultObject.Settings.groups;
                    foreach (var group in groups)
                    {
                        if(group.Name == EXCEPT_GROUP)
                            continue;
                        foreach (var entry in group.entries)
                        {
                            string dataKeyVarStr = string.Format(GetScriptGenerationData, $"{group.Name}_{entry}", $"{entry}");
                            totalDataKeyStr = StringBuilder_C.Append_Func(totalDataKeyStr, dataKeyVarStr);
                        }
                    }
                    string dataKeyScriptStr = string.Format(GetScriptGenerationClass, CLASS_NAME, totalDataKeyStr);
                    File.WriteAllText(dataKeyScriptPath, dataKeyScriptStr);
                    Debug_C.Log_Func("생성 완료");
                    

                }
#endif
            }


            public const string GetScriptGenerationData = @"public const string {0} = ""{1}"";";
            public const string GetScriptGenerationClass = @"
public static partial class {0}
{{{1}
}}
";

        }
    }
}