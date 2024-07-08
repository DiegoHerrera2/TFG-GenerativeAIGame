using System;
using _Scripts.Units;
using DG.Tweening;
using UnityEngine;

namespace _Scripts.Systems.CameraControl
{
    public class CameraFollowObject : MonoBehaviour
    {
        [SerializeField] private Unit unit;
        [SerializeField] private float flipRotationTime;
        private Coroutine _turnCoroutine;
        private void Awake()
        {
            unit = GetComponentInParent<Unit>();
            
            unit.CameraTurn += CallTurn;
        }
        private void CallTurn(float yRotation)
        {
            transform.DORotate(new Vector3(0, yRotation, 0), flipRotationTime);
        }
    }
}
