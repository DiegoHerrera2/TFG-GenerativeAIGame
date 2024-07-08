using DG.Tweening;
using UnityEngine;

namespace _Scripts.Systems.UI.Animation
{
    [CreateAssetMenu(fileName = "TweenAnimation", menuName = "TFG/Animation/FadeSprite")]
    public class SpriteFade : TweenAnimation
    {
        [SerializeField] private float fadeTo = 0f;
        [SerializeField] private float duration = 1f;
        public override Tween PlayAnimation(GameObject target)
        {
            return target.GetComponent<SpriteRenderer>().DOFade(fadeTo, duration).SetEase(Ease.OutSine);
        }

        public override void CancelAnimation(GameObject target)
        {
            var targetSpriteRenderer = target.GetComponent<SpriteRenderer>();
            targetSpriteRenderer.DOKill();
            var color = targetSpriteRenderer.color;
            color = new Color(color.r, color.g, color.b, 1);
            targetSpriteRenderer.color = color;
        }
    }
}