using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Systems.Items;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Systems.UI
{
    public class InventoryUI : Page
    {
        public List<GameObject> itemImages;
        public Inventory playerInventory;

        private void Awake()
        {
            playerInventory.ItemAdded += SetItem;
            playerInventory.ItemRemoved += RemoveItem;
            
            enterSequence.Init();
            exitSequence.Init();
            
            exitSequence.OnComplete(() => gameObject.SetActive(false));

            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            playerInventory.ItemAdded -= SetItem;
            playerInventory.ItemRemoved -= RemoveItem;
            
        }

        private void SetItem(ItemData item, int index)
        {
            itemImages[index].SetActive(true);
            itemImages[index].GetComponent<Image>().sprite = item.Sprite;
            itemImages[index].GetComponent<Image>().color = Color.white;
            itemImages[index].GetComponent<UI_SavedItemHover>().itemData = item;
            itemImages[index].GetComponent<UI_SavedItemHover>().index = index;
        }
        
        private void RemoveItem(ItemData item, int index)
        {
            itemImages[index].GetComponent<ButtonBehaviour>().IsActivated = false;
            // Animation to remove the item
            itemImages[index].GetComponent<Transform>().DOScale(Vector3.zero, 0.25f).SetEase(Ease.InOutSine).SetUpdate(true).OnComplete(() =>
            {
                // If the item is not the last one, we need to shift all the items to the left
                if (index < playerInventory.Size())
                {
                    for (var i = index; i < playerInventory.Size(); i++)
                    {
                        itemImages[i].GetComponent<Image>().sprite = itemImages[i + 1].GetComponent<Image>().sprite;
                        itemImages[i].GetComponent<UI_SavedItemHover>().itemData = itemImages[i + 1].GetComponent<UI_SavedItemHover>().itemData;
                    }
                }
                // deactivate the last item
                itemImages[playerInventory.Size()].SetActive(false);
                itemImages[index].GetComponent<Transform>().localScale = Vector3.one;
                itemImages[index].GetComponent<ButtonBehaviour>().IsActivated = true;
            });
        }
        
        protected override void Open()
        {
            gameObject.SetActive(true);
            exitSequence.Rewind();
            enterSequence.Restart();
            
        }

        protected override void Close()
        {
            enterSequence.Rewind();
            exitSequence.Restart();
        }
    }
}
