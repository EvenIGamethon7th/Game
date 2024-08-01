using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.Game.Dialog
{
    public class UI_ChoiceButton : Button
    {
        [SerializeField]
        private TextMeshProUGUI mDescription;

        protected override void Awake()
        {
            base.Awake();
            mDescription = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void Init(string description)
        {
            mDescription.text = description;
        }
    }
}