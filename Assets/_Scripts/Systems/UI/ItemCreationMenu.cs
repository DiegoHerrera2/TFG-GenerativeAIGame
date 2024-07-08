using System;
using _Scripts.Systems.Items;
using UnityEngine;

namespace _Scripts.Systems.UI
{
    public class ItemCreationMenu : MonoBehaviour
    {
        public GameObject notebookButton;
        public GameObject trashIcon;

        private void Start()
        {
            Interactable.EntityDragged += OnEntityDragging;
            Interactable.EntityDropped += OnEntityDropping;
        }
        private void OnEntityDragging()
        {
            notebookButton.SetActive(false);
            trashIcon.SetActive(true);
        }
        private void OnEntityDropping()
        {
            notebookButton.SetActive(true);
            trashIcon.SetActive(false);
        }
    }
}
