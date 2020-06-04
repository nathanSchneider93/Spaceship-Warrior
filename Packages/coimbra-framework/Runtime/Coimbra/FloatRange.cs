using JetBrains.Annotations;
using System;
using UnityEngine;

namespace Coimbra
{
    [PublicAPI, Serializable]
    public struct FloatRange : IEquatable<FloatRange>
    {
        [SerializeField] private float m_Min;
        [SerializeField] private float m_Max;

        /// <summary>
        /// The difference from min to max.
        /// </summary>
        public float Difference => Max - Min;
        public float Max => m_Max;
        public float Min => m_Min;
        /// <summary>
        /// Returns a random float number between min [inclusive] and max [inclusive].
        /// </summary>
        public float Random => UnityEngine.Random.Range(Min, Max);
        /// <summary>
        /// The sum of min and max.
        /// </summary>
        public float Sum => Min + Max;

        /// <param name="a"> The min or max value. </param>
        /// <param name="b"> The min or max value. </param>
        public FloatRange(float a, float b)
        {
            m_Min = a < b ? a : b;
            m_Max = a > b ? a : b;
        }

        public static implicit operator Vector2(FloatRange value)
        {
            return new Vector2(value.Min, value.Max);
        }

        public static implicit operator FloatRange(Vector2 value)
        {
            return new FloatRange(value.x, value.y);
        }

        public static bool operator ==(FloatRange a, FloatRange b)
        {
            return Mathf.Abs(a.Min - b.Min) < Mathf.Epsilon && Mathf.Abs(a.Max - b.Max) < Mathf.Epsilon;
        }

        public static bool operator !=(FloatRange a, FloatRange b)
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
            return $"[{Min:F}, {Max:F}]";
        }

        /// <summary>
        /// Returns true if the value is between min [inclusive] and max [inclusive].
        /// </summary>
        public bool Contains(int value)
        {
            return value >= Min && value <= Max;
        }

        /// <summary>
        /// Returns true if the value is between min [inclusive] and max [inclusive].
        /// </summary>
        public bool Contains(float value)
        {
            return value >= Min && value <= Max;
        }

        public bool Equals(FloatRange other)
        {
            return this == other;
        }
    }
}
