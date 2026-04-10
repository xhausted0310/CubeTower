using Core;
using CubeTower.Settings;
using UnityEngine;
using Zenject;

namespace CubeTower.Gameplay
{
    /// <summary>
    /// Reacts to the tower's cube parent falling onto the ground collider.
    /// Still a MonoBehaviour (it needs OnCollisionEnter on a scene object),
    /// but now routes the loss state and camera nudge through injected
    /// services instead of touching <c>Camera.main</c> directly.
    /// </summary>
    public class ExplodeCubes : MainMono
    {
        [SerializeField] private GameObject restartButton;

        [Inject] private GameState _state;
        [Inject] private CameraController _camera;
        [Inject] private GameSettings _settings;

        private bool _collisionHandled;

        private void OnCollisionEnter(Collision collision)
        {
            if (_collisionHandled)
            {
                return;
            }

            if (!collision.gameObject.CompareTag("Cube"))
            {
                return;
            }

            _collisionHandled = true;

            var root = collision.gameObject.transform;
            for (var i = root.childCount - 1; i >= 0; i--)
            {
                var child = root.GetChild(i);
                var rb = child.gameObject.AddComponent<Rigidbody>();
                rb.AddExplosionForce(_settings.ExplosionForce, Vector3.up, _settings.ExplosionRadius);
                child.SetParent(null);
            }

            if (restartButton != null)
            {
                restartButton.SetActive(true);
            }

            _camera.NudgeBackOnLose();
            _state.SetLose();

            Destroy(collision.gameObject);
        }
    }
}
