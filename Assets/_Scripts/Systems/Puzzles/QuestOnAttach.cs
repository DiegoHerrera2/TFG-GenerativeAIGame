using _Scripts.Systems.Items;
using UnityEngine;

namespace _Scripts.Systems.Puzzles
{
    [RequireComponent(typeof(Interactable))]
    public class QuestOnAttach : QuestHandlerBase
    {
        private Interactable interactable;
        private void Awake()
        {
            interactable = GetComponent<Interactable>();
            interactable.ItemAttached += CheckQuestCompletion;
        }
        
        private void CheckQuestCompletion(ItemData itemData)
        {
            CheckTagList(itemData.ItemLabels);
            Debug.Log("Checking quest completion");
        }
        
    }
}