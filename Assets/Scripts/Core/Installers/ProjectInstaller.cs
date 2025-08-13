using UnityEngine;
using Zenject;
using Core.Input;

namespace Core.Installers
{
    /// <summary>
    /// 全域 Project Context Installer
    /// 路徑: Assets/Scripts/Core/Installers/ProjectInstaller.cs
    /// </summary>
    public class ProjectInstaller : MonoInstaller
    {
        [Header("Global Settings")]
        [SerializeField] private bool enableDebugMode = false;

        public override void InstallBindings()
        {
            Debug.Log("[ProjectInstaller] Installing global bindings...");

            InstallInput();
            InstallGlobalServices();

            if (enableDebugMode)
            {
                InstallDebugServices();
            }
        }

        private void InstallInput()
        {
            // 綁定 InputService 並註冊它實作的所有介面 (IInputService, IInitializable)
            Container.BindInterfacesAndSelfTo<InputService>()
                .AsSingle()
                .NonLazy();

            Debug.Log("[ProjectInstaller] Input service bound");
        }

        private void InstallGlobalServices()
        {
            // 其他全域服務可以在這裡加入

            // 範例：音效管理
            // Container.Bind<IAudioManager>()
            //     .To<AudioManager>()
            //     .AsSingle();

            // 範例：存檔系統
            // Container.Bind<ISaveManager>()
            //     .To<SaveManager>()
            //     .AsSingle();

            // 範例：場景管理
            // Container.Bind<ISceneLoader>()
            //     .To<SceneLoader>()
            //     .AsSingle();

            Debug.Log("[ProjectInstaller] Global services bound");
        }

        private void InstallDebugServices()
        {
            // 除錯服務
            Debug.Log("[ProjectInstaller] Debug mode enabled");

            // 範例：除錯控制台
            // Container.Bind<IDebugConsole>()
            //     .To<DebugConsole>()
            //     .AsSingle();
        }
    }
}