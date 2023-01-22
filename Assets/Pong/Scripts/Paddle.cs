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
        private bool _canMove;

        public Color PaddleColor
        {
            get => _spriteRenderer.color;
            set => _spriteRenderer.color = value;
        }
        public bool HasCollision
        {
            set
            {
                //_collider.enabled = value;
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
        public bool CanMove
        {
            set
            {
                _canMove = value;
                _rb.velocity = Vector2.zero;
            }
        }

        private void FixedUpdate()
        {
            if (_canMove)
            {
                _rb.velocity = new Vector2(_moveDirection.x * _speed, 0f);
            }
        }

        public void OnMove(InputAction.CallbackContext ctx) => _moveDirection = ctx.ReadValue<Vector2>();
    }
}
