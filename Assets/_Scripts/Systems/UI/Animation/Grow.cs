using DG.Tweening;
using UnityEngine;

namespace _Scripts.Systems.UI.Animation
{
    [CreateAssetMenu(fileName = "TweenAnimation", menuName = "TFG/Animation/Grow")]
    public class Grow : TweenAnimation
    {
        [SerializeField] private Vector3 initialScale;
        [SerializeField] private Vector3 targetScale;
        [SerializeField] private float duration;
        [SerializeField] private Ease ease;

        public override Tween PlayAnimation(GameObject target)
        {
            var targetTransform = target.transform;
            targetTransform.localScale = initialScale;
            return targetTransform.DOScale(targetScale, duration).SetEase(ease);
        }

        public override void CancelAnimation(GameObject target)
        {
            target.transform.DOKill();
            target.transform.localScale = initialScale;
        }
    }
}