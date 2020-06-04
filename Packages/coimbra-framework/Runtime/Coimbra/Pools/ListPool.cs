using JetBrains.Annotations;
using System.Collections.Generic;

namespace Coimbra
{
    /// <summary>
    /// A ready to use <see cref="List{T}"/> pool.
    /// </summary>
    [PublicAPI]
    public static class ListPool<T>
    {
        private static readonly ClassPool<List<T>> Pool = new ClassPool<List<T>>(HandleCreate, null, HandleRelease);

        public static FakeDisposable<List<T>> GetDisposable()
        {
            return Pool.GetDisposable();
        }

        public static List<T> Get()
        {
            return Pool.Get();
        }

        public static void Release(ref List<T> item)
        {
            Pool.Release(ref item);
        }

        public static void Reset(int? preloadCount = null, int? maxCapacity = null)
        {
            Pool.Reset(preloadCount, maxCapacity);
        }

        private static List<T> HandleCreate()
        {
            return new List<T>();
        }

        private static void HandleRelease(List<T> item)
        {
            item.Clear();
        }
    }
}
