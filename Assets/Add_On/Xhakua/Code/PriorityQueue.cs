using System;
using System.Collections;
using System.Collections.Generic;

public class PriorityQueue<T> : IEnumerable<T>
{
    private class HeapItem
    {
        public T Element;
        public float Priority;
        public HeapItem(T element, float priority)
        {
            Element = element;
            Priority = priority;
        }
    }

    private List<HeapItem> _heap = new List<HeapItem>();
    private Dictionary<T, int> _elementIndices = new Dictionary<T, int>();
    private readonly Comparison<float> _comparer;

    public int Count => _heap.Count;

    public PriorityQueue(bool isMinHeap = true)
    {
        _comparer = isMinHeap
            ? (a, b) => a.CompareTo(b)
            : (a, b) => b.CompareTo(a);
    }

    public PriorityQueue(Comparison<float> customComparer)
    {
        _comparer = customComparer ?? throw new ArgumentNullException(nameof(customComparer));
    }

    public void Enqueue(T element, float priority)
    {
        if (priority == 0)
            return;
        if (_elementIndices.TryGetValue(element, out int existingIndex))
        {
            // 元素已存在，更新优先级
            UpdatePriority(existingIndex, priority);
        }
        else
        {
            // 添加新元素
            _heap.Add(new HeapItem(element, priority));
            int newIndex = _heap.Count - 1;
            _elementIndices[element] = newIndex;
            Swim(newIndex);
        }
    }

    public T Dequeue()
    {
        if (_heap.Count == 0) throw new InvalidOperationException("Queue is empty");

        var top = _heap[0];
        Remove(top.Element);
        return top.Element;
    }

    public bool Contains(T element) => _elementIndices.ContainsKey(element);

    private void UpdatePriority(int index, float newPriority)
    {
        float oldPriority = _heap[index].Priority;
        _heap[index].Priority = newPriority;

        // 根据优先级变化方向调整堆结构
        if (_comparer(newPriority, oldPriority) < 0)
            Swim(index);
        else
            Sink(index);
    }

    public void Clear()
    {
        _heap.Clear();
        _elementIndices.Clear();
    }
    public void Remove(T element)
    {
        if (!_elementIndices.TryGetValue(element, out int index))
            return;

        int lastIndex = _heap.Count - 1;
        if (index != lastIndex)
        {
            Swap(index, lastIndex);
            _heap.RemoveAt(lastIndex);
            _elementIndices.Remove(element);
            Swim(index);
            Sink(index);
        }
        else
        {
            _heap.RemoveAt(lastIndex);
            _elementIndices.Remove(element);
        }
    }

    private void Swim(int index)
    {
        while (index > 0)
        {
            int parent = (index - 1) / 2;
            if (_comparer(_heap[index].Priority, _heap[parent].Priority) >= 0)
                break;

            Swap(index, parent);
            index = parent;
        }
    }

    private void Sink(int index)
    {
        int last = _heap.Count - 1;
        while (true)
        {
            int left = 2 * index + 1;
            int right = 2 * index + 2;
            int target = index;

            if (left <= last && _comparer(_heap[left].Priority, _heap[target].Priority) < 0)
                target = left;
            if (right <= last && _comparer(_heap[right].Priority, _heap[target].Priority) < 0)
                target = right;

            if (target == index) break;

            Swap(index, target);
            index = target;
        }
    }

    private void Swap(int i, int j)
    {
        (_heap[i], _heap[j]) = (_heap[j], _heap[i]);
        _elementIndices[_heap[i].Element] = i;
        _elementIndices[_heap[j].Element] = j;
    }

    public IEnumerator<T> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }

    // 其他方法保持不变（如 Peek, Clear, GetEnumerator 等）
}