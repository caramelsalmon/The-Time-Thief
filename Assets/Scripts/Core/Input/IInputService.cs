using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Input
{
    /// <summary>
    /// 全域輸入服務介面
    /// 路徑: Assets/Scripts/Core/Input/IInputService.cs
    /// </summary>
    public interface IInputService
    {
        // 輸入狀態
        Vector2 MoveInput { get; }
        Vector2 LookInput { get; }
        bool IsJumping { get; }
        bool IsRunning { get; }

        // 輸入事件
        event Action OnJumpPressed;
        event Action OnJumpReleased;
        event Action OnRunStarted;
        event Action OnRunCanceled;
        event Action OnMenuPressed;
        event Action OnInteractPressed;

        // 輸入控制
        void EnableGameplayInput();
        void DisableGameplayInput();
        void EnableUIInput();
        void DisableUIInput();
        void EnableAllInput();
        void DisableAllInput();

        // 取得 PlayerInput 實例（特殊情況使用）
        PlayerInput GetPlayerInput();
    }
}