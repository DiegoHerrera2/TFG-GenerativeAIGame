﻿using DG.Tweening;
using UnityEngine;

namespace _Scripts.Systems.UI.Animation

{
    [CreateAssetMenu(fileName = "TweenAnimation", menuName = "TFG/Animation/HoverOnEnterTween")]
    public class HoverOnEnterTween : TweenAnimation
    {
        [SerializeField] private float hoverScale = 1.1f;
        [SerializeField] private float hoverTime = 0.075f;

        public override Tween PlayAnimation(GameObject target)
        {
            var targetTransform = target.transform;
            return targetTransform.DOScale(hoverScale, hoverTime).SetEase(Ease.OutSine);
        }

        public override void CancelAnimation(GameObject target)
        {
            // Kill any tween that is currently running
            var targetTransform = target.transform;
            targetTransform.DOKill();
            targetTransform.localScale = Vector3.one;
        }
    }
}