using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cargold.FrameWork
{
    [System.Serializable]
    public class Define_C : DataGroupTemplate
    {
        public const string KrStr = "디파인";
        public const string Str = "Define";
        public const string Camera = "카메라";
        public const string TimeS = Cargold.TimeSystem.TimeSystem_Manager.KorStr + "/";
        public const string CameraS = Camera + "/";
        public const string Tile = "타일";

        [FoldoutGroup(SaveSystem.SaveSystem_Manager.Str), LabelText("세이브 기능 활성화")] public bool isSaveActivate = false;
        [FoldoutGroup(SaveSystem.SaveSystem_Manager.Str), LabelText("저장 간격")] public float saveInterval = 3f;
        [FoldoutGroup(Cargold.TimeSystem.TimeSystem_Manager.KorStr), HorizontalGroup(TimeS + "1"), BoxGroup(TimeS + "1/일"), HideLabel] public int testAddDay = 0;
        [FoldoutGroup(Cargold.TimeSystem.TimeSystem_Manager.KorStr), HorizontalGroup(TimeS + "1"), BoxGroup(TimeS + "1/시"), HideLabel] public int testAddHour = 0;
        [FoldoutGroup(Cargold.TimeSystem.TimeSystem_Manager.KorStr), HorizontalGroup(TimeS + "1"), BoxGroup(TimeS + "1/분"), HideLabel] public int testAddMin = 0;
        [FoldoutGroup(Cargold.TimeSystem.TimeSystem_Manager.KorStr), HorizontalGroup(TimeS + "1"), BoxGroup(TimeS + "1/초"), HideLabel] public int testAddSec = 0;
        [LabelText("스프라이트 기본 매터리얼"), ShowInInspector, PropertyOrder(-1)]
        public Material GetSpriteDefaultMat
        {
            get
            {
                if (this.spriteDefaultMat == null)
                    this.spriteDefaultMat = new Material(Shader.Find("Sprites/Default"));

                return this.spriteDefaultMat;
            }
        }
        [SerializeField, ReadOnly] private Material spriteDefaultMat;
        public override string GetTypeNameStr => Define_C.Str;

        [FoldoutGroup(Camera), BoxGroup(CameraS + "팔로우"), LabelText("도착 거리 조건")] public float camera_follow_distCon = 0.01f;
        [FoldoutGroup(Camera), BoxGroup(CameraS + "팔로우"), LabelText("이동 속도")] public float camera_follow_Speed = 2f;
        [FoldoutGroup(Camera), BoxGroup(CameraS + "팔로우"), LabelText("오프셋")] public Vector3 camera_follow_Offset = Vector3.back * 10f;
        [FoldoutGroup(Camera), BoxGroup(CameraS + "쉐이크"), LabelText("데이터")]
        public ShakeData[] shakeDataArr = new ShakeData[]
            {
                    new ShakeData(0.2f, 0.04f, 1f)
            };

        [FoldoutGroup(Tile), LabelText("빈 텍스쳐")] public Texture emptyTexture;

#if UNITY_EDITOR
        public override void CallEdit_OnDataImport_Func()
        {
            base.CallEdit_OnDataImport_Func();

            Editor_C.TryGetFindFolderPath_Func("0_ETC", out string _exampleScriptFolderPath);
            string _path = Editor_C.GetPath_Func(_exampleScriptFolderPath, "Sprite", "Trash");
            this.emptyTexture = Editor_C.GetLoadAssetAtPath_Func<Texture>(_path, true, true);
        }
#endif

        [System.Serializable]
        public struct ShakeData
        {
            [LabelText("쉐이크 세기")] public float power;
            [LabelText("쉐이크 간격")] public float interval;
            [LabelText("쉐이크 시간")] public float duratin;

            public ShakeData(float power, float interval, float duratin)
            {
                this.power = power;
                this.interval = interval;
                this.duratin = duratin;
            }
        }
    }
}