using CubeTower.Gameplay;
using CubeTower.Gameplay.Signals;
using CubeTower.Settings;
using UnityEngine;
using Zenject;

namespace CubeTower.Installers
{
    /// <summary>
    /// Scene-level Zenject installer. Holds every SerializeField reference that
    /// used to live on the old GameController MonoBehaviour, and binds the
    /// gameplay services so the rewritten plain-C# <see cref="GameController"/>
    /// orchestrator can be constructed by the container.
    /// </summary>
    public class GameInstaller : MonoInstaller
    {
        [Header("Scene refs")]
        [SerializeField] private Transform ghostCube;
        [SerializeField] private Transform allCubesTransform;
        [SerializeField] private Rigidbody allCubesRb;
        [SerializeField] private GameObject cubePrefab;
        [SerializeField] private Transform mainCamera;
        [SerializeField] private GameObject[] canvasStartPage;

        [Header("Settings")]
        [SerializeField] private GameSettings gameSettings;

        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<LoseSignal>();
            Container.DeclareSignal<FirstCubeSignal>();
            Container.DeclareSignal<CubePlacedSignal>();

            Container.BindInstance(gameSettings).IfNotBound();

            Container.Bind<GridState>().AsSingle();
            Container.Bind<GameState>().AsSingle();

            Container.BindInterfacesAndSelfTo<InputService>().AsSingle();

            Container.BindInterfacesAndSelfTo<CameraController>().AsSingle()
                .WithArguments(mainCamera);

            Container.Bind<CubeFactory>()
                .AsSingle()
                .WithArguments(cubePrefab, allCubesTransform);

            Container.Bind<CubeSpawner>().AsSingle();

            Container.BindInterfacesAndSelfTo<GhostCubeController>().AsSingle()
                .WithArguments(ghostCube);

            Container.BindInterfacesAndSelfTo<GameController>().AsSingle()
                .WithArguments(allCubesRb, canvasStartPage);
        }
    }
}
