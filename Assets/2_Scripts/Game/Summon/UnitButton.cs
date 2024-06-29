using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.Game.Summon
{
    public class UnitButton : Button
    {
        public TextMeshProUGUI Text { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            Text = GetComponentInChildren<TextMeshProUGUI>();
        }
    }
}