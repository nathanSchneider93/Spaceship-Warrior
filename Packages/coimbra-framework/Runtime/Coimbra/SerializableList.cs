using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Coimbra
{
    [PublicAPI, Serializable]
    public class SerializableList<T> : IList, IList<T>, IReadOnlyList<T>
    {
        [SerializeField] private List<T> _items;

        public T this[int index] { get => _items[index]; set => _items[index] = value; }

        public int Count => _items.Count;
        public int Length => _items.Count;
        public int Capacity { get => _items.Capacity; set => _items.Capacity = value; }

        object IList.this[int index] { get => ((IList)_items)[index]; set => ((IList)_items)[index] = value; }

        bool ICollection.IsSynchronized => ((ICollection)_items).IsSynchronized;
        bool IList.IsFixedSize => ((IList)_items).IsFixedSize;
        bool IList.IsReadOnly => ((IList)_items).IsReadOnly;
        bool ICollection<T>.IsReadOnly => ((ICollection<T>)_items).IsReadOnly;
        object ICollection.SyncRoot => ((ICollection)_items).SyncRoot;

        public SerializableList()
        {
            _items = new List<T>();
        }

        public SerializableList(int capacity)
        {
            _items = new List<T>(capacity);
        }

        public SerializableList(IEnumerable<T> collection)
        {
            _items = new List<T>(collection);
        }

        public static implicit operator List<T>(SerializableList<T> list)
        {
            return list._items;
        }

        public void Add(T item)
        {
            _items.Add(item);
        }

        public void AddRange(IEnumerable<T> collection)
        {
            _items.AddRange(collection);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public void CopyTo(T[] array)
        {
            _items.CopyTo(array);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            _items.CopyTo(index, array, arrayIndex, count);
        }

        public void ForEach(Action<T> action)
        {
            _items.ForEach(action);
        }

        public void Insert(int index, T item)
        {
            _items.Insert(index, item);
        }

        public void InsertRange(int index, IEnumerable<T> collection)
        {
            _items.InsertRange(index, collection);
        }

        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }

        public void RemoveRange(int index, int count)
        {
            _items.RemoveRange(index, count);
        }

        public void Reverse(int index, int count)
        {
            _items.Reverse(index, count);
        }

        public void Reverse()
        {
            _items.Reverse();
        }

        public void Sort()
        {
            _items.Sort();
        }

        public void Sort(IComparer<T> comparer)
        {
            _items.Sort(comparer);
        }

        public void Sort(int index, int count, IComparer<T> comparer)
        {
            _items.Sort(index, count, comparer);
        }

        public void Sort(Comparison<T> comparison)
        {
            _items.Sort(comparison);
        }

        public void TrimExcess()
        {
            _items.TrimExcess();
        }

        public bool Contains(T item)
        {
            return _items.Contains(item);
        }

        public bool Exists(Predicate<T> match)
        {
            return _items.Exists(match);
        }

        public bool Remove(T item)
        {
            return _items.Remove(item);
        }

        public bool TrueForAll(Predicate<T> match)
        {
            return _items.TrueForAll(match);
        }

        public int BinarySearch(T item)
        {
            return _items.BinarySearch(item);
        }

        public int BinarySearch(T item, IComparer<T> comparer)
        {
            return _items.BinarySearch(item, comparer);
        }

        public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
        {
            return _items.BinarySearch(index, count, item, comparer);
        }

        public int FindIndex(Predicate<T> match)
        {
            return _items.FindIndex(match);
        }

        public int FindIndex(int startIndex, Predicate<T> match)
        {
            return _items.FindIndex(startIndex, match);
        }

        public int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            return _items.FindIndex(startIndex, count, match);
        }

        public int FindLastIndex(Predicate<T> match)
        {
            return _items.FindLastIndex(match);
        }

        public int FindLastIndex(int startIndex, Predicate<T> match)
        {
            return _items.FindLastIndex(startIndex, match);
        }

        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            return _items.FindLastIndex(startIndex, count, match);
        }

        public int IndexOf(T item)
        {
            return _items.IndexOf(item);
        }

        public int IndexOf(T item, int index)
        {
            return _items.IndexOf(item, index);
        }

        public int IndexOf(T item, int index, int count)
        {
            return _items.IndexOf(item, index, count);
        }

        public int LastIndexOf(T item)
        {
            return _items.LastIndexOf(item);
        }

        public int LastIndexOf(T item, int index)
        {
            return _items.LastIndexOf(item, index);
        }

        public int LastIndexOf(T item, int index, int count)
        {
            return _items.LastIndexOf(item, index, count);
        }

        public int RemoveAll(Predicate<T> match)
        {
            return _items.RemoveAll(match);
        }

        public T Find(Predicate<T> match)
        {
            return _items.Find(match);
        }

        public T FindLast(Predicate<T> match)
        {
            return _items.FindLast(match);
        }

        public T[] ToArray()
        {
            return _items.ToArray();
        }

        public List<T> FindAll(Predicate<T> match)
        {
            return _items.FindAll(match);
        }

        public List<T> GetRange(int index, int count)
        {
            return _items.GetRange(index, count);
        }

        public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
        {
            return _items.ConvertAll(converter);
        }

        public ReadOnlyCollection<T> AsReadOnly()
        {
            return _items.AsReadOnly();
        }

        public List<T>.Enumerator GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)_items).CopyTo(array, index);
        }

        void IList.Insert(int index, object value)
        {
            ((IList)_items).Insert(index, value);
        }

        void IList.Remove(object value)
        {
            ((IList)_items).Remove(value);
        }

        bool IList.Contains(object value)
        {
            return ((IList)_items).Contains(value);
        }

        int IList.Add(object value)
        {
            return ((IList)_items).Add(value);
        }

        int IList.IndexOf(object value)
        {
            return ((IList)_items).IndexOf(value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
