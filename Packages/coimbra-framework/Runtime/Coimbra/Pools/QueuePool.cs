using JetBrains.Annotations;
using System.Collections.Generic;

namespace Coimbra
{
    /// <summary>
    /// A ready to use <see cref="Queue{T}"/> pool.
    /// </summary>
    [PublicAPI]
    public static class QueuePool<T>
    {
        private static readonly ClassPool<Queue<T>> Pool = new ClassPool<Queue<T>>(HandleCreate, null, HandleRelease);

        public static FakeDisposable<Queue<T>> GetDisposable()
        {
            return Pool.GetDisposable();
        }

        public static Queue<T> Get()
        {
            return Pool.Get();
        }

        public static void Release(ref Queue<T> item)
        {
            Pool.Release(ref item);
        }

        public static void Reset(int? preloadCount = null, int? maxCapacity = null)
        {
            Pool.Reset(preloadCount, maxCapacity);
        }

        private static Queue<T> HandleCreate()
        {
            return new Queue<T>();
        }

        private static void HandleRelease(Queue<T> item)
        {
            item.Clear();
        }
    }
}
