using UnityEngine;

namespace CubeTower.Gameplay
{
    /// <summary>
    /// Thin wrapper that couples the Zenject <see cref="CubeFactory"/> with
    /// <see cref="GridState"/> so callers don't have to update both sides.
    /// </summary>
    public class CubeSpawner
    {
        private readonly CubeFactory _factory;
        private readonly GridState _grid;

        public CubeSpawner(CubeFactory factory, GridState grid)
        {
            _factory = factory;
            _grid = grid;
        }

        public Transform Spawn(Vector3Int pos)
        {
            var cube = _factory.Create(pos);
            _grid.Add(pos);
            return cube;
        }
    }
}
