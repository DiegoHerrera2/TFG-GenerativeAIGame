using System;
using Systems;
using Units;
using UnityEngine;
using UnityHFSM;

namespace _Scripts.Units.States
{
    public class MovementState : State
    {
        public MovementState(Unit unitController, UnitStats unitStats)
        {
            _unitController = unitController;
            _stats = unitStats;
        }

        private readonly Unit _unitController;
        private readonly UnitStats _stats;
        private static readonly int RunSpeed = Animator.StringToHash("RunSpeed");

        public override void OnLogic()
        {
            _unitController.yRotation = _unitController.Input.x switch
            {
                // Checking out x input to determine orientation
                > 0.15f => 0,
                < -0.15f => 180,
                _ => _unitController.yRotation
            };

            var targetX = _unitController.Input.x * _stats.moveSpeed;
            var acceleration = _unitController.CollidingBelow ? _stats.accelerationTimeGrounded : _stats.accelerationTimeAirborne;
            _unitController.velocity.x = Mathf.SmoothDamp(_unitController.velocity.x, targetX, ref _unitController.velocityXSmoothing, acceleration);
            _unitController.animator.SetFloat(RunSpeed, (Mathf.Abs(_unitController.velocity.x) / _stats.moveSpeed) / 2);
            
        }
    }
}