using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Systems
{
    public class SceneChangeListener : MonoBehaviour
    {
        
        [SerializeField] private CanvasGroup fadeCanvasGroup;
        // Start is called before the first frame update
        private void OnEnable()
        {
         SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log("Scene loaded: " + scene.name);
            fadeCanvasGroup.alpha = 1;
            fadeCanvasGroup.DOFade(0, 1f);
        }
    }
}
