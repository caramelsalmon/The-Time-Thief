using UnityEngine;

namespace Player
{
    /// <summary>
    /// 玩家移動參數
    /// 路徑: Assets/Scripts/Player/PlayerMovementConfig.cs
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerMovementConfig", menuName = "Player/Movement Config")]
    public class PlayerMovementConfig : ScriptableObject
    {
        [Header("移動設定")]
        [Tooltip("走路速度")]
        public float walkSpeed = 5f;

        [Tooltip("跑步速度")]
        public float runSpeed = 10f;

        [Tooltip("跳躍高度")]
        public float jumpHeight = 2f;

        [Tooltip("重力值")]
        public float gravity = -19.62f;

        [Header("地面檢測")]
        [Tooltip("地面檢測距離")]
        public float groundCheckDistance = 0.4f;

        [Tooltip("地面圖層")]
        public LayerMask groundLayers = -1;

        [Header("視角設定")]
        [Tooltip("滑鼠靈敏度")]
        public float mouseSensitivity = 2f;

        [Tooltip("垂直視角限制")]
        public float lookXLimit = 45f;

        [Header("移動平滑")]
        [Tooltip("移動平滑時間")]
        public float movementSmoothTime = 0.1f;

        [Tooltip("旋轉平滑時間")]
        public float rotationSmoothTime = 0.1f;
    }
}