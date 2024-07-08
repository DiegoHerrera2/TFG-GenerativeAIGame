using System;
using _Scripts.Systems.Particles;
using UnityEngine;

namespace _Scripts.Systems.Puzzles
{
    public class MultipleQuest : QuestHandlerBase
    {
        private QuestHandlerBase[] _questHandlers;
        private int _completedQuests;

        private new void Start()
        {
            base.Start();
            _questHandlers = GetComponentsInChildren<QuestHandlerBase>();
            foreach (var questHandler in _questHandlers)
            {
                if (questHandler == this) continue;
                questHandler.QuestCompletedEvent += CheckQuests;
            }
        
        }

        //Once all the quest handlers have been completed, the multiple quest is completed
        
        // Check it after every QuestCompletedEvent from the list
        
        private void CheckQuests()
        {
            _completedQuests++;
            if (_completedQuests != _questHandlers.Length - 1) return;
            InvokeCompletion();
            Destroy(this);
        }
    }
}