using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Systems.Particles;
using UnityEngine;

namespace _Scripts.Systems.Puzzles
{
    public abstract class QuestHandlerBase : MonoBehaviour
    {
        [SerializeField] private string questQuote;
        [SerializeField] private List<string> questTags;
        private ParticlePool _particlePool;
        public event Action QuestCompletedEvent;
        
        protected void Start()
        {
            _particlePool = GetComponent<ParticlePool>();
        }
        
        protected void CheckTagList(List<string> tagsToCheck)
        {
            if (questTags.Count == 0) return;
            var itemTagsLower = tagsToCheck.Select(tagToCheck => tagToCheck.ToLower()).ToList();
            
            if (!questTags.Any(tagToCheck => itemTagsLower.Contains(tagToCheck.ToLower())) && tagsToCheck.Count > 0) return;
            
            QuestCompletedEvent?.Invoke();
            _particlePool.PlayEffect(ParticleType.Confetti, transform.position);
            Destroy(this);
        }
        
        protected bool CheckTagListWithoutCompletion(List<string> tagsToCheck)
        {
            if (questTags.Count == 0) return false;
            var itemTagsLower = tagsToCheck.Select(tagToCheck => tagToCheck.ToLower()).ToList();
            
            return questTags.Any(tagToCheck => itemTagsLower.Contains(tagToCheck.ToLower()));
        }
        
        protected static bool CheckTagListWithoutCompletion(List<string> tagsToCheck, List<string> tagsToCheckAgainst)
        {
            if (tagsToCheckAgainst.Count == 0) return false;
            var itemTagsLower = tagsToCheckAgainst.Select(tagToCheck => tagToCheck.ToLower()).ToList();

            return tagsToCheck.Any(tagToCheck => itemTagsLower.Contains(tagToCheck.ToLower()));
        }

        protected void InvokeCompletion()
        {
            QuestCompletedEvent?.Invoke();
        }

        public string QuestQuote => questQuote;
        
        public void SetQuestTags(List<string> tags)
        {
            questTags = tags;
        }
    }
}
