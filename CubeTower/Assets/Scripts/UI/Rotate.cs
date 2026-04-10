using Core;
using UnityEngine;

namespace CubeTower.UI
{
    public class Rotate : MainMono
    {
        public const float Speed = 10f;

        private Transform _rotate;

        private void Start()
        {
            _rotate = GetComponent<Transform>();
        }

        private void Update()
        {
            if (_rotate != null)
            {
                _rotate.Rotate(0, Speed * Time.deltaTime, 0);
            }
        }
    }
}
