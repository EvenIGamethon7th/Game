using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_GradeText : MonoBehaviour
{
    private TextMeshProUGUI mGradeText;

    void Awake()
    {
        mGradeText = GetComponent<TextMeshProUGUI>();
    }

    public void SetGrade(int grade)
    {
        mGradeText.text = $"{grade}ÇÐ³â";
    }
}
