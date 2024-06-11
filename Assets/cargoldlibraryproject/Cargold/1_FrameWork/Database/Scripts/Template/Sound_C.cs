using Cargold;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cargold.FrameWork
{
    [System.Serializable]
    public class Sound_C : DataGroupTemplate
    {
        public const string KrStr = "사운드";
        public const string Str = "Sound";
        public const string BgmStr = "Bgm";
        public const string SfxStr = "Sfx";

        public override string GetTypeNameStr => Sound_C.Str;

        [TableList(AlwaysExpanded = true, DrawScrollView = false, ShowPaging = false, ShowIndexLabels = true), FoldoutGroup(BgmStr), HideLabel]
        public BgmData[] bgmDataArr;
        [TableList(AlwaysExpanded = true, DrawScrollView = false, ShowPaging = false, ShowIndexLabels = true), FoldoutGroup(SfxStr), HideLabel]
        public SfxData[] sfxDataArr;

        [FoldoutGroup(BgmStr), LabelText("배경음 페이드 속도")] public float bgmFadeSpeed = 1f;

#if UNITY_EDITOR
        [FoldoutGroup(BgmStr), Button("Bgm 종료")]
        public void CallEdit_BgmEnd_Func()
        {
            Cargold.FrameWork.SoundSystem_Manager.CallEdit_EndBgm_Func();
        } 
#endif

        [System.Serializable]
        public abstract class Record
        {
            [VerticalGroup("종류"), HideLabel, PropertyOrder(-1)]
            [InfoBox("Key 지정하셈!", "IsKeyNone", InfoMessageType = InfoMessageType.Error)]
            public abstract int sourceKey { get; }
            private bool IsKeyNone => sourceKey < 0;

            [VerticalGroup("클립"), HideLabel, AssetSelector]
            [InfoBox("클립 추가하셈!", "IsClipNull", InfoMessageType = InfoMessageType.Error)]
            public AudioClip clip;
            private bool IsClipNull { get { return this.clip == null; } }

            [VerticalGroup("볼륨"), HideLabel]
            [ProgressBar(0f, 1f)]
            public float volume = .8f;
        }

        [System.Serializable]
        public class BgmData : Record
        {
            [PropertyOrder(-1)] public BgmType bgmType;

            public override int sourceKey => this.bgmType;

#if UNITY_EDITOR
            [VerticalGroup("테스트 ㄱㄱ~"), Button("꾹")]
            private void CallEdit_Play_Func()
            {
                Cargold.FrameWork.SoundSystem_Manager.CallEdit_PlayBgm_Func(this);
            } 
#endif
        }

        [System.Serializable]
        public class SfxData : Record
        {
            [PropertyOrder(-1)] public SfxType sfxType;

            public override int sourceKey => this.sfxType;

#if UNITY_EDITOR
            [VerticalGroup("테스트 ㄱㄱ~"), Button("꾹")]
            private void CallEdit_Play_Func()
            {
                Cargold.FrameWork.SoundSystem_Manager.CallEdit_PlaySfx_Func(this);
            } 
#endif
        }
    }
}