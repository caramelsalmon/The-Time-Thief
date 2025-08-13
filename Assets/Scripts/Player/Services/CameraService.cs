using UnityEngine;
using Zenject;

namespace Player.Services
{
    /// <summary>
    /// 相機服務實作 - 管理第一人稱相機
    /// 路徑: Assets/Scripts/Player/Services/CameraService.cs
    /// </summary>
    public class CameraService : ICameraService
    {
        private Camera _mainCamera;
        private Transform _cameraHolder;
        private float _defaultFOV = 60f;

        public Transform CameraTransform => _mainCamera?.transform;

        [Inject]
        public CameraService()
        {
            InitializeCamera();
        }

        private void InitializeCamera()
        {
            _mainCamera = Camera.main;

            if (_mainCamera == null)
            {
                Debug.LogError("Main Camera not found! Please ensure there's a camera tagged as 'MainCamera' in the scene.");
                return;
            }

            _defaultFOV = _mainCamera.fieldOfView;
            Debug.Log("CameraService initialized with Main Camera");
        }

        public void SetFollowTarget(Transform target)
        {
            if (target == null)
            {
                Debug.LogWarning("Cannot set null target for camera follow");
                return;
            }

            // 建立相機支架
            if (_cameraHolder == null)
            {
                GameObject holder = new GameObject("CameraHolder");
                _cameraHolder = holder.transform;
            }

            // 設定相機支架為玩家子物件
            _cameraHolder.SetParent(target);
            _cameraHolder.localPosition = new Vector3(0, 1.6f, 0); // 頭部高度
            _cameraHolder.localRotation = Quaternion.identity;

            // 將相機附加到支架
            if (_mainCamera != null)
            {
                _mainCamera.transform.SetParent(_cameraHolder);
                _mainCamera.transform.localPosition = Vector3.zero;
                _mainCamera.transform.localRotation = Quaternion.identity;
            }

            Debug.Log($"Camera following target: {target.name}");
        }

        public void UpdateRotation(float xRotation)
        {
            if (_cameraHolder != null)
            {
                _cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            }
        }

        public void SetFieldOfView(float fov)
        {
            if (_mainCamera != null)
            {
                _mainCamera.fieldOfView = Mathf.Clamp(fov, 30f, 120f);
            }
        }

        public void ResetCamera()
        {
            if (_cameraHolder != null)
            {
                _cameraHolder.localRotation = Quaternion.identity;
            }

            if (_mainCamera != null)
            {
                _mainCamera.fieldOfView = _defaultFOV;
            }
        }
    }
}