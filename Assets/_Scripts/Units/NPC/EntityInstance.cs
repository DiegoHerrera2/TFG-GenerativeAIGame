using System;
using _Scripts.Systems.Items;
using UnityEngine;

namespace _Scripts.Units.NPC
{
    [Serializable]
    public class EntityInstance : MonoBehaviour, IEntity
    {
        [SerializeField] protected EntityData entityData;
        public SpriteRenderer SpriteRenderer { get; private set; }

        private void Awake()
        {
            var entityDataName = entityData.Name;
            SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            entityData = new EntityData(entityDataName, SpriteRenderer.sprite);
        }
        public EntityData EntityData
        {
            get => entityData;
            set
            {
                entityData = value;
                SpriteRenderer.sprite = entityData.Sprite;
            }
        }
        public string Name => entityData.Name;
        public Transform Transform => transform;
    }
}