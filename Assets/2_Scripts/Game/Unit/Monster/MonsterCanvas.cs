using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterCanvas : MonoBehaviour
{
    [SerializeField]
    private Canvas mHpCanvas;
    [SerializeField]
    private Slider mHpSlider;
    [SerializeField]
    private Image mHpColor;
    [SerializeField]
    private TextMeshProUGUI mHpText;

    private float mMaxHp;

    private void Awake()
    {
        mHpCanvas.worldCamera = Camera.main;
    }

    public void InitHpSlider(float maxHp, bool isBoss)
    {
        mHpSlider.maxValue = maxHp;
        mHpSlider.value = maxHp;
        mMaxHp = maxHp;
        mHpColor.color = Color.green;
        if (isBoss)
            mHpText.text = $"{mMaxHp} / {mMaxHp}";
        else
            mHpText.gameObject.SetActive(false);
    }

    public void SetHpSlider(float currentHp)
    {
        mHpSlider.value = currentHp;

        mHpColor.color = Color.Lerp(Color.red, Color.green, currentHp / mMaxHp);
        mHpText.text = $"{Mathf.Max(currentHp, 0)} / {mMaxHp}";
    }
}
