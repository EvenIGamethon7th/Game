using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold.ParallaxScrolling
{
    public abstract class ParallaxScrolling_Script : MonoBehaviour
    {
        [SerializeField, LabelText("Y축 고정")] private bool isStaticY = true;
        [SerializeField, LabelText("카메라 뷰포트 X값")] private float viewportX = 0f;
        [SerializeField, LabelText("엘렘 그룹들")] private ParallaxScrollingElemGroup[] elemGroupArr = null;

        public void Init_Func()
        {
            this.Deactivate_Func(true);
        }

        public virtual void Activate_Func()
        {
            foreach (ParallaxScrollingElemGroup _elemGroup in this.elemGroupArr)
                _elemGroup.Activate_Func(this.viewportX);
        }

        public void OnScrolling_Func(Vector2 _pos)
        {
            if (this.isStaticY == true)
                _pos = new Vector2(_pos.x, 0f);

            foreach (ParallaxScrollingElemGroup _elemGroup in this.elemGroupArr)
                _elemGroup.OnScrolling_Func(this.viewportX, _pos);
        }

        public virtual void Deactivate_Func(bool _isInit = false)
        {
            if (_isInit == false)
            {

            }
        }

#if UNITY_EDITOR
        [FoldoutGroup(CargoldLibrary_C.GetLibraryKorStr), ShowInInspector, OnValueChanged("CallEdit_Scroll_Func")] private float callEditPosX;

        private void CallEdit_Scroll_Func()
        {
            this.OnScrolling_Func(new Vector2(this.callEditPosX, 0f));
        }

        [FoldoutGroup(CargoldLibrary_C.GetLibraryKorStr), Button(CargoldLibrary_C.CatchingStr)]
        private void CallEdit_Test_Func()
        {
            this.Activate_Func();
        }
#endif
    }
    
    [System.Serializable]
    public class ParallaxScrollingElemGroup
    {
        [SerializeField, LabelText("이동량 보정율"), PropertyRange(0d, 2d)] private float addMovePosRate = 1f;
        [SerializeField, LabelText("엘렘들")] private ParallaxScrollingElem[] elemArr;

        public void Activate_Func(float _viewportX)
        {
            foreach (ParallaxScrollingElem _elem in this.elemArr)
                _elem.Activate_Func(_viewportX, this.addMovePosRate);
        }

        public void OnScrolling_Func(float _viewportX, Vector2 _pos)
        {
            Vector2 _modifyPos = _pos * (1f - this.addMovePosRate);

            foreach (ParallaxScrollingElem _elem in this.elemArr)
                _elem.OnScrolling_Func(_viewportX, _modifyPos);
        }

#if UNITY_EDITOR
        public void CallEdit_AddSrdr_Func(SpriteRenderer[] _srdrArr)
        {
            foreach (SpriteRenderer _srdr in _srdrArr)
                this.elemArr = this.elemArr.GetAdd_Func(new ParallaxScrollingElem(_srdr.transform));
        }
#endif
    }

    [System.Serializable]
    public class ParallaxScrollingElem
    {
        [SerializeField, LabelText("타겟 Trf")] private Transform targetTrf = null;
        [ShowInInspector, ReadOnly] private Vector2 initPos;

        public ParallaxScrollingElem(Transform _targetTrf)
        {
            this.targetTrf = _targetTrf;
        }

        public void Activate_Func(float _viewportX, float _addMovePosRate)
        {
            float _targetLocalPosX = this.targetTrf.localPosition.x;

            // 카메라 뷰포트만큼 보정
            float _xGapByViewport = 0f;
            if (_viewportX < _targetLocalPosX)
                _xGapByViewport = _targetLocalPosX - _viewportX;
            else if (_targetLocalPosX < -_viewportX)
                _xGapByViewport = _targetLocalPosX + _viewportX;

            // 이동 가중치만큼 선 이동
            float _xGap = _xGapByViewport * (_addMovePosRate - 1f);

            this.targetTrf.SetPosX_Func(_targetLocalPosX + _xGap, Space.Self);

            this.initPos = this.targetTrf.position;
        }

        public void OnScrolling_Func(float _viewportX, Vector2 _pos)
        {
            this.targetTrf.position = this.initPos + _pos;
        }
    }
}