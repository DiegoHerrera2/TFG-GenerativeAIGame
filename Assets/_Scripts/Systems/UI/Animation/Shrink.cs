using DG.Tweening;
using UnityEngine;

namespace _Scripts.Systems.UI.Animation
{
    [CreateAssetMenu(fileName = "TweenAnimation", menuName = "TFG/Animation/Shrink")]
    public class Shrink : TweenAnimation
    {
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private float initialScale = 1;
        [SerializeField] private float finalScale = 0;
        [SerializeField] private Ease ease = Ease.OutSine;

        public override Tween PlayAnimation(GameObject target)
        {
            var targetTransform = target.transform;
            return targetTransform.DOScale(finalScale, duration).From(initialScale).SetEase(ease);
        }

        public override void CancelAnimation(GameObject target)
        {
            var targetTransform = target.transform;
            targetTransform.DOKill();
            targetTransform.DOScale(finalScale, 0f);
        }
    }
}