using System;
using System.Collections.Generic;
using UnityEngine;

namespace _2_Scripts.Utils.Structure
{
    [Serializable]
    public class UniqueQueue<T>
    {
        [SerializeField]
        private Queue<T> queue = new Queue<T>();
        [SerializeField]
        private HashSet<T> set = new HashSet<T>();

        public void Enqueue(T item)
        {
            if (set.Add(item))
            {
                queue.Enqueue(item);
            }
        }

        public T Dequeue()
        {
            if (queue.Count == 0)
                throw new InvalidOperationException("The queue is empty.");

            T item = queue.Dequeue();
            set.Remove(item);
            return item;
        }

        public T Peek()
        {
            if (queue.Count == 0)
                throw new InvalidOperationException("The queue is empty.");

            return queue.Peek();
        }

        public void Clear()
        {
            queue.Clear();
            set.Clear();
        }

        public int Count => queue.Count;

        public bool IsEmpty => queue.Count == 0;
    }
}