using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeRunnerClient.Models
{
    public class PriorityQueue<T>
    {
        private List<T> list = new List<T>();

        public int Count => list.Count;

        public void Enqueue(T item)
        {
            list.Add(item);
            int i = list.Count - 1;
            while (i > 0)
            {
                int parent = (i - 1) / 2;
                if (Comparer<T>.Default.Compare(list[i], list[parent]) >= 0)
                    break;
                (list[i], list[parent]) = (list[parent], list[i]);
                i = parent;
            }
        }

        public T Dequeue()
        {
            if (list.Count == 0)
                throw new InvalidOperationException("Queue is empty");
            T front = list[0];
            int last = list.Count - 1;
            list[0] = list[last];
            list.RemoveAt(last);
            last--;
            int i = 0;
            while (true)
            {
                int leftChild = 2 * i + 1;
                if (leftChild > last)
                    break;
                int rightChild = leftChild + 1;
                int minChild = (rightChild <= last && Comparer<T>.Default.Compare(list[rightChild], list[leftChild]) < 0) ? rightChild : leftChild;
                if (Comparer<T>.Default.Compare(list[i], list[minChild]) <= 0)
                    break;
                (list[i], list[minChild]) = (list[minChild], list[i]);
                i = minChild;
            }
            return front;
        }
    }
}
