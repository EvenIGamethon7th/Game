using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cargold.FrameWork
{
    [System.Serializable]
    public class UI_C : DataGroupTemplate
    {
        public const string KrStr = "유아이";
        public const string Str = "UI";
        public override string GetTypeNameStr => UI_C.Str;

        [FoldoutGroup("매터리얼"), LabelText("그레이 스케일"), ShowInInspector] public Material GetGrayMat
        {
            get
            {
#if UNITY_EDITOR
                if (this.grayMat == null)
                {
                    string _path = StringBuilder_C.Append_Func(LibraryRemocon.Instance.GetExternalAssetFolderPath, "/GrayScale/UIGrayscale.mat");
                    this.grayMat = Editor_C.GetLoadAssetAtPath_Func<Material>(_path);

                    Editor_C.SetSaveAsset_Func(this);
                }
#endif

                return this.grayMat;
            }
        }
        [SerializeField] private Material grayMat;

#if DoTween_C
        [FoldoutGroup("버튼"), LabelText("눌렀을 때 Twn 시간")] public float btnScaleDownTime = .1f;
        [FoldoutGroup("버튼"), LabelText("눌렀을 때 Twn 크기 감소율")] public float btnScaleDown = .1f;
        [FoldoutGroup("버튼"), LabelText("뗄 때 Twn 시간")] public float btnPunchTime = .4f;
        [FoldoutGroup("버튼"), LabelText("뗄 때 Twn Ease")] public DG.Tweening.Ease btnOutEase = DG.Tweening.Ease.OutElastic; 
#endif

        [FoldoutGroup("트윈"), LabelText("세기")] public float twn_Power = 1f;
        [FoldoutGroup("트윈"), LabelText("지속시간")] public float twn_Duraion = .25f;

        [FoldoutGroup("지속 터치"), LabelText("시작 지연")] public float continuousBtn_BeginDelay = .25f;
        [FoldoutGroup("지속 터치"), LabelText("지연 감속 간격")] public float continuousBtn_DecreaseInterval = .02f;
        [FoldoutGroup("지속 터치"), LabelText("최소 지연 간격")] public float continuousBtn_MaxInterval = .02f;
        [FoldoutGroup("지속 터치/알림"), LabelText("발동 횟수 누적 조건")] public int continuousBtn_StackCount = 4;
        [FoldoutGroup("지속 터치/알림"), LabelText("발동 횟수 차감 지연 시간")] public float continuousBtn_StackDelay = .5f;

        [FoldoutGroup("게이지"), LabelText("감소 시간")] public float gauge_Duraion = .25f;
        [FoldoutGroup("게이지"), LabelText("대기 시간")] public float gauge_DelayTime = .1f;

        [FoldoutGroup("이미지"), LabelText("긍정 버튼"), SerializeField] private Sprite btnSprite_Positive = null;
        [FoldoutGroup("이미지"), LabelText("중립 버튼"), SerializeField] private Sprite btnSprite_Neutral = null;
        [FoldoutGroup("이미지"), LabelText("부정 버튼"), SerializeField] private Sprite btnSprite_Negetive = null;

        [FoldoutGroup("토스트"), LabelText("앵커 사이즈")] public float toast_sizeX = 0f;

        public Sprite GetBtnSprite_Func(BtnType _btnType)
        {
            switch (_btnType)
            {
                case BtnType.None:
                case BtnType.Neutral:       return this.btnSprite_Neutral;
                case BtnType.Positive:      return this.btnSprite_Positive;
                case BtnType.Negetive:      return this.btnSprite_Negetive;

                default:
                    Debug_C.Error_Func("_btnType : " + _btnType);
                    return null;
            }
        }

#if UNITY_EDITOR
        public override void CallEdit_OnDataImport_Func()
        {
            base.CallEdit_OnDataImport_Func();
        }
#endif

        public enum BtnType
        {
            None = 0,

            Positive,
            Neutral,
            Negetive,
        }
    }
}