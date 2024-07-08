using System;
using DG.Tweening;
using UnityEngine;

namespace _Scripts.Systems.UI.Animation

{
    [CreateAssetMenu(fileName = "TweenAnimation", menuName = "TFG/Animation/FadeCanvas")]
    public class FadeCanvas : TweenAnimation
    {
        [SerializeField] private float fadeTo = 0f;
        [SerializeField] private float duration = 1f;
        public override Tween PlayAnimation(GameObject target)
        {
            var targetCanvasGroup = target.GetComponent<CanvasGroup>();
            return targetCanvasGroup.DOFade(fadeTo, duration).SetEase(Ease.OutSine);
        }

        public override void CancelAnimation(GameObject target)
        {
            var targetCanvasGroup = target.GetComponent<CanvasGroup>();
            targetCanvasGroup.DOKill();
            targetCanvasGroup.alpha = 1;
        }
    }
}