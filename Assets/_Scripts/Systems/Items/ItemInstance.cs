using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Systems.ImageRecognition;
using UnityEngine;
using UnityHFSM;

namespace _Scripts.Systems.Items
{
    [Serializable]
    public class ItemInstance : MonoBehaviour, IEntity
    {
        [SerializeField] private ItemData itemData;
        public SpriteRenderer SpriteRenderer { get; private set; }
        private void Awake()
        {
            SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        public ItemData ItemData
        {
            get => itemData;
            set
            {
                itemData = value;
                SpriteRenderer.sprite = itemData.Sprite;
            }
        }
        public List<string> ItemLabels => itemData.ItemLabels;
        public string Name => itemData.Name;

        public Transform Transform => transform;
    }
}