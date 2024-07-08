using _Scripts.Systems.ImageRecognition;
using UnityEngine;

namespace _Scripts.Systems.UI
{
    public class TrashController : MonoBehaviour, IDroppingHandler
    {
        public void HandleDrop(GameObject droppedGameObject)
        {
            ImageItemGenerator.Instance.ReleaseItem(droppedGameObject);
        }
    }
}