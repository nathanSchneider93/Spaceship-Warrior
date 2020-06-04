using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coimbra
{
    [Serializable]
    public abstract class ClassPoolBase<T> where T : class
    {
        private readonly object Lock = new object();
        private readonly FakeDisposable<T>.DisposeHandler DisposeHandler;
        private readonly HashSet<T> AvailableSet = new HashSet<T>();
        private readonly Stack<T> AvailableStack = new Stack<T>();

        [SerializeField, Positive] private int m_PreloadCount;
        [SerializeField, Positive] private int m_MaxCapacity;

        /// <summary>
        /// Amount of items available from the beginning.
        /// </summary>
        public int PreloadCount
        {
            get => m_PreloadCount;
            set => m_PreloadCount = Mathf.Max(value, 0);
        }
        /// <summary>
        /// Max amount of items in the pool.
        /// </summary>
        public int MaxCapacity
        {
            get => m_MaxCapacity;
            set => m_MaxCapacity = Mathf.Max(value, 0);
        }

        protected ClassPoolBase()
        {
            DisposeHandler = Release;
        }

        protected ClassPoolBase(int preloadCount, int maxCapacity) : this()
        {
            m_PreloadCount = Mathf.Max(preloadCount, 0);
            m_MaxCapacity = Mathf.Max(maxCapacity, 0);
        }

        /// <summary>
        /// Pick one item from the pool.
        /// </summary>
        public T Get()
        {
            T item = null;

            lock (Lock)
            {
                if (AvailableStack.Count > 0)
                {
                    item = AvailableStack.Pop();
                    AvailableSet.Remove(item);
                }
            }

            if (item == null)
            {
                item = Create();
            }

            OnGet(item);

            return item;
        }

        /// <summary>
        /// Pick one item from the pool using a <see cref="FakeDisposable{T}"/>.
        /// </summary>
        public FakeDisposable<T> GetDisposable()
        {
            return new FakeDisposable<T>(Get(), DisposeHandler);
        }

        /// <summary>
        /// Return the item to the pool.
        /// </summary>
        public void Release(ref T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            bool release = false;

            lock (Lock)
            {
                if (AvailableSet.Contains(item))
                {
                    return;
                }

                if (MaxCapacity == 0 || AvailableStack.Count < MaxCapacity)
                {
                    AvailableStack.Push(item);
                    AvailableSet.Add(item);
                    release = true;
                }
            }

            if (release)
            {
                OnRelease(item);
            }
            else
            {
                OnDelete(item);
            }

            item = null;
        }

        /// <summary>
        /// Reset the pool to its initial state.
        /// </summary>
        /// <param name="preloadCount">If not null. it will override the current <see cref="PreloadCount"/>.</param>
        /// <param name="maxCapacity">If not null, it will override the current <see cref="MaxCapacity"/>.</param>
        public void Reset(int? preloadCount = null, int? maxCapacity = null)
        {
            if (preloadCount.HasValue)
            {
                PreloadCount = preloadCount.Value;
            }

            if (maxCapacity.HasValue)
            {
                MaxCapacity = maxCapacity.Value;
            }

            int desiredCount = MaxCapacity > 0 ? Mathf.Min(PreloadCount, MaxCapacity) : PreloadCount;
            bool preload = false;

            lock (Lock)
            {
                if (AvailableStack.Count < desiredCount)
                {
                    preload = true;
                }
                else
                {
                    while (AvailableStack.Count > desiredCount)
                    {
                        Delete();
                    }
                }
            }

            if (preload)
            {
                Preload(desiredCount);
            }
        }

        /// <summary>
        /// Called when creating a new item for the pool. It should never return null.
        /// </summary>
        protected abstract T Create();

        /// <summary>
        /// Called when deleting an item due the pool being full.
        /// </summary>
        protected abstract void OnDelete(T item);

        /// <summary>
        /// Called when picking an item from the pool.
        /// </summary>
        protected abstract void OnGet(T item);

        /// <summary>
        /// Called when returning an item from the pool.
        /// </summary>
        protected abstract void OnRelease(T item);

        /// <summary>
        /// Fill the pool with items.
        /// </summary>
        /// <param name="desiredCount">If null, it will pick the pool's <see cref="PreloadCount"/>.</param>
        protected void Preload(int? desiredCount = null)
        {
            if (desiredCount.HasValue == false)
            {
                desiredCount = MaxCapacity > 0 ? Mathf.Min(PreloadCount, MaxCapacity) : PreloadCount;
            }

            lock (Lock)
            {
                while (AvailableStack.Count < desiredCount)
                {
                    Add();
                }
            }
        }

        private void Add()
        {
            T item = Create();
            AvailableSet.Add(item);
            AvailableStack.Push(item);
        }

        private void Delete()
        {
            T item = AvailableStack.Pop();
            AvailableSet.Remove(item);
            OnDelete(item);
        }
    }
}
