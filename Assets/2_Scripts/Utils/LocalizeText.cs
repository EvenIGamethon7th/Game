using System;
using System.Collections;
using System.Collections.Generic;
using Rito.Attributes;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class LocalizeText : MonoBehaviour
{

    [SerializeField] private LocalizeKey _localizeKey;

    [SerializeField,MultiLineProperty, ReadOnly] private string _previewText;

    [GetComponent] private TextMeshProUGUI _text;
    
    private void Start()
    {
        UserSystem_Manager.Instance.common.Subscribe_LangTypeChanged_Func(CallLocalizeText);
    }

    private void CallLocalizeText(SystemLanguage language)
    {
        this._previewText = LocalizeSystem_Manager.Instance.GetLcz_Func(this._localizeKey);
        _text.text = _previewText;
    }
}
