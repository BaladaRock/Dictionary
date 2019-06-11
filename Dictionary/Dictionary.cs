using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Dictionary
{
    public class HashtableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly int[] buckets;
        private readonly Element[] elements;

        public HashtableDictionary(int bucketSize = 5, int elementSize = 5)
        {
            buckets = new int[bucketSize];
            Array.Fill(buckets, -1);

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

        public bool IsReadOnly { get; }

        public TValue this[TKey key]
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public void Add(TKey key, TValue value)
        {
            ThrowAddExceptions(key);
            AddElement(ref elements[Count], key, value);

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
            Count = 0;
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(TKey key)
        {
            ThrowArgumentIsNull(key);
            for (int temp = buckets[GetFirstBucketPosition(key)]; temp != -1; temp = elements[temp].Next)
            {
                if (elements[temp].Key.Equals(key))
                {
                    return true;
                }
            }

            return false;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return GetBuckets(elements[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Remove(TKey key)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            throw new NotImplementedException();
        }

        private int GetFirstBucketPosition(TKey key)
        {
            return Math.Abs(key.GetHashCode()) % buckets.Length;
        }

        private void AddElement(ref Element element, TKey key, TValue value)
        {
            element.Key = key;
            element.Value = value;
            element.Next = -1;
        }

        private void ThrowAddExceptions(TKey key)
        {
            ThrowArgumentIsNull(key);
            ThrowArgument(key);
            ThrowReadOnly();
        }

        private void ThrowReadOnly()
        {
            if (!IsReadOnly)
            {
                return;
            }

            throw new NotSupportedException($"List {this} is readonly! ");
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

            throw new ArgumentNullException(paramName: $"{key}");
        }

        private KeyValuePair<TKey, TValue> GetBuckets(Element element)
        {
            return KeyValuePair.Create(element.Key, element.Value);
        }

        private bool CheckForKey(Element element, TKey key)
        {
            return element.Key.Equals(key);
        }

        private struct Element
        {
            public TKey Key { get; set; }

            public TValue Value { get; set; }

            public int Next { get; set; }
        }
    }
}
