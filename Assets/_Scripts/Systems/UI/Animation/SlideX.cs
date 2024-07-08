using DG.Tweening;
using UnityEngine;

namespace _Scripts.Systems.UI.Animation
{
    [CreateAssetMenu(fileName = "TweenAnimation", menuName = "TFG/Animation/SlideX")]
    public class SlideX : TweenAnimation
    {
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private float initialPosition = -1000;
        [SerializeField] private float finalPosition = 0;
        public override Tween PlayAnimation(GameObject target)
        {
            var targetTransform = target.transform;
            return targetTransform.DOLocalMoveX(finalPosition, duration).From(initialPosition).SetEase(Ease.OutSine);
        }

        public override void CancelAnimation(GameObject target)
        {
            var targetTransform = target.transform;
            targetTransform.DOKill();
            targetTransform.DOLocalMoveX(finalPosition, 0f);
        }
    }
}