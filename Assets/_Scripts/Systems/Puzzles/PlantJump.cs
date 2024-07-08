using _Scripts.Systems.Items;
using _Scripts.Units;
using DG.Tweening;
using Systems;
using UnityEngine;

namespace _Scripts.Systems.Puzzles
{
    public class PlantJump : MonoBehaviour
    {
        private bool _isActive = false;
        private QuestHandlerBase _questHandler;
        private Animator _animator;
        private static readonly int Jump = Animator.StringToHash("Jump");
        private BoxCollider2D _boxCollider2D;
        [SerializeField] private AudioCueData jumpSound;
        private void Awake()
        {
            _questHandler = GetComponentInChildren<QuestHandlerBase>();
            _animator = GetComponent<Animator>();
            if (_questHandler == null)
            {
                Debug.LogError("QuestHandlerBase not found in parent");
            }
        
            _questHandler.QuestCompletedEvent += () =>
            {
                transform.DOScale(new Vector3(0.25f,0.25f,0.25f), 1f).OnComplete(() => _isActive = true);
            };
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_isActive) return;
            var unit = other.GetComponent<Unit>();
            unit.velocity.y = 50;
            _animator.SetTrigger(Jump);
            AudioManager.PlayWithPitchRange(jumpSound, transform.position, new Vector2(0.9f, 1.1f));
        }
    }
}
