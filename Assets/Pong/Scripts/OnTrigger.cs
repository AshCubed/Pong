using System;
using UnityEngine;
using UnityEngine.Events;

namespace Pong
{
    public enum OnContactType {ENTER, STAY, EXIT };
    
    [RequireComponent(typeof(Collider2D))]
    public class OnTrigger : MonoBehaviour
    {
        [SerializeField]public UnityEvent<Collider2D> OnTriggerEnterEvent;
        [SerializeField]public UnityEvent<Collider2D> OnTriggerStayEvent;
        [SerializeField]public UnityEvent<Collider2D> OnTriggerExitEvent;
        
        public UnityEvent<Collider2D, OnContactType> OnTriggerEvent { get; private set; }

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
            Collider.isTrigger = true;
            OnTriggerEvent = new UnityEvent<Collider2D, OnContactType>();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            OnTriggerEvent?.Invoke(col, OnContactType.ENTER);
            OnTriggerEnterEvent?.Invoke(col);
        }

        private void OnTriggerStay2D(Collider2D col)
        {
            OnTriggerEvent?.Invoke(col, OnContactType.STAY);
            OnTriggerStayEvent?.Invoke(col);
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            OnTriggerEvent?.Invoke(col, OnContactType.EXIT);
            OnTriggerExitEvent?.Invoke(col);
        }
    }
}
