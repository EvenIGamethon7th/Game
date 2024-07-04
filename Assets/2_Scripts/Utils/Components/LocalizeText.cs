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
    private TextMeshProUGUI _text;
    
    public void SetLocalizeKey(string key)
    {
        _localizeKey = new LocalizeKey(key);
        this._previewText = LocalizeSystem_Manager.Instance.GetLcz_Func(this._localizeKey);
        _text = GetComponent<TextMeshProUGUI>(); // 임시 코드 나중에 지워야 함.
        _text.text = _previewText;
    }
    
    private void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
        UserSystem_Manager.Instance.common.Subscribe_LangTypeChanged_Func(CallLocalizeText);
    }

    private void CallLocalizeText(SystemLanguage language)
    {
        this._previewText = LocalizeSystem_Manager.Instance.GetLcz_Func(this._localizeKey);
        _text.text = _previewText;
    }
}
