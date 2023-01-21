using UnityEngine;
using UnityEngine.Events;

namespace Pong
{
    [RequireComponent(typeof(Collider2D))]
    public class OnCollision : MonoBehaviour
    {

        [SerializeField]public UnityEvent<Collision2D> OnCollisionEnterEvent;
        [SerializeField]public UnityEvent<Collision2D> OnCollisionStayEvent;
        [SerializeField]public UnityEvent<Collision2D> OnCollisionExitEvent;
        
        public UnityEvent<Collision2D, OnContactType> OnCollisionEvent { get; private set; }

        public Collider2D Collider
        {
            get
            {
                if (_collider)
                {
                    return _collider;
                }
                _collider = GetComponent<Collider2D>();
                return _collider;
            }
        }

        private Collider2D _collider;
        private void Awake()
        {
            Collider.isTrigger = false;
            OnCollisionEvent = new UnityEvent<Collision2D, OnContactType>();
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            OnCollisionEvent?.Invoke(col, OnContactType.ENTER);
            OnCollisionEnterEvent?.Invoke(col);
        }

        private void OnCollisionStay2D(Collision2D col)
        {
            OnCollisionEvent?.Invoke(col, OnContactType.STAY);
            OnCollisionStayEvent?.Invoke(col);
        }

        private void OnCollisionExit2D(Collision2D col)
        {
            OnCollisionEvent?.Invoke(col, OnContactType.EXIT);
            OnCollisionExitEvent?.Invoke(col);
        }
    }
}
