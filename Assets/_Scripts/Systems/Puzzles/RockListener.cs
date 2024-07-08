using System;
using _Scripts.Systems.Particles;
using DG.Tweening;
using UnityEngine;

namespace _Scripts.Systems.Puzzles
{
    public class RockListener : MonoBehaviour
    {
        [SerializeField] private QuestHandlerBase questHandlerToListen;
        [SerializeField] private GameObject sprite;
        private ParticlePool _particlePool;
        
        private void Awake()
        {
            _particlePool = GetComponent<ParticlePool>();
            questHandlerToListen.QuestCompletedEvent += OnQuestCompleted;
        }

        private void OnDestroy()
        {
            questHandlerToListen.QuestCompletedEvent -= OnQuestCompleted;
        }

        private void OnQuestCompleted()
        {
            _particlePool.PlayEffect(ParticleType.Smoke, transform.position);
            var sequence = DOTween.Sequence();
            sequence.Join(transform.DOMoveY(-10, 3f).SetEase(Ease.InSine));
            sequence.Join(sprite.transform.DOShakePosition(3f, 0.1f, 10, 90, false));
            sequence.onComplete += () => gameObject.SetActive(false);
        }
        
        
        
        
        
    }
}