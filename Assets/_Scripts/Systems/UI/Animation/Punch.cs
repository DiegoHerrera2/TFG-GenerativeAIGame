using System;
using DG.Tweening;
using UnityEngine;

namespace _Scripts.Systems.UI.Animation

{
    [CreateAssetMenu(fileName = "TweenAnimation", menuName = "TFG/Animation/Punch")]
    public class Punch : TweenAnimation
    {
        [SerializeField] private float punchScale = 1.2f;
        [SerializeField] private float duration = 1f;
        [SerializeField] private int vibrato = 1;
        [SerializeField] private float elasticity = 0.5f;

        public override Tween PlayAnimation(GameObject target)
        {
            var targetTransform = target.transform;
            return targetTransform.DOPunchScale(new Vector3(punchScale, punchScale, punchScale), duration, vibrato, elasticity).SetEase(Ease.OutSine);
        }

        public override void CancelAnimation(GameObject target)
        {
            var targetTransform = target.transform;
            targetTransform.DOKill();
            targetTransform.localScale = Vector3.one;
        }
    }
}