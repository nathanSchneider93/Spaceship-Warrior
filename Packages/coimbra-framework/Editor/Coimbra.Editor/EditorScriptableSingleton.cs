using JetBrains.Annotations;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Singleton pattern for <see cref="ScriptableObject"/>s when inside the Unity Editor.
    /// </summary>
    [PublicAPI]
    public abstract class EditorScriptableSingleton<T> : ScriptableObject where T : EditorScriptableSingleton<T>
    {
        private static T _instance;

        /// <summary>
        /// Use it before accessing the singleton when inside of OnDestroy callbacks.
        /// </summary>
        public static bool Exists => _instance != null || GetInstance(false) != null;

        /// <summary>
        /// Default asset path when the instance is automatically created.
        /// </summary>
        protected virtual string DefaultAssetPath => $"Assets/Data/{_instance.GetType().Name}.asset";

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

            AssetDatabase.Refresh();

            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");

            if (guids != null && guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                _instance = AssetDatabase.LoadAssetAtPath<T>(path);

                if (guids.Length > 1)
                {
                    string error = $"Found {guids.Length} objects of type {typeof(T)}! Please, ensure there's only one!";

                    for (int i = 0; i < guids.Length; i++)
                    {
                        path = AssetDatabase.GUIDToAssetPath(guids[i]);
                        Debug.LogError(error, AssetDatabase.LoadAssetAtPath<T>(path));
                    }
                }

                _instance.OnInitialize();
            }
            else if (createIfNone)
            {
                _instance = CreateInstance<T>();

                string assetPath = _instance.DefaultAssetPath;
                string fullPath = Path.GetFullPath(assetPath);
                string assetFolder = Path.GetDirectoryName(fullPath);

                if (Directory.Exists(assetFolder) == false)
                {
                    Directory.CreateDirectory(assetFolder);
                }

                AssetDatabase.CreateAsset(_instance, assetPath);
                AssetDatabase.SaveAssets();

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
