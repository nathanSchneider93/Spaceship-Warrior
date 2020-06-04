using JetBrains.Annotations;
using UnityEngine;

namespace Coimbra
{
    [PublicAPI, DisallowMultipleComponent]
    public abstract class SingletonBase<T> : MonoBehaviour where T : SingletonBase<T>
    {
        private static T _instance;

        /// <summary>
        /// Use it before accessing the singleton when inside of OnDestroy callbacks.
        /// </summary>
        public static bool Exists => _instance != null;

        /// <summary>
        /// Was this instance already initialized?
        /// </summary>
        protected bool IsInitialized { get; private set; }

        private void Awake()
        {
            if (IsInitialized)
            {
                return;
            }

            OnAwake();
            IsInitialized = true;
        }

        private void OnDestroy()
        {
            bool isInstance = _instance == this as T;

            OnDispose(isInstance);

            if (isInstance)
            {
                _instance = null;
            }
        }

        /// <summary>
        /// Set the singleton instance.
        /// </summary>
        protected static void SetInstance(T value)
        {
            _instance = value;

            if (value != null)
            {
                value.Awake();
            }
        }

        /// <summary>
        /// Get the singleton instance.
        /// </summary>
        protected static T GetInstance(bool createIfNone)
        {
            if (_instance != null)
            {
                return _instance;
            }

            T[] instances = FindObjectsOfType<T>();

            if (instances != null && instances.Length > 0)
            {
                if (instances.Length > 1)
                {
                    Debug.LogError($"Found {instances.Length} objects of type {typeof(T)}! Please, ensure there's only one!");

                    for (int i = 1; i < instances.Length; i++)
                    {
                        DestroyImmediate(instances[i]);
                    }
                }

                instances[0].Awake();
            }
            else if (createIfNone)
            {
                new GameObject(typeof(T).Name).AddComponent<T>().Awake();
            }

            return _instance;
        }

        /// <summary>
        /// Use this method for initialization instead of Awake callback.
        /// </summary>
        protected abstract void OnAwake();

        /// <summary>
        /// Use this method to un-initialize instead of OnDestroy callback.
        /// </summary>
        protected abstract void OnDispose(bool isInstance);
    }
}
