using System;
using _Scripts.Systems.Particles;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Systems.Puzzles
{
    public class BridgeListener : MonoBehaviour
    {
        [SerializeField] private QuestHandlerBase questHandlerToListen;
        [SerializeField] private GameObject sprite;
        private ParticlePool _particlePool;
        
        private float _initialY;
        
        private void Awake()
        {
            _particlePool = GetComponent<ParticlePool>();
            questHandlerToListen.QuestCompletedEvent += OnQuestCompleted;
            _initialY = transform.position.y;
            transform.position = new Vector3(transform.position.x, -10, transform.position.z);
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            questHandlerToListen.QuestCompletedEvent -= OnQuestCompleted;
        }

        private void OnQuestCompleted()
        {
            gameObject.SetActive(true);
            // play the effect 5 times in a row, randomly
            for (var i = 0; i < 5; i++) { _particlePool.PlayEffect(ParticleType.Smoke, transform.position + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0)); }
            var sequence = DOTween.Sequence();
            sequence.Join(transform.DOMoveY(_initialY, 3f).SetEase(Ease.InSine));
            sequence.Join(sprite.transform.DOShakePosition(3f, 0.1f, 10, 90, false));

        }
        
        
        
        
        
    }
}