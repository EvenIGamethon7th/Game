using _2_Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using CharacterInfo = _2_Scripts.Game.ScriptableObject.Character.CharacterInfo;

namespace _2_Scripts.UI
{
    public class UI_EditUnitPanel : MonoBehaviour
    {
        private List<CharacterInfo> mUserCharacterList = new List<CharacterInfo>();
        private bool isInitList = false;
        private bool isInitButton = false;

        [SerializeField]
        private RectTransform mRectTransform;
        private RectTransform mPanelTransform;
        private Vector3 mOriginPos;
        private Vector3 mClosePos;

        [SerializeField]
        private Button mButton;
        private TextMeshProUGUI mText;

        [SerializeField]
        private TMP_InputField mInputField;

        private List<UI_UnitButton> mButtons = new List<UI_UnitButton>();

        void Start()
        {
            mPanelTransform = GetComponent<RectTransform>();
            mOriginPos = mPanelTransform.anchoredPosition3D;
            mClosePos = -mOriginPos;
            mText = mButton.GetComponentInChildren<TextMeshProUGUI>();
            mButton.onClick.AddListener(ChangePos);
            MessageBroker.Default.Receive<TaskMessage>()
                .Where(message => message.Task == ETaskList.CharacterDataResourceLoad).Subscribe(
                    _ =>
                    {
                        foreach (var resource in ResourceManager.Instance._resources.Where(x => x.Value is CharacterInfo))
                        {
                            mUserCharacterList.Add(resource.Value as CharacterInfo);
                        }
                        isInitList = true;
                    }).AddTo(this);

            MessageBroker.Default.Receive<TaskMessage>()
                .Where(message => message.Task == ETaskList.DefaultResourceLoad).Subscribe(
                    _ =>
                    {
                        isInitButton = true;
                    }).AddTo(this);

            IDisposable dispose = null;

            dispose = this.UpdateAsObservable().Where(_ => isInitButton && isInitList).Subscribe(
                _ =>
                {
                    foreach (var character in mUserCharacterList)
                    {
                        foreach (var data in character.CharacterEvolutions)
                        {
                            var btn = ObjectPoolManager.Instance.CreatePoolingObject(AddressableTable.Default_EditUnitButton, Vector2.zero).GetComponent<UI_UnitButton>();
                            mButtons.Add(btn);
                            var rt = btn.GetComponent<RectTransform>();
                            rt.parent = mRectTransform;
                            rt.localScale = Vector3.one;
                            rt.localPosition = Vector3.zero;

                            btn.UpdateGraphic(data.Value.GetData);
                        }
                    }
                    
                    dispose.Dispose();
                }).AddTo(this);

            this.ObserveEveryValueChanged(_ => mInputField.text).Subscribe(_ => CheckInput());
        }

        private void ChangePos()
        {
            if (mPanelTransform.anchoredPosition3D == mOriginPos)
            {
                mPanelTransform.anchoredPosition3D = mClosePos;
                mText.text = "<";
            }

            else
            {
                mPanelTransform.anchoredPosition3D = mOriginPos;
                mText.text = ">";
            }
        }

        private void CheckInput()
        {
            string txt = mInputField.text.TrimEnd();
            if (txt.Length < 1 || String.IsNullOrWhiteSpace(txt))
            {
                foreach (var btn in mButtons)
                {
                    btn.gameObject.SetActive(true);
                }
            }

            else
            {
                foreach (var btn in mButtons)
                {
                    if (!btn.CharacterName.Contains(txt, StringComparison.OrdinalIgnoreCase))
                        btn.gameObject.SetActive(false);
                    else btn.gameObject.SetActive(true);
                }
            }
        }
    }
}
