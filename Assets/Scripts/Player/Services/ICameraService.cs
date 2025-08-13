using UnityEngine;

namespace Player.Services
{
    /// <summary>
    /// 相機服務介面
    /// 路徑: Assets/Scripts/Player/Services/ICameraService.cs
    /// </summary>
    public interface ICameraService
    {
        Transform CameraTransform { get; }
        void SetFollowTarget(Transform target);
        void UpdateRotation(float xRotation);
        void SetFieldOfView(float fov);
        void ResetCamera();
    }
}