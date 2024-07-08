using System;
using _Scripts.Systems.UI.Animation;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Systems.UI
{
    public class InputWindow : Page
    {
        // Start is called before the first frame update
    
        public ButtonBehaviour acceptButton;
        public ButtonBehaviour cancelButton;

        private TMP_InputField _inputField;
        
        public event Action<string> PromptEntered;

        private void Awake()
        {
            acceptButton.IsActivated = false;
            cancelButton.IsActivated = false;
            
            _inputField = GetComponentInChildren<TMP_InputField>();
            acceptButton.clicked.AddListener(OnPromptEntered);
            cancelButton.clicked.AddListener(ClosePage);
            
            enterSequence.Init();
            exitSequence.Init();

            enterSequence.OnComplete(() =>
            {
                acceptButton.IsActivated = true;
                cancelButton.IsActivated = true;
            });
            
            exitSequence.OnComplete(() =>
            {
                _inputField.text = "";
                gameObject.SetActive(false);
            });

            gameObject.SetActive(false);
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                OnPromptEntered();
                _inputField.text = "";
            }
        }
        protected override void Open()
        {
            gameObject.SetActive(true);
            _inputField.Select();
            _inputField.ActivateInputField();

            exitSequence.Rewind();
            enterSequence.Restart();

            _inputField.text = "";
        }
        private void OnPromptEntered()
        {
            if (string.IsNullOrEmpty(_inputField.text)) return;
            ClosePage();
            PromptEntered?.Invoke(_inputField.text);
            _inputField.text = "";
        }

        protected override void Close()
        {
            acceptButton.IsActivated = false;
            cancelButton.IsActivated = false;
            
            enterSequence.Rewind();
            exitSequence.Restart();
        }

    }
}
