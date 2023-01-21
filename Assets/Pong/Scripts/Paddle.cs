using UnityEngine;
using UnityEngine.InputSystem;

namespace Pong
{
    public class Paddle : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private Collider2D _collider;
        [SerializeField] private float _speed;
        private Vector2 _moveDirection;


        public Color PaddleColor
        {
            get => _spriteRenderer.color;
            set => _spriteRenderer.color = value;
        }
        public bool HasCollision
        {
            get => _collider.enabled;
            set
            {
                _collider.enabled = value;
                var c = _spriteRenderer.color;
                if (value)
                {
                    c.a = 1f;
                    _spriteRenderer.color = c;
                }
                else
                {
                    c.a = 0.5f;
                    _spriteRenderer.color = c;
                }
            }
        }

        private void FixedUpdate()
        {
            _rb.velocity = new Vector2(_moveDirection.x * _speed, 0f);
        }

        public void OnMove(InputAction.CallbackContext ctx) => _moveDirection = ctx.ReadValue<Vector2>();
    }
}
