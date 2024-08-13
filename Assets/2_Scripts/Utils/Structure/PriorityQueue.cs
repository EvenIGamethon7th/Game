using System;
using System.Collections.Generic;

namespace _2_Scripts.Utils.Structure
{
    public class PriorityQueue<T> where T : IComparable<T>
    {
        private SortedSet<(T item, int order)> queue = new SortedSet<(T item, int order)>();
        private int order = 0;

        public void Enqueue(T item)
        {
            queue.Add((item, order++));
        }

        public T Dequeue()
        {
            if (queue.Count == 0)
                throw new InvalidOperationException("The queue is empty.");
        
            var firstItem = queue.Min;
            queue.Remove(firstItem);
            return firstItem.item;
        }

        public T Peek()
        {
            if (queue.Count == 0)
                throw new InvalidOperationException("The queue is empty.");
        
            return queue.Min.item;
        }

        public int Count => queue.Count;
    }
}