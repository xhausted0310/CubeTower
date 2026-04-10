using System.Collections.Generic;
using UnityEngine;

namespace CubeTower.Gameplay
{
    /// <summary>
    /// Authoritative set of occupied cube grid positions. Seeded with the
    /// starting platform layout that used to live in GameController.
    /// </summary>
    public class GridState
    {
        private static readonly Vector3Int[] InitialPositions =
        {
            new Vector3Int(0, 0, 0),
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, 0, 1),
            new Vector3Int(0, 0, -1),
            new Vector3Int(1, 0, 1),
            new Vector3Int(-1, 0, -1),
            new Vector3Int(-1, 0, 1),
            new Vector3Int(1, 0, -1),
        };

        private static readonly Vector3Int[] NeighborOffsets =
        {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(0, 0, 1),
            new Vector3Int(0, 0, -1),
        };

        private readonly HashSet<Vector3Int> _occupied = new HashSet<Vector3Int>(InitialPositions);

        public bool IsOccupied(Vector3Int pos)
        {
            if (pos.y == 0)
            {
                return true;
            }

            return _occupied.Contains(pos);
        }

        public void Add(Vector3Int pos)
        {
            _occupied.Add(pos);
        }

        /// <summary>
        /// Returns empty neighbor positions of <paramref name="origin"/>, excluding
        /// the position currently occupied by the ghost cube (so the ghost never
        /// picks its own current cell).
        /// </summary>
        public List<Vector3Int> GetEmptyNeighbors(Vector3Int origin, Vector3Int exclude)
        {
            var result = new List<Vector3Int>(6);
            foreach (var offset in NeighborOffsets)
            {
                var candidate = origin + offset;
                if (candidate == exclude)
                {
                    continue;
                }

                if (IsOccupied(candidate))
                {
                    continue;
                }

                result.Add(candidate);
            }

            return result;
        }

        public void GetExtents(out int maxX, out int maxY, out int maxZ)
        {
            maxX = 0;
            maxY = 0;
            maxZ = 0;
            foreach (var pos in _occupied)
            {
                var ax = Mathf.Abs(pos.x);
                var ay = Mathf.Abs(pos.y);
                var az = Mathf.Abs(pos.z);
                if (ax > maxX) maxX = ax;
                if (ay > maxY) maxY = ay;
                if (az > maxZ) maxZ = az;
            }
        }
    }
}
