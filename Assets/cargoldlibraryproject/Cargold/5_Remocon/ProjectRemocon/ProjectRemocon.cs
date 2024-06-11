using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold.Infinite;
using Cargold.FrameWork;
using Cargold.Remocon;
using TableImporterData = Cargold.LibraryRemocon.FrameWorkData.DatabaseData.TableImporterData;

namespace Cargold.Remocon
{
    [CreateAssetMenu(fileName = ProjectRemocon.Str, menuName = CargoldLibrary_C.GetLibraryFolderName + Editor_C.SeparatorStr + ProjectRemocon.Str)]
    public sealed class ProjectRemocon : SerializedScriptableObject, FrameWork.UserSystem_Manager.IUserDataContainer
    {
        #region Instance
        private static ProjectRemocon instance;
        public static ProjectRemocon Instance
        {
            get
            {
                if (instance == null)
                {
                    System.Type _projectRemoconType = typeof(Cargold.Remocon.ProjectRemocon);
                    string _pathStr = Editor_C.GetPath_Func(CargoldLibrary_C.GetLibraryFolderName, _projectRemoconType.Name);
                    ProjectRemocon _projectRemocon = Resources_C.Load<ProjectRemocon>(_pathStr);
                    _projectRemocon.Init_Func();

                    instance = _projectRemocon;
                }

                return instance;
            }
        }
        #endregion

        public const string Str = "ProjectRemocon";
        public const string TableImpoterStr = "3_테이블";

        [FoldoutGroup("1_유저 데이터")
            , InfoBox("유저 데이터 연결하셈!!!", InfoMessageType.Error, "IsUserDataEmpty_Func")
            , InlineEditor(InlineEditorObjectFieldModes.Boxed), HideLabel, GUIColor(1f, .85f, .85f)]
        public Test_UserData_C userData;

        [FoldoutGroup("2_데이터베이스"), InlineEditor(InlineEditorObjectFieldModes.Hidden), HideLabel, GUIColor(.9f, 1f, .9f)]
        public IDatabase_C database;

        [FoldoutGroup(TableImpoterStr), InlineEditor(InlineEditorObjectFieldModes.Hidden), HideLabel, GUIColor(.85f, .85f, 1f)]
        public ITableImporter_C tableImporter;

        [FoldoutGroup(TableImpoterStr), LabelText("테이블 목록"), GUIColor(.85f, .85f, 1f)
            , ListDrawerSettings(
            Expanded = true
            , HideAddButton = true
            , HideRemoveButton = true
            , AlwaysAddDefaultValue = false, DraggableItems = false, ShowIndexLabels = false, ShowItemCount = false, ShowPaging = false)]
        public List<TableImportRemocon> tableImportRemoconList;

        [FoldoutGroup("4_치트키"), InlineProperty, HideLabel, GUIColor(1f, 1f, .85f)]
        public ProjectCheat cheatClass = new ProjectCheat();

        [FoldoutGroup("5_빌드 시스템"), HideLabel, GUIColor(.85f, 1f, 1f)] public BuildSystem buildSystem = new BuildSystem();

        public void Init_Func()
        {

        }

        public bool IsUserDataEmpty_Func()
        {
            return this.userData is null;
        }
        public bool IsTableEmpty_Func()
        {
            return this.database == null;
        }

        public UserData_C GetUserData_Func()
        {
            if (this.IsUserDataEmpty_Func() == true)
                return null;

            return this.userData.GetUserData_Func();
        }

#if UNITY_EDITOR
        [FoldoutGroup(TableImpoterStr), Button("테이블 목록 동기화")]
        private void CallEdit_OnGoogleAccess_Func()
        {
            TableImporterData.TableImporterRemocon[] _remoconClassArr = LibraryRemocon.Instance.frameWorkData.databaseData.tableImporterData.GetRemoconClassArr;

            this.tableImportRemoconList.Clear();

            foreach (TableImporterData.TableImporterRemocon _remoconClass in _remoconClassArr)
            {
                int _remoconID = _remoconClass.GetRemoconID;
                string _tableNameStr = _remoconClass.GetTableNameStr;
                TableImportRemocon _tableImportRemocon = new TableImportRemocon(_remoconID, _tableNameStr);
                this.tableImportRemoconList.Add(_tableImportRemocon);
            }
        }
#endif
    }

    public interface IDatabase_C { }
    public interface ITableImporter_C
    {
        void OnGoogleAccess_Func();
        void OnTableImporting_Func();
    }

    [System.Serializable]
    public class TableImportRemocon
    {
        [SerializeField, HideInInspector] private int remoconID;
        [HorizontalGroup("1", Width = 100f), SerializeField, HideLabel, ReadOnly] private string tableNameStr;

        private TableImporterData.TableImporterRemocon GetTableImporterRemocon
            => LibraryRemocon.Instance.frameWorkData.databaseData.tableImporterData.GetRemoconData_Func(this.remoconID);

        public TableImportRemocon(int _remoconID, string _tableNameStr)
        {
            this.remoconID = _remoconID;
            this.tableNameStr = _tableNameStr;
        }

        [HorizontalGroup("1"), Button(TableImporterData.GetStep1)]
        private void CallEdit_OnGoogleAccess_Func()
        {
            this.GetTableImporterRemocon.OnGoogleAccessVaild_Func();
        }
        [HorizontalGroup("1"), Button(TableImporterData.OnTableImportingStr)]
        private void CallEdit_OnTableImporting_Func()
        {
            this.GetTableImporterRemocon.OnExcelDownload_Func();
        }
    }
}