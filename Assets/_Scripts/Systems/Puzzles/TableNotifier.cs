using System;
using _Scripts.Systems.Items;
using UnityEngine;

namespace _Scripts.Systems.Puzzles
{
    public class TableNotifier : MonoBehaviour
    {
        public event Action<ItemData> ItemEntered;
        public event Action<ItemData> ItemExited;
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.TryGetComponent(out ItemInstance itemInstance))
            {
                ItemEntered?.Invoke(itemInstance.ItemData);
            }
        }
        
        private void OnTriggerExit2D(Collider2D col)
        {
            if (col.TryGetComponent(out ItemInstance itemInstance))
            {
                ItemExited?.Invoke(itemInstance.ItemData);
            }
        }
    }
}