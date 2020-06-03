using Coimbra;
using UnityEngine;

namespace SpaceshipWarrior
{
    public sealed class CameraSystem : Singleton<CameraSystem>
    {
        private Camera m_Camera;
        private Transform m_CameraTransform;

        private static CameraSystem Instance => GetInstance(true);
        private static Camera Camera
        {
            get
            {
                if (Instance.m_Camera == null)
                {
                    Instance.m_Camera = Camera.main;
                }

                return Instance.m_Camera;
            }
        }
        private static Transform CameraTransform
        {
            get
            {
                if (Instance.m_CameraTransform == null)
                {
                    Instance.m_CameraTransform = Camera.transform;
                }

                return Instance.m_CameraTransform;
            }
        }

        public static Vector3 GetCameraPosition()
        {
            return CameraTransform.position;
        }

        public static Vector3 ScreenToWorldPoint(Vector3 value)
        {
            return Camera.ScreenToWorldPoint(value);
        }

        protected override void OnDispose(bool isInstance)
        {
        }

        protected override void OnInitialize()
        {
        }
    }
}
