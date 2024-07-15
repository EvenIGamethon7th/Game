using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _2_Scripts.Demo
{
    public class DemoSkillSort : SerializedMonoBehaviour
    {
        [SerializeField]
        List<GameObject> go = new List<GameObject>();

        private void Start()
        {
            int columns = 5; // 가로로 배치할 개수
            int rows = Mathf.CeilToInt((float)go.Count / columns); // 필요한 행의 수 계산

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    int index = i * columns + j;
                    if (index < go.Count)
                    {
                        Instantiate(go[index], new Vector3(j * 10, i * 10, 0), Quaternion.identity, transform);
                    }
                }
            }
        }
    }
}