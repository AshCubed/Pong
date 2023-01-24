using UnityEngine;
using Random = UnityEngine.Random;

namespace Pong
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class AsteroidRotate : MonoBehaviour
    {
        [Tooltip("Check true if you want the spawned items to be given random forces to keep them moving")]
        [SerializeField] private bool _shouldRandomForceInGame;
        [SerializeField] private float _waitTime = 10f;
        [SerializeField] private int _force = 10;
        private Rigidbody2D _rigidbody;
        private float _currentTime;

        // Start is called before the first frame update
        private void Start()
        {
            _currentTime = 0f;
            _rigidbody = GetComponent<Rigidbody2D>();
            _rigidbody.gravityScale = 0;
            ProduceRandomForceAndTorque();
        }

        private void Update()
        {
            if (!_shouldRandomForceInGame) return;
            if (!gameObject.activeSelf) return;
            _currentTime += Time.deltaTime;
            if (!(_currentTime >= _waitTime)) return;
            ProduceRandomForceAndTorque();
            _currentTime = 0f;
        }

        /// <summary>
        /// Adds a random Force and Torque to this rigidbody using <see cref="_force"/>
        /// </summary>
        private void ProduceRandomForceAndTorque()
        {
            var num = Random.Range(0, 2);
            switch (num)
            {
                case 0:
                    _rigidbody.AddForce(new Vector2(_force, 0));
                    _rigidbody.AddTorque(_force);
                    break;
                case 1:
                    _rigidbody.AddForce(new Vector2(0, _force));
                    _rigidbody.AddTorque(_force);
                    break;
            }
        }
    }
}
