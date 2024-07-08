using DG.Tweening;
using UnityEngine;

namespace _Scripts.Systems.UI.Animation
{
    [CreateAssetMenu(fileName = "TweenAnimation", menuName = "TFG/Animation/ClockAnimation")]
    public class ClockAnimation : TweenAnimation
    {
        private Sequence _sequence;
        
        public override Tween PlayAnimation(GameObject target)
        {
            var targetTransform = target.transform;
            _sequence = DOTween.Sequence();
            // Like a tick tack animation. Tick is tilting to the left, tack is tilting to the right.
            _sequence.Append(targetTransform.DOPunchRotation(new Vector3(0, 0, 3), 0.5f, 15, 6));
            _sequence.Append(targetTransform.DOPunchRotation(new Vector3(0, 0, -3), 0.5f, 15, 6));
            _sequence.SetLoops(-1);
            
            return _sequence;
        }

        public override void CancelAnimation(GameObject target)
        {
            var targetTransform = target.transform;
            _sequence.Kill();
            targetTransform.rotation = Quaternion.identity;
        }
    }
}
