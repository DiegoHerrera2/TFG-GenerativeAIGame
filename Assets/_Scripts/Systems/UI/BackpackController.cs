using _Scripts.Systems.ImageRecognition;
using _Scripts.Systems.Items;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Systems.UI
{
    public class BackpackController : MonoBehaviour, IDroppingHandler
    {
        [SerializeField] private Inventory inventory;
        public void HandleDrop(GameObject droppedGameObject)
        {
            var result = inventory.AddItem(droppedGameObject.GetComponent<ItemInstance>().ItemData);
            if (result) ImageItemGenerator.Instance.ReleaseItem(droppedGameObject);
        }
    }
}
