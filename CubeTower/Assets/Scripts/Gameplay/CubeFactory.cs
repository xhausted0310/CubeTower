using UnityEngine;
using Zenject;

namespace CubeTower.Gameplay
{
    /// <summary>
    /// Plain C# factory that instantiates the cube prefab at a requested world
    /// position via Zenject's <see cref="DiContainer.InstantiatePrefab"/>. We
    /// deliberately avoid <c>PlaceholderFactory</c> + <c>FromComponentInNewPrefab</c>
    /// here: that pattern requires the target component to accept the spawn
    /// parameter through an <c>[Inject]</c> method, which doesn't work with
    /// non-MonoBehaviour components like <see cref="Transform"/>.
    /// </summary>
    public class CubeFactory
    {
        private readonly DiContainer _container;
        private readonly GameObject _prefab;
        private readonly Transform _parent;

        public CubeFactory(DiContainer container, GameObject prefab, Transform parent)
        {
            _container = container;
            _prefab = prefab;
            _parent = parent;
        }

        public Transform Create(Vector3 position)
        {
            var go = _container.InstantiatePrefab(_prefab, position, Quaternion.identity, _parent);
            return go.transform;
        }
    }
}
