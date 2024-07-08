using _Scripts.Systems.UI.Animation;
using UnityEngine;

namespace _Scripts.Systems.Puzzles
{
    public class QuestMark : MonoBehaviour
    {
        private QuestHandlerBase _questHandler;
        [SerializeField] private TweenSequence exitSequence;

        private void Start()
        {
            _questHandler = GetComponentInParent<QuestHandlerBase>();
            if (_questHandler == null)
            {
                Debug.LogError("QuestHandlerBase not found in parent");
            }
        
            _questHandler.QuestCompletedEvent += () =>
            {
                exitSequence.Restart();
            };
        
            exitSequence.Init(gameObject);
        
            exitSequence.OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }
    
    }
}
