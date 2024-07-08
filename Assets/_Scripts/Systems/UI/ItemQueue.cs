using System.Collections.Generic;
using _Scripts.Systems.ImageRecognition;
using _Scripts.Systems.UI.Animation;
using UnityEngine;

namespace _Scripts.Systems.UI
{
    public class ItemQueue : MonoBehaviour
    {
        [SerializeField] private List<GameObject> clocks = new List<GameObject>();
        [SerializeField] private InputWindow inputWindow;
        [SerializeField] private TweenAnimation clockAnimation;
        private int _index = 0;

        private void Awake()
        {
            ImageItemGenerator.Instance.ItemGenerated += OnItemGenerated;
            ImageItemGenerator.Instance.ItemGenerationFailed += OnItemGenerated;
            inputWindow.PromptEntered += OnPromptEntered;
        }
    
        private void OnItemGenerated()
        {
            _index--;
            clockAnimation.CancelAnimation(clocks[_index]);
            clocks[_index].SetActive(false);
        }
    
        private void OnPromptEntered(string prompt)
        {
            if (_index >= clocks.Count) return;
            clocks[_index].SetActive(true);
            clockAnimation.PlayAnimation(clocks[_index]);
            _index++;
        }
    
    }
}
