using JetBrains.Annotations;
using System;

namespace Coimbra
{
    [PublicAPI]
    public struct FakeDisposable<T> : IDisposable
    {
        public delegate void DisposeHandler(ref T value);

        private T _value;
        private DisposeHandler _onDispose;

        public T Value => _value;

        public FakeDisposable(T value, DisposeHandler onDispose)
        {
            _value = value;
            _onDispose = onDispose;
        }

        public void Dispose()
        {
            _onDispose?.Invoke(ref _value);
        }
    }
}
