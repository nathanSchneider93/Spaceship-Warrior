using JetBrains.Annotations;
using UnityEngine.SceneManagement;

namespace Coimbra
{
    /// <summary>
    /// Singleton pattern where the newer object is held and the older one is destroyed.
    /// </summary>
    [PublicAPI]
    public abstract class DynamicSceneSingleton<T> : SceneSingletonBase<T> where T : DynamicSceneSingleton<T>
    {
        /// <summary>
        /// Use this method for initialization instead of Awake callback.
        /// </summary>
        /// <param name="previous"> The previous instance. </param>
        protected abstract void OnInitialize(T previous);

        /// <summary>
        /// Do not call this method!
        /// </summary>
        protected sealed override void OnAwake()
        {
            Scene scene = gameObject.scene;
            T previous = Exists(scene) ? GetInstance(scene, false) : null;
            var current = this as T;

            if (previous == current)
            {
                return;
            }

            SetInstance(scene, current);
            OnInitialize(previous);

            if (previous != null)
            {
                Destroy(previous.gameObject);
            }
        }
    }
}
