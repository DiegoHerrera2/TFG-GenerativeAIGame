using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Systems.Items
{
    [Serializable]
    public class EntityData
    {
        [SerializeField] protected string entityName;
        protected Sprite EntitySprite;

        protected EntityData()
        {
            
        }

        protected EntityData(string name)
        {
            entityName = name;
        }
        
        public EntityData(string name, Sprite entitySprite)
        {
            entityName = name;
            EntitySprite = entitySprite;
        }
        
        public string Name
        {
            get => entityName;
            set => entityName = value;
        }
        public Sprite Sprite
        {
            get => EntitySprite;
            set => EntitySprite = value;
        }
        
    }
}