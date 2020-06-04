using JetBrains.Annotations;

namespace Coimbra
{
    /// <summary>
    /// Singleton pattern where the first object created is held and any other newer is destroyed.
    /// </summary>
    [PublicAPI]
    public abstract class Singleton<T> : SingletonBase<T> where T : Singleton<T>
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
            if (Exists == false)
            {
                SetInstance(this as T);
                OnInitialize();
            }
            else if (GetInstance(false) != this as T)
            {
                Destroy(gameObject);
            }
        }
    }
}
