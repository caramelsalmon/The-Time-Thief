using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Core.Input
{
    /// <summary>
    /// 全域輸入服務 - 管理所有輸入
    /// 路徑: Assets/Scripts/Core/Input/InputService.cs
    /// </summary>
    public class InputService : IInputService, IInitializable, IDisposable
    {
        private PlayerInput _playerInput;
        private InputActionAsset _inputActions;

        // Action Maps
        private InputActionMap _gameplayMap;
        private InputActionMap _uiMap;

        // 輸入狀態
        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool IsJumping { get; private set; }
        public bool IsRunning { get; private set; }

        // 事件
        public event Action OnJumpPressed;
        public event Action OnJumpReleased;
        public event Action OnRunStarted;
        public event Action OnRunCanceled;
        public event Action OnMenuPressed;
        public event Action OnInteractPressed;

        public void Initialize()
        {
            CreatePlayerInput();
            SetupActionMaps();
            SubscribeToEvents();

            // 預設啟用遊戲輸入
            EnableGameplayInput();

            Debug.Log("[InputService] Initialized as global service");
        }

        private void CreatePlayerInput()
        {
            // 建立 PlayerInput GameObject
            GameObject inputGO = new GameObject("[PlayerInput]");
            GameObject.DontDestroyOnLoad(inputGO);

            _playerInput = inputGO.AddComponent<PlayerInput>();

            // 載入 Input Actions Asset
            _inputActions = Resources.Load<InputActionAsset>("InputSystem_Actions");
            if (_inputActions == null)
            {
                Debug.LogError("[InputService] InputSystem_Actions not found in Resources folder!");
                return;
            }

            _playerInput.actions = _inputActions;
            _playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
            _playerInput.defaultActionMap = "Player";

            Debug.Log("[InputService] PlayerInput created with C# Events");
        }

        private void SetupActionMaps()
        {
            if (_inputActions == null) return;

            _gameplayMap = _inputActions.FindActionMap("Player");
            _uiMap = _inputActions.FindActionMap("UI");

            if (_gameplayMap == null)
            {
                Debug.LogError("[InputService] 'Player' action map not found!");
            }

            if (_uiMap == null)
            {
                Debug.LogWarning("[InputService] 'UI' action map not found");
            }
        }

        private void SubscribeToEvents()
        {
            if (_playerInput == null) return;

            _playerInput.onActionTriggered += OnActionTriggered;
            Debug.Log("[InputService] Subscribed to input events");
        }

        private void OnActionTriggered(InputAction.CallbackContext context)
        {
            // 處理不同的輸入動作
            switch (context.action.name)
            {
                case "Move":
                    HandleMoveInput(context);
                    break;

                case "Look":
                    HandleLookInput(context);
                    break;

                case "Jump":
                    HandleJumpInput(context);
                    break;

                case "Run":
                case "Sprint":
                    HandleRunInput(context);
                    break;

                case "Menu":
                case "Pause":
                    HandleMenuInput(context);
                    break;

                case "Interact":
                    HandleInteractInput(context);
                    break;
            }
        }

        private void HandleMoveInput(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
        }

        private void HandleLookInput(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
        }

        private void HandleJumpInput(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                IsJumping = true;
                OnJumpPressed?.Invoke();
            }
            else if (context.canceled)
            {
                IsJumping = false;
                OnJumpReleased?.Invoke();
            }
        }

        private void HandleRunInput(InputAction.CallbackContext context)
        {
            if (context.started || context.performed)
            {
                IsRunning = true;
                OnRunStarted?.Invoke();
            }
            else if (context.canceled)
            {
                IsRunning = false;
                OnRunCanceled?.Invoke();
            }
        }

        private void HandleMenuInput(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnMenuPressed?.Invoke();
            }
        }

        private void HandleInteractInput(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnInteractPressed?.Invoke();
            }
        }

        // 輸入控制方法
        public void EnableGameplayInput()
        {
            _gameplayMap?.Enable();
            _uiMap?.Disable();

            // 鎖定游標（FPS 遊戲）
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Debug.Log("[InputService] Gameplay input enabled");
        }

        public void DisableGameplayInput()
        {
            _gameplayMap?.Disable();

            // 重置移動輸入
            MoveInput = Vector2.zero;
            LookInput = Vector2.zero;
            IsJumping = false;
            IsRunning = false;

            Debug.Log("[InputService] Gameplay input disabled");
        }

        public void EnableUIInput()
        {
            _uiMap?.Enable();
            _gameplayMap?.Disable();

            // 顯示游標
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Debug.Log("[InputService] UI input enabled");
        }

        public void DisableUIInput()
        {
            _uiMap?.Disable();
            Debug.Log("[InputService] UI input disabled");
        }

        public void EnableAllInput()
        {
            _playerInput?.ActivateInput();
        }

        public void DisableAllInput()
        {
            _playerInput?.DeactivateInput();
        }

        public PlayerInput GetPlayerInput()
        {
            return _playerInput;
        }

        public void Dispose()
        {
            if (_playerInput != null)
            {
                _playerInput.onActionTriggered -= OnActionTriggered;

                if (_playerInput.gameObject != null)
                {
                    GameObject.Destroy(_playerInput.gameObject);
                }
            }

            Debug.Log("[InputService] Disposed");
        }
    }
}