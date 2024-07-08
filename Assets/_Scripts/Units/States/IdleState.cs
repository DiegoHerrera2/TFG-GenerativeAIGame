using UnityEngine;
using UnityHFSM;

namespace _Scripts.Units.States
{
    public class IdleState : State
    {
        public IdleState(Unit unitController)
        {
            _unitController = unitController;
        }

        private readonly Unit _unitController;
        private static readonly int RunSpeed = Animator.StringToHash("RunSpeed");

        public override void OnEnter()
        {
            _unitController.animator.SetFloat(RunSpeed, -1);
            _unitController.velocity.x = 0;
        }
        
        public override void OnLogic()
        {
            
        }
        
    }
}