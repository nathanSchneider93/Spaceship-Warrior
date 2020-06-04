using JetBrains.Annotations;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Singleton pattern for <see cref="ScriptableObject"/>s. The asset should be put inside of a <see cref="Resources"/> folder.
    /// </summary>
    [PublicAPI]
    public abstract class ScriptableSingleton<T> : ScriptableObject where T : ScriptableSingleton<T>
    {
        private static T _instance;

        /// <summary>
        /// Use it before accessing the singleton when inside of OnDestroy callbacks.
        /// </summary>
        public static bool Exists => _instance != null || GetInstance(false) != null;

        /// <summary>
        /// Default asset path when the instance is automatically created.
        /// </summary>
        protected virtual string DefaultAssetPath => $"Assets/Resources/{_instance.GetType().Name}.asset";

        /// <summary>
        /// Set the singleton instance.
        /// </summary>
        protected static void SetInstance(T value)
        {
            _instance = value;
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

            T[] instances = Resources.LoadAll<T>("");

            if (instances != null && instances.Length > 0)
            {
                if (instances.Length > 1)
                {
                    string error = $"Found {instances.Length} objects of type {typeof(T)}! Please, ensure there's only one!";

                    for (int i = 0; i < instances.Length; i++)
                    {
                        Debug.LogError(error, instances[i]);
                    }
                }

                _instance = instances[0];
                _instance.OnInitialize();
            }
            else if (createIfNone)
            {
                _instance = CreateInstance<T>();
#if UNITY_EDITOR
                string assetPath = _instance.DefaultAssetPath;
                string fullPath = System.IO.Path.GetFullPath(assetPath);
                string assetFolder = System.IO.Path.GetDirectoryName(fullPath);

                if (System.IO.Directory.Exists(assetFolder) == false)
                {
                    System.IO.Directory.CreateDirectory(assetFolder);
                }

                UnityEditor.AssetDatabase.CreateAsset(_instance, assetPath);
                UnityEditor.AssetDatabase.SaveAssets();
#endif
                _instance.OnInitialize();
            }

            return _instance;
        }

        /// <summary>
        /// Use this method for initialization.
        /// </summary>
        protected abstract void OnInitialize();
    }
}
