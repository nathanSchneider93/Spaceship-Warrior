using JetBrains.Annotations;
using System.Collections.Generic;

namespace Coimbra
{
    /// <summary>
    /// A ready to use <see cref="Dictionary{TKey,TValue}"/> pool.
    /// </summary>
    [PublicAPI]
    public static class DictionaryPool<TKey, TValue>
    {
        private static readonly ClassPool<Dictionary<TKey, TValue>> Pool = new ClassPool<Dictionary<TKey, TValue>>(HandleCreate, null, HandleRelease);

        public static FakeDisposable<Dictionary<TKey, TValue>> GetDisposable()
        {
            return Pool.GetDisposable();
        }

        public static Dictionary<TKey, TValue> Get()
        {
            return Pool.Get();
        }

        public static void Release(ref Dictionary<TKey, TValue> item)
        {
            Pool.Release(ref item);
        }

        public static void Reset(int? preloadCount = null, int? maxCapacity = null)
        {
            Pool.Reset(preloadCount, maxCapacity);
        }

        private static Dictionary<TKey, TValue> HandleCreate()
        {
            return new Dictionary<TKey, TValue>();
        }

        private static void HandleRelease(Dictionary<TKey, TValue> item)
        {
            item.Clear();
        }
    }
}
