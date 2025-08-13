using UnityEngine;
using Zenject;
using Core.Input;
using Player.Services;

namespace Player
{
    /// <summary>
    /// 玩家控制器 - 使用全域 InputService
    /// 路徑: Assets/Scripts/Player/PlayerController.cs
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private bool _showDebugInfo = false;

        // 依賴注入
        private PlayerMovementConfig _config;
        private IInputService _inputService;  // 從 Project Context 注入
        private ICameraService _cameraService;  // 從 Scene Context 注入

        // 組件
        private CharacterController _characterController;
        private Transform _transform;

        // 移動狀態
        private Vector3 _velocity;
        private Vector2 _smoothedMovementInput;
        private Vector2 _movementInputVelocity;
        private bool _isGrounded;
        private float _rotationX = 0f;

        // 狀態
        private bool _isPaused = false;

        // 屬性
        public bool IsGrounded => _isGrounded;
        public bool IsMoving => _smoothedMovementInput.magnitude > 0.1f;
        public bool IsRunning => _inputService?.IsRunning ?? false;
        public float CurrentSpeed => IsRunning ? _config.runSpeed : _config.walkSpeed;

        [Inject]
        public void Construct(
            PlayerMovementConfig config,
            IInputService inputService,  // 來自 Project Context
            ICameraService cameraService) // 來自 Scene Context
        {
            _config = config;
            _inputService = inputService;
            _cameraService = cameraService;

            Debug.Log("[PlayerController] Dependencies injected");
            Debug.Log($"[PlayerController] InputService null? {_inputService == null}");
            Debug.Log($"[PlayerController] CameraService null? {_cameraService == null}");
        }

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _transform = transform;

            if (_characterController == null)
            {
                Debug.LogError("[PlayerController] CharacterController component missing!");
            }
        }

        private void Start()
        {
            InitializePlayer();
        }

        private void OnEnable()
        {
            SubscribeToEvents();

            // 啟用遊戲輸入
            _inputService?.EnableGameplayInput();
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();

            // 不要在這裡禁用輸入，因為可能其他系統還需要
        }

        private void OnDestroy()
        {
            // 清理
            UnsubscribeFromEvents();
        }

        private void Update()
        {
            if (_isPaused) return;

            CheckGrounded();
            HandleMovement();
            HandleRotation();
            ApplyGravity();

            if (_showDebugInfo)
            {
                ShowDebugInfo();
            }
        }

        private void InitializePlayer()
        {
            // 設定相機跟隨
            _cameraService?.SetFollowTarget(_transform);

            Debug.Log("[PlayerController] Player initialized");
        }

        private void SubscribeToEvents()
        {
            if (_inputService != null)
            {
                _inputService.OnJumpPressed += HandleJump;
                _inputService.OnRunStarted += OnRunStarted;
                _inputService.OnRunCanceled += OnRunCanceled;
                _inputService.OnMenuPressed += OnMenuPressed;
                _inputService.OnInteractPressed += OnInteractPressed;

                Debug.Log("[PlayerController] Subscribed to input events");
            }
            else
            {
                Debug.LogWarning("[PlayerController] InputService is null, cannot subscribe to events");
            }
        }

        private void UnsubscribeFromEvents()
        {
            if (_inputService != null)
            {
                _inputService.OnJumpPressed -= HandleJump;
                _inputService.OnRunStarted -= OnRunStarted;
                _inputService.OnRunCanceled -= OnRunCanceled;
                _inputService.OnMenuPressed -= OnMenuPressed;
                _inputService.OnInteractPressed -= OnInteractPressed;
            }
        }

        private void CheckGrounded()
        {
            //  檢查null
            if (_transform == null) Debug.LogError("_transform is NULL");
            if (_characterController == null) Debug.LogError("_characterController is NULL");
            if (_config == null) Debug.LogError("_config is NULL");

            _isGrounded = Physics.CheckSphere(
            _transform.position + Vector3.down * 0.1f,  // 球心稍微往下
            _characterController.radius,
            _config.groundLayers
            );

            //Debug.Log($"[Grounded Debug] _isGrounded = {_isGrounded}");
        }

        private void HandleMovement()
        {
            if (_inputService == null) return;

            Vector2 currentInput = _inputService.MoveInput;

            // 平滑處理
            _smoothedMovementInput = Vector2.SmoothDamp(
                _smoothedMovementInput,
                currentInput,
                ref _movementInputVelocity,
                _config.movementSmoothTime
            );

            // 計算移動
            Vector3 move = _transform.right * _smoothedMovementInput.x +
                          _transform.forward * _smoothedMovementInput.y;

            float speed = _inputService.IsRunning ? _config.runSpeed : _config.walkSpeed;
            _characterController.Move(move * speed * Time.deltaTime);
        }

        private void HandleRotation()
        {
            if (_inputService == null) return;

            Vector2 lookInput = _inputService.LookInput;

            if (lookInput.sqrMagnitude < 0.01f) return;

            // 水平旋轉
            _transform.Rotate(Vector3.up * lookInput.x * _config.mouseSensitivity);

            // 垂直旋轉
            _rotationX -= lookInput.y * _config.mouseSensitivity;
            _rotationX = Mathf.Clamp(_rotationX, -_config.lookXLimit, _config.lookXLimit);

            _cameraService?.UpdateRotation(_rotationX);
        }

        private void HandleJump()
        {
            if (_isGrounded && !_isPaused)
            {
                _velocity.y = Mathf.Sqrt(_config.jumpHeight * -2f * _config.gravity);
                Debug.Log("[PlayerController] Jump!");
            }
        }

        private void OnRunStarted()
        {
            if (_cameraService != null)
            {
                _cameraService.SetFieldOfView(70f);
            }
        }

        private void OnRunCanceled()
        {
            if (_cameraService != null)
            {
                _cameraService.SetFieldOfView(60f);
            }
        }

        private void OnMenuPressed()
        {
            TogglePause();
        }

        private void OnInteractPressed()
        {
            Debug.Log("[PlayerController] Interact!");
            // 實作互動邏輯
        }

        private void ApplyGravity()
        {
            _velocity.y += _config.gravity * Time.deltaTime;
            _characterController.Move(_velocity * Time.deltaTime);
        }

        private void TogglePause()
        {
            _isPaused = !_isPaused;

            if (_isPaused)
            {
                _inputService?.EnableUIInput();
                Time.timeScale = 0f;
                Debug.Log("[PlayerController] Game paused");
            }
            else
            {
                _inputService?.EnableGameplayInput();
                Time.timeScale = 1f;
                Debug.Log("[PlayerController] Game resumed");
            }
        }

        private void ShowDebugInfo()
        {
            Debug.DrawRay(_transform.position, Vector3.down * _config.groundCheckDistance, Color.red);
            Debug.DrawRay(_transform.position, _transform.forward * 2f, Color.blue);
            Debug.DrawRay(_transform.position, _velocity.normalized * 2f, Color.green);
        }

        // 公開方法
        public void ResetPosition(Vector3 position)
        {
            _characterController.enabled = false;
            _transform.position = position;
            _characterController.enabled = true;
            _velocity = Vector3.zero;
            _rotationX = 0f;
            _cameraService?.ResetCamera();
        }

        public void SetMovementEnabled(bool enabled)
        {
            this.enabled = enabled;
        }
    }
}