using _Scripts.Systems.Items;
using UnityEngine;

namespace _Scripts.Systems.Puzzles
{
    public class ProximityQuest : QuestHandlerBase
    {
        private bool _questCompleted = false;
        private CircleCollider2D _circleCollider2D;

        private void Awake()
        {
            _circleCollider2D = GetComponent<CircleCollider2D>();
            QuestCompletedEvent += () =>
            {
                _questCompleted = true;
                _circleCollider2D.enabled = false;
            };
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (_questCompleted) return;
            
            var item = other.GetComponent<ItemInstance>();
            if (item == null) return;
            CheckTagList(item.ItemLabels);
        }

    }
}
