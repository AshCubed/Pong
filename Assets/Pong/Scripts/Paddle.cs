using UnityEngine;
using UnityEngine.InputSystem;

namespace Pong
{
    public class Paddle : MonoBehaviour
    {
        [SerializeField] private bool _isPlayer1;
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private float _speed;
        private InputAction _move;
        private Vector2 _moveDirection;
        private PongInputActions _pongInputActions;

        private void Awake()
        {
            _pongInputActions = new PongInputActions();
        }

        // Update is called once per frame
        private void Update()
        {
            _moveDirection = _move.ReadValue<Vector2>();
        }

        private void FixedUpdate()
        {
            _rb.velocity = new Vector2(_moveDirection.x * _speed, 0f);
        }

        private void OnEnable()
        {
            _move = _isPlayer1 ? _pongInputActions.Player1.Move : _pongInputActions.Player2.Move;
            _move.Enable();
        }
        
        private void OnDisable()
        {
            _move.Disable();
        }
    }
}
