using CubeTower.Gameplay.Signals;
using CubeTower.Settings;
using UnityEngine;
using Zenject;

namespace CubeTower.Gameplay
{
    /// <summary>
    /// Thin orchestrator that replaces the 227-line <c>GameController</c>
    /// MonoBehaviour. Listens for taps, commits a cube, updates grid state,
    /// notifies the camera, and watches the tower's rigidbody for loss.
    /// </summary>
    public class GameController : IInitializable, ITickable
    {
        private readonly InputService _input;
        private readonly CubeSpawner _spawner;
        private readonly GridState _grid;
        private readonly GhostCubeController _ghost;
        private readonly CameraController _camera;
        private readonly GameState _state;
        private readonly GameSettings _settings;
        private readonly SignalBus _bus;
        private readonly Rigidbody _allCubesRb;
        private readonly GameObject[] _canvasStartPage;

        public GameController(
            InputService input,
            CubeSpawner spawner,
            GridState grid,
            GhostCubeController ghost,
            CameraController camera,
            GameState state,
            GameSettings settings,
            SignalBus bus,
            Rigidbody allCubesRb,
            GameObject[] canvasStartPage)
        {
            _input = input;
            _spawner = spawner;
            _grid = grid;
            _ghost = ghost;
            _camera = camera;
            _state = state;
            _settings = settings;
            _bus = bus;
            _allCubesRb = allCubesRb;
            _canvasStartPage = canvasStartPage;
        }

        public void Initialize()
        {
            _bus.Subscribe<FirstCubeSignal>(OnFirstCube);
            _bus.Subscribe<LoseSignal>(OnLose);

            _camera.NotifyGridGrowth(0, _state.CurrentTop.y, 0, _state.CurrentTop.y);
        }

        public void Tick()
        {
            if (_input.TryConsumeTap() && !_state.IsLose)
            {
                CommitCube();
            }

            if (!_state.IsLose
                && _allCubesRb != null
                && _allCubesRb.velocity.magnitude > _settings.LoseVelocityThreshold)
            {
                _state.SetLose();
            }
        }

        private void CommitCube()
        {
            if (_ghost.GhostTransform == null)
            {
                return;
            }

            if (!_state.FirstCube)
            {
                _state.MarkFirstCube();
            }

            var newPos = _ghost.CurrentGhostPos;
            _spawner.Spawn(newPos);
            _state.SetCurrentTop(newPos);

            // Preserved from the old GameController (L83-84): toggling
            // isKinematic forces Unity to re-awaken the rigidbody so the
            // freshly-parented child mass gets picked up.
            if (_allCubesRb != null)
            {
                _allCubesRb.isKinematic = true;
                _allCubesRb.isKinematic = false;
            }

            _bus.Fire(new CubePlacedSignal(newPos));

            _ghost.Reposition();

            _grid.GetExtents(out var maxX, out var maxY, out var maxZ);
            _camera.NotifyGridGrowth(maxX, maxY, maxZ, newPos.y);
        }

        private void OnFirstCube()
        {
            if (_canvasStartPage == null)
            {
                return;
            }

            foreach (var go in _canvasStartPage)
            {
                if (go != null)
                {
                    Object.Destroy(go);
                }
            }
        }

        private void OnLose()
        {
            _ghost.Disable();
            if (_ghost.GhostTransform != null)
            {
                Object.Destroy(_ghost.GhostTransform.gameObject);
            }
        }
    }
}
