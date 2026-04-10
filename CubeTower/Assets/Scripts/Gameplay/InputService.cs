using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace CubeTower.Gameplay
{
    /// <summary>
    /// Hides the editor-vs-device tap handling difference. Mirrors the exact
    /// logic from the old <c>GameController.Update</c> (L57-67), including the
    /// <c>TouchPhase.Began</c> gate that is only active on-device.
    /// </summary>
    public class InputService : ITickable
    {
        private bool _tapThisFrame;

        public bool TryConsumeTap()
        {
            if (!_tapThisFrame)
            {
                return false;
            }

            _tapThisFrame = false;
            return true;
        }

        public void Tick()
        {
            _tapThisFrame = false;

            if (!(Input.GetMouseButtonDown(0) || Input.touchCount > 0))
            {
                return;
            }

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

#if !UNITY_EDITOR
            if (Input.touchCount == 0 || Input.GetTouch(0).phase != TouchPhase.Began)
            {
                return;
            }
#endif

            _tapThisFrame = true;
        }
    }
}
