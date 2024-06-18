﻿using System;
using UnityEngine;

public class Utils
{

    private const string UNITY_AUTOGENERATED_CLONE_NAME = "(clone) ";
    public static T ParseEnum<T>(string value, bool ignoreCase = true)
    {
        return (T)Enum.Parse(typeof(T), value, ignoreCase);
    }

    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }
    
    public static string EraseUnityAutoGeneratedCloneName(string name)
    {
        int index = name.IndexOf(UNITY_AUTOGENERATED_CLONE_NAME);
        if (index > 0)
            name = name.Substring(0, index);
        return name;
            
    }
}