using UnityEngine;

namespace CubeTower.Gameplay.Signals
{
    public class CubePlacedSignal
    {
        public Vector3Int Position { get; }

        public CubePlacedSignal(Vector3Int position)
        {
            Position = position;
        }
    }
}
