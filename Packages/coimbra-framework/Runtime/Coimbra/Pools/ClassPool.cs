using JetBrains.Annotations;
using System;

namespace Coimbra
{
    /// <summary>
    /// Create an instance from that class to quickly start pooling your objects.
    /// </summary>
    [PublicAPI]
    public sealed class ClassPool<T> : ClassPoolBase<T> where T : class
    {
        private readonly Action<T> DeleteAction;
        private readonly Action<T> GetAction;
        private readonly Action<T> ReleaseAction;
        private readonly Func<T> CreateFunc;

        /// <param name="onCreate">Called when creating a new item for the pool. It should never return null.</param>
        /// <param name="onGet">Called when picking an item from the pool.</param>
        /// <param name="onRelease">Called when returning an item from the pool.</param>
        /// <param name="onDelete">Called when deleting an item due the pool being full.</param>
        public ClassPool(Func<T> onCreate, Action<T> onGet = null, Action<T> onRelease = null, Action<T> onDelete = null)
        {
            CreateFunc = onCreate ?? throw new ArgumentNullException(nameof(onCreate));
            GetAction = onGet;
            ReleaseAction = onRelease;
            DeleteAction = onDelete;
        }

        /// <param name="preloadCount">Amount of items available from the beginning.</param>
        /// <param name="maxCapacity">Max amount of items in the pool.</param>
        /// <param name="onCreate">Called when creating a new item for the pool. It should never return null.</param>
        /// <param name="onGet">Called when picking an item from the pool.</param>
        /// <param name="onRelease">Called when returning an item from the pool.</param>
        /// <param name="onDelete">Called when deleting an item due the pool being full.</param>
        public ClassPool(int preloadCount, int maxCapacity, Func<T> onCreate, Action<T> onGet = null, Action<T> onRelease = null, Action<T> onDelete = null)
            : base(preloadCount, maxCapacity)
        {
            CreateFunc = onCreate ?? throw new ArgumentNullException(nameof(onCreate));
            GetAction = onGet;
            ReleaseAction = onRelease;
            DeleteAction = onDelete;

            Preload();
        }

        protected override T Create()
        {
            return CreateFunc();
        }

        protected override void OnDelete(T item)
        {
            DeleteAction?.Invoke(item);
        }

        protected override void OnGet(T item)
        {
            GetAction?.Invoke(item);
        }

        protected override void OnRelease(T item)
        {
            ReleaseAction?.Invoke(item);
        }
    }
}
