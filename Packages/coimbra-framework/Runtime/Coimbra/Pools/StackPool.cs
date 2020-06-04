using JetBrains.Annotations;
using System.Collections.Generic;

namespace Coimbra
{
    /// <summary>
    /// A ready to use <see cref="Stack{T}"/> pool.
    /// </summary>
    [PublicAPI]
    public static class StackPool<T>
    {
        private static readonly ClassPool<Stack<T>> Pool = new ClassPool<Stack<T>>(HandleCreate, null, HandleRelease);

        public static FakeDisposable<Stack<T>> GetDisposable()
        {
            return Pool.GetDisposable();
        }

        public static Stack<T> Get()
        {
            return Pool.Get();
        }

        public static void Release(ref Stack<T> item)
        {
            Pool.Release(ref item);
        }

        public static void Reset(int? preloadCount = null, int? maxCapacity = null)
        {
            Pool.Reset(preloadCount, maxCapacity);
        }

        private static Stack<T> HandleCreate()
        {
            return new Stack<T>();
        }

        private static void HandleRelease(Stack<T> item)
        {
            item.Clear();
        }
    }
}
