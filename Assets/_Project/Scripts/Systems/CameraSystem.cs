using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public sealed class CameraSystem : SystemBase
{
    private Camera m_Camera;
    private Transform m_CameraTransform;

    private Camera Camera
    {
        get
        {
            if (m_Camera == null)
            {
                m_Camera = Camera.main;
            }

            return m_Camera;
        }
    }
    private Transform CameraTransform
    {
        get
        {
            if (m_CameraTransform == null)
            {
                m_CameraTransform = Camera.transform;
            }

            return m_CameraTransform;
        }
    }

    public float3 GetCameraPosition()
    {
        return CameraTransform.position;
    }

    public float3 ScreenToWorldPoint(float3 value)
    {
        return Camera.ScreenToWorldPoint(value);
    }

    protected override void OnUpdate()
    {
    }
}
