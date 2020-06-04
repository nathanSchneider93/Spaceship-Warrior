using JetBrains.Annotations;

namespace Coimbra
{
    [PublicAPI]
    public class Reference<T> where T : struct
    {
        public T Value;

        public Reference() : this(default(T)) { }

        public Reference(T value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
