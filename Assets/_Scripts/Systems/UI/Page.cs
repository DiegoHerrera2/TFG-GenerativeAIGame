using _Scripts.Systems.UI.Animation;
using _Scripts.Units.Player;
using UnityEngine;

namespace _Scripts.Systems.UI
{
    public abstract class Page : MonoBehaviour
    {
        [Header("Page")]
        [SerializeField] protected TweenSequence enterSequence;
        [SerializeField] protected TweenSequence exitSequence;
        [SerializeField] private PlayerInput playerInput;
        
        protected abstract void Open();
        
        public void OpenPage()
        {
            Time.timeScale = 0;
            playerInput.enabled = !disableInputOnOpen;
            Open();
        }
        
        public void ClosePage()
        {
            Time.timeScale = 1;
            playerInput.enabled = true;
            Close();
        }
        protected abstract void Close();
        public bool exitOnNewPagePush = false;
        public bool disableInputOnOpen = true;

    }
}
