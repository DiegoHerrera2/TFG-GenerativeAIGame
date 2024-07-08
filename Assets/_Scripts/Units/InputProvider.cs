using System;
using UnityEngine;

namespace _Scripts.Units
{
    public abstract class InputProvider : MonoBehaviour
    {
        public Vector2 Movement { get; protected set; }

        public event Action JumpRequestedEvent;
        public event Action JumpReleasedEvent;
        
        

        protected void RequestJump()
        {
            JumpRequestedEvent?.Invoke();
        }
        
        protected void ReleaseJump()
        {
            JumpReleasedEvent?.Invoke();
        }
    }
}