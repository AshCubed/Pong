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

        /// <summary>
        /// Returns current Paddle colour, sets paddle sprite renderer colour
        /// </summary>
        public Color PaddleColor
        {
            get => _spriteRenderer.color;
            set => _spriteRenderer.color = value;
        }
        /// <summary>
        /// Adjusts alpha colour value of paddle based on if it has collision, if true then a = 0.5f else 1.0f
        /// </summary>
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
        /// <summary>
        /// Controls if this paddle can move
        /// </summary>
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

        /// <summary>
        /// Called on Player Input component
        /// </summary>
        /// <param name="ctx"></param>
        public void OnMove(InputAction.CallbackContext ctx) => _moveDirection = ctx.ReadValue<Vector2>();
    }
}
