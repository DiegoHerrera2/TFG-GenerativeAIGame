using System;
using Units;
using UnityEngine;
using UnityHFSM;

namespace _Scripts.Units.States
{
    public class AirborneState : HybridStateMachine
    {
        public AirborneState(Unit unitController, UnitStats unitStats)
        {
            _unitController = unitController;
            _stats = unitStats;
        }
        private readonly Unit _unitController;
        private readonly UnitStats _stats;

        public override void OnLogic()
        {
            base.OnLogic();
            // When the jump button is released, we set the releaseTime to the bufferTime remaining, so if we pressed and released before
            // reaching ground but in the buffer time, we jump but with the minimum velocity
            if (_unitController.jumpReleased)
            {
                _unitController.releaseTime = _unitController.bufferTime;
                
                // If the unit is in the air and the velocity is greater than the minimum jump velocity, set it to the minimum jump velocity
                if (_unitController.velocity.y > _stats.MinJumpVelocity)
                    _unitController.velocity.y = _stats.MinJumpVelocity;
            }
            else _unitController.releaseTime = Math.Max(0f, _unitController.releaseTime - Time.deltaTime);
        }
    }
}