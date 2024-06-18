using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{

    protected static T instance;
    public bool IsDontDestroy = true;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {

                instance = FindObjectOfType<T>();
                if (instance == null)
                {

                    GameObject go = GameObject.Find(typeof(T).ToString());
                    if (go == null)
                        go = new GameObject { name = typeof(T).ToString() };

                    instance = Utils.GetOrAddComponent<T>(go);
                }
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {

        if (instance == null)
        {
            instance = this as T;
            if (IsDontDestroy)
                DontDestroyOnLoad(gameObject);
            else
            {
                Destroy(gameObject);
            }
        }
    }
    protected virtual void OnDestory()
    {
        instance = null;
        Destroy(this.gameObject);
    }
}