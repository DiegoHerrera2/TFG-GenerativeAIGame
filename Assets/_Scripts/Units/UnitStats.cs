using System;
using Systems;
using UnityEngine;

namespace Units
{
    [CreateAssetMenu(menuName = "TFG/PlayerStats")]
    public class UnitStats : ScriptableObject
    {
        [Header("Movement")] 
        public float moveSpeed;
        public float accelerationTimeAirborne = .2f;
        public float accelerationTimeGrounded = .1f;
        
        [NonSerialized]
        public float Gravity;
        [NonSerialized]
        public float MaxJumpVelocity;
        [NonSerialized]
        public float MinJumpVelocity;

        [Header("Jump")]
        public float bufferTime = 5f;
        public float coyoteTime = 3f;
        public float maxJumpHeight = 4;
        public float minJumpHeight = 1;
        public float timeToJumpApex = .4f;

        [Header("Layers")] 
        public LayerMask interactionLayerMask;
        
        

        public void UpdateParameters()
        {
            Gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
            MaxJumpVelocity = Mathf.Abs(Gravity) * timeToJumpApex;
            MinJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(Gravity) * minJumpHeight);
        }
    }
}
