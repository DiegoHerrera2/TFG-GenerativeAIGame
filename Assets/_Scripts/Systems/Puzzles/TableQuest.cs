using System.Collections.Generic;
using _Scripts.Systems.Items;
using _Scripts.Systems.Particles;
using UnityEngine;

namespace _Scripts.Systems.Puzzles
{
    public class TableQuest : QuestHandlerBase
    {

        [SerializeField] private TableNotifier tableNotifier;
        private ParticlePool _particlePool;

        [SerializeField] private List<string> breadTags = new List<string> { "bread" };
        [SerializeField] private List<string> ingredientTags = new List<string> { "food" };

        private List<string> _breadsNames = new List<string>();
        private List<string> _ingredientsNames = new List<string>();

        private void Awake()
        {
            _particlePool = GetComponent<ParticlePool>();
            tableNotifier.ItemEntered += CheckInsertingItem;
            tableNotifier.ItemExited += CheckExitingItem;
        }

        private void CheckInsertingItem(ItemData itemData)
        {
            if (CheckTagListWithoutCompletion(itemData.ItemLabels, breadTags))
            {
                _breadsNames.Add(itemData.Name);
            }
            else if (CheckTagListWithoutCompletion(itemData.ItemLabels, ingredientTags))
            {
                _ingredientsNames.Add(itemData.Name);
            }

            if (_breadsNames.Count >= 1 && _ingredientsNames.Count >= 1)
            {
                InvokeCompletion();
                _particlePool.PlayEffect(ParticleType.Confetti, transform.position);
                Destroy(this);
                Destroy(tableNotifier);
            }
            else CheckTagList(itemData.ItemLabels);
        }

        private void CheckExitingItem(ItemData itemData)
        {
            if (_breadsNames.Count == 0 && _ingredientsNames.Count == 0) return;
            
            if (_breadsNames.Contains(itemData.Name))
            {
                _breadsNames.Remove(itemData.Name);
            }
            else if (_ingredientsNames.Contains(itemData.Name))
            {
                _ingredientsNames.Remove(itemData.Name);
            }
        }
    }
}