using System.Collections.Generic;

public class PriorityQueue<TElement>
{
    private SortedDictionary<int, Queue<TElement>> dictionary = new SortedDictionary<int, Queue<TElement>>();

    public void Enqueue(TElement element, int priority)
    {
        if (!dictionary.TryGetValue(priority, out var queue))
        {
            queue = new Queue<TElement>();
            dictionary.Add(priority, queue);
        }
        queue.Enqueue(element);
    }

    public TElement Dequeue()
    {
        if (dictionary.Count == 0)
            throw new System.InvalidOperationException("The priority queue is empty.");

        // Get the first entry manually without using LINQ
        foreach (var firstEntry in dictionary)
        {
            var element = firstEntry.Value.Dequeue();
            
            // If the queue for this priority is empty, remove it from the dictionary
            if (firstEntry.Value.Count == 0)
            {
                dictionary.Remove(firstEntry.Key);
            }
            
            return element;
        }

        throw new System.InvalidOperationException("The priority queue is empty.");
    }

    public int Count
    {
        get
        {
            int count = 0;
            foreach (var entry in dictionary)
            {
                count += entry.Value.Count;
            }
            return count;
        }
    }
}