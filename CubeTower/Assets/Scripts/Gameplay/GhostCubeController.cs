using CubeTower.Settings;
using UnityEngine;
using Zenject;

namespace CubeTower.Gameplay
{
    /// <summary>
    /// Drives the blinking ghost cube that shows candidate placements. Replaces
    /// the old <c>ShowCubePlace</c> coroutine on GameController with an ITickable
    /// that runs a plain timer, and replaces the dead-branch logic in the old
    /// <c>SpawnPosition</c> (L160-167) with a single "no empty neighbors → lose"
    /// path routed through <see cref="GameState"/>.
    /// </summary>
    public class GhostCubeController : ITickable
    {
        private readonly Transform _ghost;
        private readonly GridState _grid;
        private readonly GameSettings _settings;
        private readonly GameState _state;

        private float _timer;
        private bool _disabled;

        public GhostCubeController(Transform ghost, GridState grid, GameSettings settings, GameState state)
        {
            _ghost = ghost;
            _grid = grid;
            _settings = settings;
            _state = state;
        }

        public Transform GhostTransform => _ghost;

        public Vector3Int CurrentGhostPos
        {
            get
            {
                var p = _ghost.position;
                return new Vector3Int(
                    Mathf.RoundToInt(p.x),
                    Mathf.RoundToInt(p.y),
                    Mathf.RoundToInt(p.z));
            }
        }

        public void Disable()
        {
            _disabled = true;
        }

        public void Tick()
        {
            if (_disabled || _state.IsLose || _ghost == null)
            {
                return;
            }

            _timer += Time.deltaTime;
            if (_timer >= _settings.ChangePlaceSpeed)
            {
                _timer = 0f;
                Reposition();
            }
        }

        /// <summary>
        /// Repositions the ghost to a random empty neighbor of the current top.
        /// Called both from the internal timer and from GameController directly
        /// after a cube is committed (so the ghost advances on the same frame
        /// as the placement, matching the old behavior).
        /// </summary>
        public void Reposition()
        {
            if (_ghost == null)
            {
                return;
            }

            var candidates = _grid.GetEmptyNeighbors(_state.CurrentTop, CurrentGhostPos);
            if (candidates.Count == 0)
            {
                _state.SetLose();
                return;
            }

            var next = candidates[Random.Range(0, candidates.Count)];
            _ghost.position = new Vector3(next.x, next.y, next.z);
        }
    }
}
