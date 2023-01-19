using UnityEngine;
using Random = UnityEngine.Random;

namespace Pong
{
    public class Ball : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private TrailRenderer _trailRenderer;
        [SerializeField] private float _speed;
        [SerializeField] private Rigidbody2D _rb;
        
        public Color BallColor
        {
            set
            {
                _spriteRenderer.color = value;
                _trailRenderer.startColor = value;
                _trailRenderer.endColor = value;
            }
        }
        public Paddle LastPlayerToHit { get; private set; }

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
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
            var x = Random.Range(0, 2) == 0 ? -1 : 1;
            _rb.velocity = new Vector2(_speed * x, _speed * 1);
        }

        /// <summary>
        /// Stops all movement and sets game object to false.
        /// </summary>
        public void Stop()
        {
            _rb.velocity = Vector2.zero;
            gameObject.SetActive(false);
        }

        public void ResetLastPlayer(Color defaultColor)
        {
            BallColor = defaultColor;
            LastPlayerToHit = null;
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.collider.CompareTag("Player"))
            {
                LastPlayerToHit = col.gameObject.GetComponent<Paddle>();
            }
        }
    }
}
