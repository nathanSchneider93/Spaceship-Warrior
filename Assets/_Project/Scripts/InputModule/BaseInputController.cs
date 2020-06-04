using UnityEngine;

namespace SpaceshipWarrior.InputModule
{
    public abstract class BaseInputController : ScriptableObject
    {
        public abstract void Initialize();

        public abstract bool GetShootKeyDown();

        public abstract Vector3 GetLookPoint();
    }
}
