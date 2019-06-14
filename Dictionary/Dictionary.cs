using System;
using System.Collections;
using System.Collections.Generic;

namespace Dictionary
{
    public class HashtableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly int[] buckets;
        private readonly Element[] elements;
        private int freeIndex;

        public HashtableDictionary(int bucketSize = 5, int elementSize = 5)
        {
            buckets = new int[bucketSize];
            Array.Fill(buckets, -1);

            freeIndex = -1;
            elements = new Element[elementSize];
            Count = 0;
        }

        public int Count { get; private set; }

        public bool IsReadOnly { get; private set; }

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
                if (FindElement(key) == -1)
                {
                    Count++;
                }

                elements[FindElement(key)].Value = value;
            }
        }

        public void Add(TKey key, TValue value)
        {
            ThrowAddExceptions(key);

            int bucketIndex = GetFirstBucketPosition(key);
            int index = FindNewEmptyPosition();
            elements[index].Update(key, value);

            if (buckets[bucketIndex] != -1)
            {
                elements[index].Next = buckets[bucketIndex];
            }

            buckets[bucketIndex] = index;
            Count++;
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
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
            for (int i = 0; i < buckets.Length; i++)
            {
                for (int j = buckets[i]; j != -1; j = elements[j].Next)
                {
                    yield return CreateKeyValuePair(elements[j]);
                }
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
            }

            elements[index].Next = freeIndex;
            freeIndex = index;
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

        private int FindNewEmptyPosition()
        {
            if (freeIndex != -1)
            {
                var temp = freeIndex;
                freeIndex = elements[freeIndex].Next;
                return temp;
            }

            return Count;
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

        private void ThrowArgument(TKey key)
        {
            if (!ContainsKey(key))
            {
                return;
            }

            throw new ArgumentException($"Key {key} already exists in dictionary!");
        }

        private void ThrowArgumentException(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array.Length >= Count + arrayIndex)
            {
                return;
            }

            throw new ArgumentException(message: "Copying proccess cannot be initialised\n", paramName: nameof(array));
        }

        private void ThrowArgumentIsNull(TKey key)
        {
            if (key != null)
            {
                return;
            }

            throw new ArgumentNullException(paramName: nameof(key));
        }

        private void ThrowArrayIsNull(KeyValuePair<TKey, TValue>[] array)
        {
            if (array != null)
            {
                return;
            }

            throw new ArgumentNullException(paramName: nameof(array));
        }

        private void ThrowCopyToExceptions(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ThrowArrayIsNull(array);
            ThrowIndexException(arrayIndex);
            ThrowArgumentException(array, arrayIndex);
        }

        private void ThrowIndexException(int arrayIndex)
        {
            if (arrayIndex >= 0)
            {
                return;
            }

            throw new ArgumentOutOfRangeException(paramName: nameof(arrayIndex), message: "Give a valid index!\n");
        }

        private void ThrowReadOnly()
        {
            if (!IsReadOnly)
            {
                return;
            }

            throw new NotSupportedException("List is readonly!\n");
        }

        private struct Element
        {
            public TKey Key { get; set; }

            public int Next { get; set; }

            public TValue Value { get; set; }

            public void Update(TKey key, TValue value, int next = -1)
            {
                Key = key;
                Value = value;
                Next = next;
            }
        }
    }
}