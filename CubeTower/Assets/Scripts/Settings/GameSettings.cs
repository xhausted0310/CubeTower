using UnityEngine;

namespace CubeTower.Settings
{
    [CreateAssetMenu(menuName = "CubeTower/GameSettings", fileName = "GameSettings")]
    public class GameSettings : ScriptableObject
    {
        [Header("Ghost cube")]
        public float ChangePlaceSpeed = 0.5f;

        [Header("Camera")]
        public float CameraMoveSpeed = 2f;
        public float CameraFollowYOffset = 7.91f;
        public int HorizontalNudgeInterval = 3;
        public float LoseNudgeZ = 3f;
        public float BgColorLerpSpeed = 0.5f;

        [Header("Background colors")]
        public Color[] BgColors = new Color[3];
        public int[] BgColorYThresholds = new int[] { 2, 5, 7 };

        [Header("Lose detection")]
        public float LoseVelocityThreshold = 0.1f;

        [Header("Explosion")]
        public float ExplosionForce = 70f;
        public float ExplosionRadius = 5f;
    }
}
