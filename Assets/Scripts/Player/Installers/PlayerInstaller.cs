using UnityEngine;
using Zenject;
using Player.Services;

namespace Player.Installers
{
    /// <summary>
    /// 場景級別的 Player Installer
    /// 路徑: Assets/Scripts/Player/Installers/PlayerInstaller.cs
    /// </summary>
    public class PlayerInstaller : MonoInstaller
    {
        [Header("Configuration")]
        [SerializeField] private PlayerMovementConfig _movementConfig;

        [Header("Player Setup")]
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private bool _createPlayerFromPrefab = false;
        [SerializeField] private Vector3 _spawnPosition = Vector3.zero;

        public override void InstallBindings()
        {
            Debug.Log("[PlayerInstaller] Installing player bindings...");

            InstallConfiguration();
            InstallPlayerServices();
            InstallPlayer();
        }

        private void InstallConfiguration()
        {
            if (_movementConfig != null)
            {
                Container.BindInstance(_movementConfig).AsSingle();
                Debug.Log($"[PlayerInstaller] Bound PlayerMovementConfig: {_movementConfig.name}");
            }
            else
            {
                Debug.LogError("[PlayerInstaller] PlayerMovementConfig is not assigned!");
            }
        }

        private void InstallPlayerServices()
        {
            // 相機服務（場景級別）
            Container.Bind<ICameraService>()
                .To<CameraService>()
                .AsSingle()
                .NonLazy();

            // 注意：InputService 來自 Project Context，不在這裡綁定

            Debug.Log("[PlayerInstaller] Player services bound");
        }

        private void InstallPlayer()
        {
            if (_createPlayerFromPrefab && _playerPrefab != null)
            {
                // 從 Prefab 建立玩家
                Container.Bind<PlayerController>()
                    .FromComponentInNewPrefab(_playerPrefab)
                    .AsSingle()
                    .OnInstantiated<PlayerController>((ctx, player) =>
                    {
                        player.transform.position = _spawnPosition;
                        Debug.Log($"[PlayerInstaller] Player spawned at {_spawnPosition}");
                    })
                    .NonLazy();
            }
            else
            {
                // 從場景中找尋玩家
                var existingPlayer = FindObjectOfType<PlayerController>();
                if (existingPlayer != null)
                {
                    Container.Bind<PlayerController>()
                        .FromInstance(existingPlayer)
                        .AsSingle();

                    Debug.Log($"[PlayerInstaller] Found existing player: {existingPlayer.name}");
                }
                else
                {
                    Debug.LogWarning("[PlayerInstaller] No player found in scene and no prefab assigned!");
                }
            }
        }
    }
}