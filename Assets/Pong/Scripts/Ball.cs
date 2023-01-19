using UnityEngine;

namespace Pong
{
    public class Ball : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private Rigidbody2D _rb;

        // Start is called before the first frame update
        private void Start()
        {
            Launch();
        }

        /// <summary>
        /// Launch the ball in a random X direction, Y will always be 1.
        /// </summary>
        public void Launch()
        {
            var x = Random.Range(0, 2) == 0 ? -1 : 1;
            _rb.velocity = new Vector2(_speed * x, _speed * 1);
        }
    }
}
