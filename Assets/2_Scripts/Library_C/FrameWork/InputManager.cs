
using System;
using System.Collections.Generic;
using _2_Scripts.Game.Map.Tile;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputManager : Singleton<InputManager>
    {

        private ReactiveProperty<int> mCurrentInputType = new ReactiveProperty<int>();
        public string[] exceptionLayerNames = { "Buff" };
        int mCombinedExceptionMask;
        
        public GraphicRaycaster uiRaycaster;
        public EventSystem eventSystem;
        
        
        public void Start()
        { 
            CombineExceptionMasks();
            var mouseDownStream = this.UpdateAsObservable().Where(_ => Input.GetMouseButtonDown(0));
            mouseDownStream.Subscribe(_ => OnMouseDown()).AddTo(this);
        }
        private void CombineExceptionMasks()
        {
            mCombinedExceptionMask = 0;
            foreach (var layerName in exceptionLayerNames)
            {
                int layer = LayerMask.NameToLayer(layerName);
                if (layer != -1)
                {
                    mCombinedExceptionMask |= 1 << layer;
                }
            }
        }
        private void OnMouseDown()
        {
            if (IsPointerOverUIObject())
            {
                Debug.Log("Touched UI element");
                return;
            }
            
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 touchPosition = new Vector2(mousePosition.x, mousePosition.y);

            RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero, 100, ~mCombinedExceptionMask, 0);
            if (hit.collider != null)
            {
                GameObject touchedObject = hit.collider.gameObject;
                mCurrentInputType.Value = touchedObject.layer;
                string layerName = LayerMask.LayerToName(touchedObject.layer);
                Debug.Log($"Touch Layer {touchedObject.name} {layerName}");
            }
            else
            {
                mCurrentInputType.Value = -1;
            }
        }
        
        private bool IsPointerOverUIObject()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(eventSystem);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            uiRaycaster.Raycast(eventDataCurrentPosition, results);
            return results.Count > 0;
        }
        
    }
