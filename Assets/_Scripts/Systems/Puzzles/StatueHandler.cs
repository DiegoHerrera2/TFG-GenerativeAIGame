using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace _Scripts.Systems.Puzzles
{
    public class StatueHandler : MonoBehaviour
    {
        private Light2D _light;
        private QuestHandlerBase _questHandler;
        
        private void Awake()
        {
            _light = GetComponentInChildren<Light2D>();
            _questHandler = GetComponent<QuestHandlerBase>();
            
            _questHandler.QuestCompletedEvent += () =>
            {
                // Tween the light from 0 to 1 intensity
                StartCoroutine(LightUp());
            };
        }

        private IEnumerator LightUp()
        {
            while (_light.intensity < 0.15f)
            {
                _light.intensity += Time.deltaTime;
                yield return null;
            }
        }
        
    }
}