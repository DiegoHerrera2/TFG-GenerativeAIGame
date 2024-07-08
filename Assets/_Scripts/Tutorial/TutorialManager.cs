using System;
using System.Drawing;
using System.Numerics;
using _Scripts.Systems.ImageRecognition;
using _Scripts.Systems.Items;
using _Scripts.Systems.Puzzles;
using _Scripts.Systems.UI;
using _Scripts.Systems.UI.Animation;
using _Scripts.Units.Player;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityHFSM;
using static UnityEngine.Device.Screen;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace _Scripts.Tutorial
{
    public class TutorialManager : MonoBehaviour
    {

        [SerializeField] private GameObject tutorialTextBox;
        private TMP_Text _tutorialText;
        
        [Header("UI Elements to show/hide")]
        [SerializeField] private GameObject itemCreation;
        [SerializeField] private GameObject itemSaving;
        [SerializeField] private GameObject arrow;
        [SerializeField] private GameObject notebook;
        [Header("Buttons to react")]
        [SerializeField] private ButtonBehaviour nextButton;
        [SerializeField] private ButtonBehaviour notebookButton;
        [SerializeField] private InputWindow inputWindow;
        [SerializeField] private ButtonBehaviour closeInputWindowButton;
        [SerializeField] private ButtonBehaviour descriptionMenuCloseButton;
        [SerializeField] private ButtonBehaviour backpackButton;
        [SerializeField] private ButtonBehaviour backpackCloseButton;
        
        [SerializeField] private QuestHandlerBase tutorialQuestHandler;
        
        [Header("Player elements to interact with")]
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private Inventory playerInventory;

        private StateMachine _stateMachine;

        private void Start()
        {
            _tutorialText = tutorialTextBox.GetComponent<TMP_Text>();
            
            //Disable all interactables in the scene
            var interactables = FindObjectsOfType<Interactable>();
            foreach (var interactable in interactables)
            {
                interactable.enabled = false;
            }

            itemCreation.SetActive(false);
            itemSaving.SetActive(false);
            
            _stateMachine = new StateMachine();
            
            nextButton.clicked.AddListener(() =>
            {
                _stateMachine.Trigger("next");
            });
            
            notebookButton.clicked.AddListener(() =>
            {
                _stateMachine.Trigger("clickedOnNotebook");
            });

            inputWindow.PromptEntered += (prompt) =>
            {
                _stateMachine.Trigger("promptEntered");
            };
            
            closeInputWindowButton.clicked.AddListener(() =>
            {
                _stateMachine.Trigger("inputWindowClosed");
            });
            
            ImageItemGenerator.Instance.ItemGenerated += () =>
            {
                _stateMachine.Trigger("itemGenerated");
            };
            
            ImageItemGenerator.Instance.ItemGenerationFailed += () =>
            {
                _stateMachine.Trigger("itemGenerationFailed");
            };
            
            playerInventory.ItemAdded += (_,_) =>
            {
                _stateMachine.Trigger("itemSaved");
            };
            
            backpackButton.clicked.AddListener(() =>
            {
                _stateMachine.Trigger("backpackOpened");
            });
            
            backpackCloseButton.clicked.AddListener(() =>
            {
                _stateMachine.Trigger("backpackClosed");
            });
            
            Interactable.EntityClicked += (_) =>
            {
                _stateMachine.Trigger("interactableClicked");
            };
            
            descriptionMenuCloseButton.clicked.AddListener(() =>
            {
                _stateMachine.Trigger("descriptionMenuClosed");
            });
            
            tutorialQuestHandler.QuestCompletedEvent += () =>
            {
                _stateMachine.Trigger("questCompleted");
            };

            var initialState = new State( (_) => { _tutorialText.text = "Bienvenido al tutorial de este proyecto. Para avanzar pulsa el botón de la derecha"; playerInput.enabled = false; tutorialQuestHandler.gameObject.SetActive(false); },null, (_) => { nextButton.gameObject.SetActive(false); });
            
            var itemCreationState = new State( (_) => { 
                _tutorialText.text = "En este proyecto tienes que crear objetos para avanzar. Esta libreta es tu herramienta principal. Pulsa en ella para continuar";
                itemCreation.SetActive(true);
                notebook.SetActive(true);
                PointTo(notebookButton.transform); 
            }, null, (_) => { DisableArrow(); });
            
            var writingOnNotebookState = new State( (_) => { _tutorialText.text = "Cualquier cosa que quieras se hará realidad. Prueba a escribir algo"; }, null, (_) => { notebook.SetActive(false); });
            var waitForItemGenerationState = new State( (_) => { _tutorialText.text = "Espera a que el objeto se genere"; }, null, (_) => { itemCreation.SetActive(false); });
            var itemGenerationFailedState = new State( (_) => { _tutorialText.text = "Algo ha fallado con la generación, inténtalo de nuevo"; itemCreation.SetActive(true); notebook.SetActive(true); }, null, (_) => { notebook.SetActive(false); });
            var itemSavingState = new State( (_) => { 
                _tutorialText.text = "Ahora que tienes tu objeto, puedes interactuar con él utilizando el ratón. Prueba a arrastrar el objeto a la mochila";
                itemSaving.SetActive(true);
                PointTo(backpackButton.transform); 
            });
            
            var backpackOpenState = new State((_) =>
            {
                _tutorialText.text = "Acabas de guardar tu objeto. Para ver tus objetos guardados pulsa en la mochila";
            }, null , (_) => { DisableArrow();  });
            var backpackCloseState = new State((_) =>
            {
                _tutorialText.text = "Aquí puedes gestionar tu inventario, así como hacer aparecer objetos que hayas guardado. Para cerrar la mochila, pulsa en la X";
            }, 
                null,
                (_) =>
                {
                    // Reactivate all interactables
                    foreach (var interactable in interactables)
                    {
                        interactable.enabled = true;
                    }
                    itemSaving.SetActive(false);
                });
            var interactablesExplanationState = new State( (_) => { _tutorialText.text = "En la escena puedes interactuar con los objetos haciendo click en ellos. Prueba con ese mago de ahí"; tutorialQuestHandler.gameObject.SetActive(true); });
            var descriptionMenuState = new State( (_) => { _tutorialText.text = "Esta es la descripción del elemento que has clicado"; nextButton.gameObject.SetActive(true); }, null, (_) => { nextButton.gameObject.SetActive(false); });
            var nameAndQuestState = new State( (_) => { _tutorialText.text = "Aquí abajo tienes el nombre y la misión del elemento (siempre y cuando tenga una). Ahora sal de este menú"; });
            var dragAndDropState = new State((_) =>
            {
                _tutorialText.text = "Para completar misiones tendrás que arrastrar los objetos correctos para cada misión. Con lo que sabes, intenta completar la misión de este mago";
                itemCreation.SetActive(true);
                itemSaving.SetActive(true);
                notebook.SetActive(true);
            });
            var endState = new State( (_) => { _tutorialText.text = "Aquí se acaba el tutorial, espero que lo disfrutes"; nextButton.gameObject.SetActive(true);  }, null, (_) => { playerInput.enabled = true; Destroy(gameObject);  });
            var voidState = new State(null);
            
            // Estados
            _stateMachine.AddState("Start", initialState);
            _stateMachine.AddState("ItemCreation", itemCreationState);
            _stateMachine.AddState("WritingOnNotebook", writingOnNotebookState);
            _stateMachine.AddState("waitForItemGeneration", waitForItemGenerationState);
            _stateMachine.AddState("GenerationFailed", itemGenerationFailedState);
            _stateMachine.AddState("ItemSaving", itemSavingState);
            _stateMachine.AddState("BackpackOpen", backpackOpenState);
            _stateMachine.AddState("BackpackClose", backpackCloseState);
            _stateMachine.AddState("InteractableExplanation", interactablesExplanationState);
            _stateMachine.AddState("DescriptionMenu", descriptionMenuState);
            _stateMachine.AddState("NameAndQuest", nameAndQuestState);
            _stateMachine.AddState("dragAndDropState", dragAndDropState);
            _stateMachine.AddState("End", endState);
            _stateMachine.AddState("Void", voidState);
            
            // Transiciones
            _stateMachine.AddTriggerTransition("next", "Start", "ItemCreation");
            _stateMachine.AddTriggerTransition("clickedOnNotebook", "ItemCreation", "WritingOnNotebook");
            _stateMachine.AddTriggerTransition("promptEntered", "WritingOnNotebook", "waitForItemGeneration",null, (_) => { playerInput.enabled = false; });
            _stateMachine.AddTriggerTransition("inputWindowClosed", "WritingOnNotebook", "ItemCreation", null, (_) => { playerInput.enabled = false; });
            _stateMachine.AddTriggerTransition("itemGenerated", "waitForItemGeneration", "ItemSaving");
            _stateMachine.AddTriggerTransition("itemGenerationFailed", "waitForItemGeneration", "GenerationFailed");
            _stateMachine.AddTriggerTransition("clickedOnNotebook", "GenerationFailed", "WritingOnNotebook");
            _stateMachine.AddTriggerTransition("itemSaved", "ItemSaving", "BackpackOpen");
            _stateMachine.AddTriggerTransition("backpackOpened", "BackpackOpen", "BackpackClose");
            _stateMachine.AddTriggerTransition("backpackClosed", "BackpackClose", "InteractableExplanation", null, (_) => { playerInput.enabled = false; });
            _stateMachine.AddTriggerTransition("interactableClicked", "InteractableExplanation", "DescriptionMenu");
            _stateMachine.AddTriggerTransition("descriptionMenuClosed", "DescriptionMenu", "InteractableExplanation", null, (_) => { playerInput.enabled = false; });
            _stateMachine.AddTriggerTransition("next", "DescriptionMenu", "NameAndQuest");
            _stateMachine.AddTriggerTransition("descriptionMenuClosed", "NameAndQuest", "dragAndDropState", null, (_) => { playerInput.enabled = false; });
            _stateMachine.AddTriggerTransition("questCompleted", "dragAndDropState", "End");
            _stateMachine.AddTriggerTransition("next", "End", "Void");
            
            
            _stateMachine.SetStartState("Start");
            
            _stateMachine.Init();
            
        }

        private void Update()
        {
            _stateMachine.OnLogic();
            // Print the number of tweens
        }

        private void PointTo(Transform target)
        {
            arrow.SetActive(true);
            
            var initialPosition = new Vector3(width / 2, height / 2, 0);
            var targetPosition = Camera.main.WorldToScreenPoint(target.position);
            var direction = targetPosition - initialPosition;
            arrow.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            
            // make the arrow move close to the target, but not too close
            var distance = Vector3.Distance(initialPosition, targetPosition);
            var newPosition = initialPosition + direction.normalized * (distance * 0.8f);
            var finalPosition = new Vector3(Camera.main.ScreenToWorldPoint(newPosition).x, Camera.main.ScreenToWorldPoint(newPosition).y, 0);
            arrow.transform.position = finalPosition;
            
            // Move back and forth to the target
            arrow.transform.DOMove(target.position / 1.5f, 1f).SetLoops(-1, LoopType.Yoyo);
        }
        
        private void DisableArrow()
        {
            arrow.SetActive(false);
            arrow.transform.DOKill();
        }
        
    }
}
