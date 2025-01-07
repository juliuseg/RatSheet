using System;
using System.Collections.Generic;


public class MinHeap<T>
{
    private readonly List<T> heap = new List<T>();
    private readonly Comparison<T> comparer;

    public MinHeap(Comparison<T> comparison)
    {
        comparer = comparison;
    }

    public int Count => heap.Count;

    public void Enqueue(T item)
    {
        heap.Add(item);
        int currentIndex = heap.Count - 1;
        while (currentIndex > 0)
        {
            int parentIndex = (currentIndex - 1) / 2;
            if (comparer(heap[currentIndex], heap[parentIndex]) >= 0) break;

            // Swap
            (heap[currentIndex], heap[parentIndex]) = (heap[parentIndex], heap[currentIndex]);
            currentIndex = parentIndex;
        }
    }

    public T Dequeue()
    {
        if (heap.Count == 0) throw new InvalidOperationException("Heap is empty");
        T root = heap[0];
        heap[0] = heap[^1];
        heap.RemoveAt(heap.Count - 1);

        int currentIndex = 0;
        while (true)
        {
            int leftChild = currentIndex * 2 + 1;
            int rightChild = currentIndex * 2 + 2;
            int smallest = currentIndex;

            if (leftChild < heap.Count && comparer(heap[leftChild], heap[smallest]) < 0)
                smallest = leftChild;

            if (rightChild < heap.Count && comparer(heap[rightChild], heap[smallest]) < 0)
                smallest = rightChild;

            if (smallest == currentIndex) break;

            // Swap
            (heap[currentIndex], heap[smallest]) = (heap[smallest], heap[currentIndex]);
            currentIndex = smallest;
        }

        return root;
    }
}
