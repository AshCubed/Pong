using System;
using Pong.Audio;
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
        [SerializeField] private ParticleSystem _hitParticleSystem;
        [SerializeField] private LayerMask _layerWall;
        [SerializeField] private WallAngleHit[] _wallAngleHits;

        [Serializable]
        private struct WallAngleHit
        {
            public GameObject Wall;
            public float AngleZ;
            public bool ShouldCopyColor;
        }
        
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
            AudioManager.Instance.PlayOneShot("BallLaunch");
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
            if (col.collider.CompareTag(Constants.Tags.PLAYER))
            {
                AudioManager.Instance.PlayOneShot("BallHit");
                LastPlayerToHit = col.gameObject.GetComponent<Paddle>();
            }

            if ((_layerWall.value & (1 << col.gameObject.layer)) > 0) 
            {
                foreach (var item in _wallAngleHits)
                {
                    if (col.gameObject == item.Wall)
                    {
                        var contactPoint = col.GetContact(0).point;
                        var spawnPoint = new Vector3(contactPoint.x, contactPoint.y, 0f);
                        var particle = Instantiate(_hitParticleSystem, spawnPoint, new Quaternion(0, 0, item.AngleZ, 0));
                        if (LastPlayerToHit && item.ShouldCopyColor)
                        {
                            var main = particle.main;
                            main.startColor = LastPlayerToHit.PaddleColor;
                        }
                        break;
                    }
                }
            }
        }
    }
}
