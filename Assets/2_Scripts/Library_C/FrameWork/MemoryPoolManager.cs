using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MemoryPoolManager<T> where T : class, IPoolable, new ()
{
    private static Dictionary<Type, List<IPoolable>> mMemoryPool = new ();

    public static void ClearMemory()
    {
        foreach (var pool in mMemoryPool.Values)
        {
            pool.Clear();
        }
        mMemoryPool.Clear();
    }

    public static T CreatePoolingObject()
    {
        return GetObject();
    }

    private static T GetObject()
    {
        T newT = null;
        if (mMemoryPool.TryGetValue(typeof(T), out var list))
        {
            IPoolable obj = list.Where(x => !x.IsActive).FirstOrDefault();

            if (obj == null)
            {
                newT = new();
                list.Add(newT);
                obj = newT;
            }

            obj.IsActive = true;

            return obj as T;
        }

        else
        {
            List<IPoolable> Tlist = new List<IPoolable>();
            mMemoryPool.Add(typeof(T), Tlist);
            newT = new();
            newT.IsActive = true;
            Tlist.Add(newT);

            return newT;
        }
    }
}

public interface IPoolable
{
    public bool IsActive { get; set; }
    public void Clear();
}
