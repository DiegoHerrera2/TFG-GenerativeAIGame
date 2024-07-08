using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;


// Im not really sure if this will end up like a Monobehaviour, but let it be for now
namespace _Scripts.Systems.Items
{
    public class Inventory : MonoBehaviour
    {
        private List<ItemData> _items;
        
        public event Action<ItemData, int> ItemAdded;
        public event Action<ItemData, int> ItemRemoved;
        private void Awake()
        {
            _items = new List<ItemData>();
        }

        public bool AddItem(ItemData newItem)
        {
            if (_items.Contains(newItem)) return false;
            
            _items.Add(newItem);
            ItemAdded?.Invoke(newItem, _items.Count - 1);
            return true;
        }

        public ItemData GetItem(int index)
        {
            return index >= _items.Count ? null : _items[index];
        }

        public bool RemoveItem(int index)
        {
            if (index >= _items.Count) return false;
            var item = _items[index];
            _items.RemoveAt(index);
            ItemRemoved?.Invoke(item, index);
            return true;
        }

        public int Size()
        {
            return _items.Count;
        }
    }
}
