using JetBrains.Annotations;
using UnityEngine.SceneManagement;

namespace Coimbra
{
    /// <summary>
    /// Singleton pattern where the first object created is held and any other newer is destroyed.
    /// </summary>
    [PublicAPI]
    public abstract class SceneSingleton<T> : SceneSingletonBase<T> where T : SceneSingleton<T>
    {
        /// <summary>
        /// Use this method for initialization instead of Awake callback.
        /// </summary>
        protected abstract void OnInitialize();

        /// <summary>
        /// Do not call this method!
        /// </summary>
        protected sealed override void OnAwake()
        {
            Scene scene = gameObject.scene;

            if (Exists(scene) == false)
            {
                SetInstance(scene, this as T);
                OnInitialize();
            }
            else if (GetInstance(scene, false) != this as T)
            {
                Destroy(gameObject);
            }
        }
    }
}
