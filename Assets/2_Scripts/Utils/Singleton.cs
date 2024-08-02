using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class Singleton<T> : MonoBehaviour where T : Component
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

    private void Awake()
    {

        if (instance == null)
        {
            instance = this as T;
            AwakeInit();
            if (IsDontDestroy)
                DontDestroyOnLoad(gameObject);
       
            SceneManager.activeSceneChanged -= ChangeSceneInit;
            SceneManager.activeSceneChanged += ChangeSceneInit;
        }

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    protected virtual void AwakeInit()
    {

    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    protected virtual void ChangeSceneInit(Scene prev, Scene next)
    {
    }
}