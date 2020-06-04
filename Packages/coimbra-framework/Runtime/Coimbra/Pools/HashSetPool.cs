using JetBrains.Annotations;
using System.Collections.Generic;

namespace Coimbra
{
    /// <summary>
    /// A ready to use <see cref="HashSet{T}"/> pool.
    /// </summary>
    [PublicAPI]
    public static class HashSetPool<T>
    {
        private static readonly ClassPool<HashSet<T>> Pool = new ClassPool<HashSet<T>>(HandleCreate, null, HandleRelease);

        public static FakeDisposable<HashSet<T>> GetDisposable()
        {
            return Pool.GetDisposable();
        }

        public static HashSet<T> Get()
        {
            return Pool.Get();
        }

        public static void Release(ref HashSet<T> item)
        {
            Pool.Release(ref item);
        }

        public static void Reset(int? preloadCount = null, int? maxCapacity = null)
        {
            Pool.Reset(preloadCount, maxCapacity);
        }

        private static HashSet<T> HandleCreate()
        {
            return new HashSet<T>();
        }

        private static void HandleRelease(HashSet<T> item)
        {
            item.Clear();
        }
    }
}
