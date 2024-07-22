using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_AcademyInfo : MonoBehaviour
{
    [SerializeField]
    private GameObject mBubble;
    [SerializeField]
    private TextMeshProUGUI[] mInfoText;

    void Awake()
    {
        mBubble = transform.GetChild(0).gameObject;
        mBubble.TryGetComponent(out mInfoText);
    }

    //public void SetText()
    private void OnClick()
    {
        mBubble.SetActive(!mBubble.activeSelf);

    }
}
