using JetBrains.Annotations;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Coimbra
{
    [PublicAPI, Serializable]
    public struct IntRange : IEquatable<IntRange>
    {
        [SerializeField] private int m_Min;
        [SerializeField] private int m_Max;

        /// <summary>
        /// The difference from min to max.
        /// </summary>
        public int Difference => Max - Min;
        public int Min => m_Min;
        public int Max => m_Max;
        /// <summary>
        /// Returns a random integer number between min [inclusive] and max [exclusive].
        /// </summary>
        public int RandomExclusive => Random.Range(Min, Max);
        /// <summary>
        /// Returns a random integer number between min [inclusive] and max [inclusive].
        /// </summary>
        public int RandomInclusive => Random.Range(Min, Max + 1);
        /// <summary>
        /// The sum of min and max.
        /// </summary>
        public int Sum => Min + Max;

        /// <param name="a"> The min or max value. </param>
        /// <param name="b"> The min or max value. </param>
        public IntRange(int a, int b)
        {
            m_Min = a < b ? a : b;
            m_Max = a > b ? a : b;
        }

        public static implicit operator FloatRange(IntRange value)
        {
            return new FloatRange(value.Min, value.Max);
        }

        public static implicit operator Vector2(IntRange value)
        {
            return new Vector2(value.Min, value.Max);
        }

        public static implicit operator Vector2Int(IntRange value)
        {
            return new Vector2Int(value.Min, value.Max);
        }

        public static implicit operator IntRange(Vector2Int value)
        {
            return new IntRange(value.x, value.y);
        }

        public static bool operator ==(IntRange a, IntRange b)
        {
            return a.Min == b.Min && a.Max == b.Max;
        }

        public static bool operator !=(IntRange a, IntRange b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (base.Equals(obj))
            {
                return true;
            }

            switch (obj)
            {
                case IntRange _:
                case Vector2Int _:

                    return this == (IntRange)obj;

                case FloatRange _:
                case Vector2 _:

                    return this == (FloatRange)obj;

                default:

                    return false;
            }
        }

        public override int GetHashCode()
        {
            return (Min.GetHashCode() + Max.GetHashCode()) * 37;
        }

        public override string ToString()
        {
            return $"[{Min}, {Max}]";
        }

        /// <summary>
        /// Returns true if the value is between min [inclusive] and max [exclusive].
        /// </summary>
        public bool ContainsExclusive(int value)
        {
            return value >= Min && value < Max;
        }

        /// <summary>
        /// Returns true if the value is between min [inclusive] and max [exclusive].
        /// </summary>
        public bool ContainsExclusive(float value)
        {
            return value >= Min && value < Max;
        }

        /// <summary>
        /// Returns true if the value is between min [inclusive] and max [inclusive].
        /// </summary>
        public bool ContainsInclusive(int value)
        {
            return value >= Min && value <= Max;
        }

        /// <summary>
        /// Returns true if the value is between min [inclusive] and max [inclusive].
        /// </summary>
        public bool ContainsInclusive(float value)
        {
            return value >= Min && value <= Max;
        }

        public bool Equals(IntRange other)
        {
            return this == other;
        }
    }
}
