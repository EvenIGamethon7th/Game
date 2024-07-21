﻿using System;
using System.Collections.Generic;
using _2_Scripts.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI
{
    public class UI_IngameController : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> mBottomGo;

        [SerializeField] private List<Button> mBottomButtons;

        private void Start()
        {
            MessageBroker.Default.Receive<TaskMessage>()
                .Where(message => message.Task == ETaskList.CharacterDataResourceLoad).Subscribe(
                    _ =>
                    {
                        if (mBottomGo[0].TryGetComponent<UI_AcademyPannel>(out var academi)) 
                        { 
                            academi.Init(); 
                        }
                        mBottomGo[1].SetActive(true);
                    }).AddTo(this);

            for (int i = 0; i <  mBottomGo.Count; ++i)
            {
                int idx = i;
                mBottomButtons[idx].onClick.AddListener(() => SetActive(idx));
            }
        }

        private void SetActive(int index)
        {
            for (int i = 0; i < mBottomGo.Count; ++i)
            {
                mBottomGo[i].SetActive(false);
            }
            mBottomGo[index].SetActive(true);
        }
    }
}