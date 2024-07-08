using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Systems
{
    public class SceneChanger : MonoBehaviour
    {
        
        [SerializeField] private CanvasGroup fadeCanvasGroup;
        // Function to go to the next scene
        public void GoToNextScene()
        {
            // Fade out the screen and then load the next scene
            fadeCanvasGroup.DOFade(1, 1f).OnComplete(() =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            });
        }
        
    }
}
