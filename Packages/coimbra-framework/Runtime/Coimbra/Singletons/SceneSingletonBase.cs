using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Coimbra
{
    [PublicAPI, DisallowMultipleComponent]
    public abstract class SceneSingletonBase<T> : MonoBehaviour where T : SceneSingletonBase<T>
    {
        private static readonly Dictionary<string, T> Instances = new Dictionary<string, T>();
        private static readonly List<T> TargetSceneInstancesCache = new List<T>();

        [System.NonSerialized] private bool _isInitialized;

        private void Awake()
        {
            if (_isInitialized)
            {
                return;
            }

            OnAwake();
            _isInitialized = true;
        }

        private void OnDestroy()
        {
            string path = gameObject.scene.path;
            bool isInstance = Instances.TryGetValue(path, out T instance) && instance == this as T;

            OnDispose(isInstance);

            if (isInstance)
            {
                Instances.Remove(path);
            }
        }

        /// <summary>
        /// Use it before accessing the singleton when inside of OnDestroy callbacks.
        /// </summary>
        public static bool Exists(string targetScenePath)
        {
            if (Instances.TryGetValue(targetScenePath, out T instance))
            {
                return instance != null;
            }

            Scene targetScene = SceneManager.GetSceneByPath(targetScenePath);

            return GetInstance(targetScene, false);
        }

        /// <summary>
        /// Use it before accessing the singleton when inside of OnDestroy callbacks.
        /// </summary>
        public static bool Exists(Scene targetScene)
        {
            return Exists(targetScene.path);
        }

        /// <summary>
        /// Was this instance already initialized?
        /// </summary>
        public static bool IsInitialized(string targetScenePath)
        {
            return Instances.TryGetValue(targetScenePath, out T instance) && instance != null && instance._isInitialized;
        }

        /// <summary>
        /// Was this instance already initialized?
        /// </summary>
        public static bool IsInitialized(Scene targetScene)
        {
            return IsInitialized(targetScene.path);
        }

        /// <summary>
        /// Set the singleton instance.
        /// </summary>
        protected static void SetInstance(string targetScenePath, T value)
        {
            if (value != null)
            {
                Instances[targetScenePath] = value;
                value.Awake();
            }
            else if (Instances.ContainsKey(targetScenePath))
            {
                Instances.Remove(targetScenePath);
            }
        }

        /// <summary>
        /// Set the singleton instance.
        /// </summary>
        protected static void SetInstance(Scene targetScene, T value)
        {
            SetInstance(targetScene.path, value);

            if (value != null)
            {
                value.Awake();
            }
        }

        /// <summary>
        /// Get the singleton instance.
        /// </summary>
        protected static T GetInstance(Scene targetScene, bool createIfNone)
        {
            if (Instances.TryGetValue(targetScene.path, out T instance) && instance != null)
            {
                return instance;
            }

            T[] allScenesInstances = FindObjectsOfType<T>();

            if (allScenesInstances != null && allScenesInstances.Length > 0)
            {
                for (var i = 0; i < allScenesInstances.Length; i++)
                {
                    if (allScenesInstances[i].gameObject.scene.path == targetScene.path)
                    {
                        TargetSceneInstancesCache.Add(allScenesInstances[i]);
                    }
                }

                if (TargetSceneInstancesCache.Count > 0)
                {
                    if (TargetSceneInstancesCache.Count > 1)
                    {
                        Debug.LogError($"Found {TargetSceneInstancesCache.Count} objects of type {typeof(T)} found on scene {targetScene}! Please, ensure there's only one!");

                        for (var i = 1; i < TargetSceneInstancesCache.Count; i++)
                        {
                            DestroyImmediate(TargetSceneInstancesCache[i]);
                        }
                    }

                    instance = TargetSceneInstancesCache[0];
                    instance.Awake();
                    TargetSceneInstancesCache.Clear();

                    return instance;
                }
            }

            if (createIfNone)
            {
                instance = new GameObject(typeof(T).Name).AddComponent<T>();

                if (instance.gameObject.scene.path != targetScene.path)
                {
                    SceneManager.MoveGameObjectToScene(instance.gameObject, targetScene);
                }

                instance.Awake();
            }

            return instance;
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
