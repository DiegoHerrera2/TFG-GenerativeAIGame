using System;
using _Scripts.Systems.UI.Animation;
using DG.Tweening;
using Systems;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Scripts.Systems.UI
{
    public class ButtonBehaviour : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {

        private bool _clicking;
        private bool _hovering;
        [SerializeField] private bool clickable = true;

        public UnityEvent hoverEnter;
        public UnityEvent hoverExit;
        public UnityEvent clicked;

        public bool IsActivated { get; set; } = true;
        
        private ButtonBehaviour _parentHoverManager;
        
        [SerializeField] private TweenSequence hoverEnterTween;
        [SerializeField] private TweenSequence hoverExitTween;
        [SerializeField] private TweenSequence clickTween;
        
        [SerializeField] private AudioCueData hoverEnterAudio;
        [SerializeField] private AudioCueData hoverExitAudio;
        [SerializeField] private AudioCueData clickAudio;
        
        private void Awake()
        {
            // Find parent hover manager. It could not exist
            _parentHoverManager = transform.parent.GetComponent<ButtonBehaviour>();
            IsActivated = true;
            
            hoverEnterTween.Init(gameObject);
            hoverExitTween.Init(gameObject);
            clickTween.Init(gameObject);
                
            clickTween.OnComplete(() =>
            {
                Debug.Log("Click animation complete from: " + gameObject.name);
                // If the click animation is complete, set clicking to false and if the button is not being hovered, play the hover exit animation
                _clicking = false;
                if (_hovering) hoverEnterTween.Restart();
            });
        }

        private void OnDisable()
        {
            hoverEnterTween.Rewind();
            hoverExitTween.Rewind();
            clickTween.Rewind();
            
            _clicking = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!IsActivated) return;
            if (!_clicking)
            {
                hoverExitTween.Rewind();
                hoverEnterTween.Restart();
                if (hoverEnterAudio != null) AudioManager.PlayWithPitchRange(hoverEnterAudio, transform.position, new Vector2(0.9f, 1.1f));
            }
            _hovering = true;
            hoverEnter?.Invoke();
            // Call parent onpointerenter if exists
            if (_parentHoverManager != null) _parentHoverManager.OnPointerEnter(eventData);
            
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!IsActivated) return;
            
            _hovering = false;
            if (!_clicking)
            {
                hoverEnterTween.Rewind();
                hoverExitTween.Restart();
                if (hoverExitAudio != null) AudioManager.PlayWithPitchRange(hoverExitAudio, transform.position, new Vector2(0.9f, 1.1f));
            }
            hoverExit?.Invoke();
            // Call parent onpointerexit if exists
            if (_parentHoverManager != null) _parentHoverManager.OnPointerExit(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!IsActivated || !clickable) return;
            clickTween.Rewind();
            _clicking = true;
            // Prioritize click animation and audio 
            hoverEnterTween.Rewind();
            clickTween.Restart();
            if (clickAudio != null) AudioManager.PlayWithPitchRange(clickAudio, transform.position, new Vector2(0.9f, 1.1f));
            clicked?.Invoke();
        }

    }
}