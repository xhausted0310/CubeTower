using CubeTower.Settings;
using UnityEngine;
using Zenject;

namespace CubeTower.Gameplay
{
    /// <summary>
    /// All camera behaviour used to live inline in GameController.Update /
    /// MoveCamera, and ExplodeCubes also poked <c>Camera.main.transform</c>
    /// directly on loss. This class consolidates every camera mutation so
    /// there is exactly one place that owns the camera transform.
    /// </summary>
    public class CameraController : ITickable
    {
        private readonly Transform _camTransform;
        private readonly Camera _camera;
        private readonly GameSettings _settings;

        private float _camMoveToY;
        private int _prevHorizontalTier;
        private Color _targetBgColor;

        public CameraController(Transform camTransform, GameSettings settings)
        {
            _camTransform = camTransform;
            _settings = settings;
            _camera = camTransform.GetComponent<Camera>();
            if (_camera == null)
            {
                _camera = Camera.main;
            }

            _camMoveToY = _camTransform.localPosition.y;
            if (_camera != null)
            {
                _targetBgColor = _camera.backgroundColor;
            }
        }

        public void Tick()
        {
            var local = _camTransform.localPosition;
            _camTransform.localPosition = Vector3.MoveTowards(
                local,
                new Vector3(local.x, _camMoveToY, local.z),
                _settings.CameraMoveSpeed * Time.deltaTime);

            if (_camera != null && _camera.backgroundColor != _targetBgColor)
            {
                _camera.backgroundColor = Color.Lerp(
                    _camera.backgroundColor,
                    _targetBgColor,
                    Time.deltaTime * _settings.BgColorLerpSpeed);
            }
        }

        /// <summary>
        /// Called by the game controller whenever a new cube is committed.
        /// Consolidates the Y follow update, horizontal nudge (L208-212 of the
        /// old GameController), and background-color tiering (L214-225).
        /// </summary>
        public void NotifyGridGrowth(int maxX, int maxY, int maxZ, int currentTopY)
        {
            _camMoveToY = _settings.CameraFollowYOffset + currentTopY - 1f;

            var maxHorizontal = maxX > maxZ ? maxX : maxZ;
            if (maxHorizontal > 0
                && maxHorizontal % _settings.HorizontalNudgeInterval == 0
                && _prevHorizontalTier != maxHorizontal)
            {
                _camTransform.localPosition -= new Vector3(0, 0, _settings.HorizontalNudgeInterval);
                _prevHorizontalTier = maxHorizontal;
            }

            SetBgColorForHeight(maxY);
        }

        public void NudgeBackOnLose()
        {
            _camTransform.position -= new Vector3(0, 0, _settings.LoseNudgeZ);
        }

        private void SetBgColorForHeight(int maxY)
        {
            var thresholds = _settings.BgColorYThresholds;
            var colors = _settings.BgColors;
            if (thresholds == null || colors == null)
            {
                return;
            }

            // Walk thresholds from highest to lowest, matching the old
            // GameController tier selection (L214-225).
            for (var i = thresholds.Length - 1; i >= 0; i--)
            {
                if (maxY >= thresholds[i] && i < colors.Length)
                {
                    _targetBgColor = colors[i];
                    return;
                }
            }
        }
    }
}
