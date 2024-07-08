using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Systems.UI.Animation

{
    [CreateAssetMenu(fileName = "TweenAnimation", menuName = "TFG/Animation/OnClickTween")]
    public class OnClickTween : TweenAnimation
    {
        [SerializeField] private Vector3 clickScale = new(1.1f, 1.1f, 1.1f);
        [SerializeField] private float clickTime = 0.2f;
        [SerializeField] private int vibrato = 5;
        [SerializeField] private float elasticity = 0.3f;
        
        [SerializeField] private float recoverTime = 0.05f;
        [SerializeField] private float recoverScale = 1.1f;
        
        [SerializeField] private Color clickColor = new (1, 1, 1, 0.5f);
        [SerializeField] private Color recoverColor = new (1, 1, 1, 1);

        private Sequence _sequence;
        

        public override Tween PlayAnimation(GameObject target)
        {
            _sequence = DOTween.Sequence();
            
            var targetTransform = target.transform;
            var targetImage = target.GetComponent<Image>();
            
            int[] rotation = { Random.Range(-10, 7), Random.Range(7, 10) };

            if (targetImage != null)
            {
                // Color
                _sequence.Append(targetImage.DOColor(clickColor, 0.075f));
                _sequence.Append(targetImage.DOColor(recoverColor, recoverTime));
            }

            // Animation
            _sequence.Join(targetTransform.DOPunchRotation(new Vector3(0, 0, rotation[Random.Range(0, 2)]), clickTime, vibrato, elasticity));
            _sequence.Join(targetTransform.DOPunchScale(clickScale, clickTime, vibrato, elasticity));
            
            // Recover from animation
            _sequence.Append(targetTransform.DOScale(recoverScale, recoverTime));
            _sequence.Join(targetTransform.DORotate(Vector3.zero, recoverTime));


            return _sequence;
        }
        
        public override void CancelAnimation(GameObject target)
        {
            var targetTransform = target.transform;
            _sequence.Kill();
            targetTransform.localScale = Vector3.one;
            targetTransform.rotation = Quaternion.identity;
        }
        
    }
}