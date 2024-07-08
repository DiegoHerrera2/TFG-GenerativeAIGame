using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Systems.Items
{
    [Serializable]
    public class ItemData : EntityData
    {
        public ItemData()
        {
            
        }
        public ItemData(string prompt, List<string> labels)
        {
            entityName = prompt;
            itemLabels = labels;
        }
        
        [SerializeField] private List<string> itemLabels;
        public List<string> ItemLabels
        {
            get => itemLabels;
            set => itemLabels = value;
        }
    }
}