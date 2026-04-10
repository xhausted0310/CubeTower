using CubeTower.Gameplay.Signals;
using UnityEngine;
using Zenject;

namespace CubeTower.Gameplay
{
    /// <summary>
    /// Single source of truth for the run's high-level state. Before the refactor
    /// the "lose" flag lived on GameController while ExplodeCubes had its own
    /// parallel path — this class merges them so both call sites route through
    /// <see cref="SetLose"/> and the LoseSignal fires exactly once.
    /// </summary>
    public class GameState
    {
        private readonly SignalBus _bus;

        public GameState(SignalBus bus)
        {
            _bus = bus;
        }

        public bool IsLose { get; private set; }
        public bool FirstCube { get; private set; }
        public Vector3Int CurrentTop { get; private set; } = new Vector3Int(0, 1, 0);

        public void SetCurrentTop(Vector3Int pos)
        {
            CurrentTop = pos;
        }

        public void MarkFirstCube()
        {
            if (FirstCube)
            {
                return;
            }

            FirstCube = true;
            _bus.Fire(new FirstCubeSignal());
        }

        public void SetLose()
        {
            if (IsLose)
            {
                return;
            }

            IsLose = true;
            _bus.Fire(new LoseSignal());
        }
    }
}
