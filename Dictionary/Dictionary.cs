using System;
using System.Collections;
using System.Collections.Generic;

namespace Dictionary
{
    public class HashtableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly int[] buckets;
        private readonly LinkedList<int> chainOfElements = new LinkedList<int>();
        private Element[] elements;

        public HashtableDictionary(int bucketSize = 5, int elementSize = 5)
        {
            buckets = new int[bucketSize];
            Array.Fill(buckets, -1);

            chainOfElements.AddFirst(-1);
            elements = new Element[elementSize];
            Count = 0;
        }

        public ICollection<TKey> Keys
        {
            get
            {
                var list = new List<TKey>();
                foreach (var element in this)
                {
                    list.Add(element.Key);
                }

                return list;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                var list = new List<TValue>();
                foreach (var element in this)
                {
                    list.Add(element.Value);
                }

                return list;
            }
        }

        public int Count { get; private set; }

        public bool IsReadOnly { get; private set; }

        public TValue this[TKey key]
        {
            get
            {
                ThrowArgumentIsNull(key);
                if (FindElement(key) == -1)
                {
                    throw new KeyNotFoundException($"Key {key} was not found! ");
                }

                return elements[FindElement(key)].Value;
            }

            set
            {
                ThrowReadOnly();
                AddToLinkedList(key);

                if (FindElement(key) == -1)
                {
                    throw new KeyNotFoundException($"Key {key} was not found! ");
                }

                elements[FindElement(key)].Value = value;
            }
        }

        public void Add(TKey key, TValue value)
        {
            ThrowAddExceptions(key);
            AddToLinkedList(key);
            EnsureCapacity();

            elements[Count].Update(key, value);

            int bucketIndex = GetFirstBucketPosition(key);
            if (buckets[bucketIndex] != -1)
            {
                elements[Count].Next = buckets[bucketIndex];
            }

            buckets[bucketIndex] = Count;
            Count++;
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            ThrowReadOnly();
            Array.Fill(buckets, -1);
            Count = 0;
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return FindElement(item.Key, item.Value) != -1;
        }

        public bool ContainsKey(TKey key)
        {
            ThrowArgumentIsNull(key);

            return FindElement(key) != -1;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ThrowCopyToExceptions(array, arrayIndex);

            var enumerator = GetEnumerator();
            for (int i = arrayIndex; i < Count + arrayIndex; i++)
            {
                enumerator.MoveNext();
                array[i] = enumerator.Current;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return CreateKeyValuePair(elements[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Remove(TKey key)
        {
            ThrowArgumentIsNull(key);
            ThrowReadOnly();

            int index = FindElement(key, out int previous);
            if (index == -1)
            {
                return false;
            }

            if (previous != -1)
            {
                elements[previous].Next = elements[index].Next;
            }
            else
            {
                buckets[GetFirstBucketPosition(key)] = elements[index].Next;
                chainOfElements.AddFirst(index);
            }

            Count--;
            return true;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (FindElement(item.Key, item.Value) != FindElement(item.Key))
            {
                return false;
            }

            return Remove(item.Key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            ThrowArgumentIsNull(key);

            if (FindElement(key) == -1)
            {
                value = default;
                return false;
            }

            value = elements[FindElement(key)].Value;
            return true;
        }

        public HashtableDictionary<TKey, TValue> AsReadOnly()
        {
            var newDictionary = new HashtableDictionary<TKey, TValue>();
            var enumerator = GetEnumerator();
            while (enumerator.MoveNext())
            {
                newDictionary.Add(enumerator.Current);
            }

            newDictionary.IsReadOnly = true;
            return newDictionary;
        }

        private int GetFirstBucketPosition(TKey key)
        {
            return Math.Abs(key.GetHashCode()) % buckets.Length;
        }

        private void ThrowAddExceptions(TKey key)
        {
            ThrowReadOnly();
            ThrowArgumentIsNull(key);
            ThrowArgument(key);
        }

        private void EnsureCapacity()
        {
            if (Count != elements.Length)
            {
                return;
            }

            ResizeArray();
        }

        private void ResizeArray()
        {
            int doubleLength = elements.Length * 2;
            var newElements = new Element[elements.Length];
            Array.Resize(ref elements, doubleLength);
            MoveElements(newElements);
        }

        private void MoveElements(Element[] newElements)
        {
            for (int i = 0; i < Count; i++)
            {
                newElements[i] = elements[i];
            }
        }

        private void ThrowReadOnly()
        {
            if (!IsReadOnly)
            {
                return;
            }

            throw new NotSupportedException("List is readonly!\n");
        }

        private void ThrowArgument(TKey key)
        {
            if (!ContainsKey(key))
            {
                return;
            }

            throw new ArgumentException($"Key {key} already exists in dictionary!");
        }

        private void ThrowArgumentIsNull(TKey key)
        {
            if (key != null)
            {
                return;
            }

            throw new ArgumentNullException(paramName: nameof(key));
        }

        private void ThrowCopyToExceptions(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ThrowArrayIsNull(array);
            ThrowIndexException(arrayIndex);
            ThrowArgumentException(array, arrayIndex);
        }

        private void ThrowArgumentException(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array.Length >= Count + arrayIndex)
            {
                return;
            }

            throw new ArgumentException(message: "Copying proccess cannot be initialised\n", paramName: nameof(array));
        }

        private void ThrowIndexException(int arrayIndex)
        {
            if (arrayIndex >= 0)
            {
                return;
            }

            throw new ArgumentOutOfRangeException(paramName: nameof(arrayIndex), message: "Give a valid index!\n");
        }

        private void ThrowArrayIsNull(KeyValuePair<TKey, TValue>[] array)
        {
            if (array != null)
            {
                return;
            }

            throw new ArgumentNullException(paramName: nameof(array));
        }

        private KeyValuePair<TKey, TValue> CreateKeyValuePair(Element element)
        {
            return KeyValuePair.Create(element.Key, element.Value);
        }

        private bool CheckForKey(Element element, TKey key)
        {
            return element.Key.Equals(key);
        }

        private int FindElement(TKey key)
        {
            return FindElement(key, out int previous);
        }

        private int FindElement(TKey key, out int previousIndex)
        {
            previousIndex = -1;
            for (int i = buckets[GetFirstBucketPosition(key)]; i != -1; i = elements[i].Next)
            {
                if (elements[i].Key.Equals(key))
                {
                    return i;
                }

                previousIndex = i;
            }

            return -1;
        }

        private int FindElement(TKey key, TValue value)
        {
            int index = FindElement(key);

            return index != -1 && elements[index].Value.Equals(value)
                ? index
                : -1;
        }

        private void AddToLinkedList(TKey key)
        {
            if (chainOfElements.First.Value == -1)
            {
                return;
            }

            int bucketIndex = GetFirstBucketPosition(key);
            chainOfElements.AddFirst(bucketIndex);
            buckets[bucketIndex] = chainOfElements.First.Value;
            Count++;
        }

        private struct Element
        {
            public TKey Key { get; set; }

            public TValue Value { get; set; }

            public int Next { get; set; }

            public void Update(TKey key, TValue value, int next = -1)
            {
                Key = key;
                Value = value;
                Next = next;
            }
        }
    }
}
