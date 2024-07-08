using _Scripts.Systems.Items;
using _Scripts.Systems.Puzzles;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Systems.UI
{
    public class DescriptionUI : Page
    {
        // Start is called before the first frame update
        [SerializeField] private GameObject descriptionMenu;
        [SerializeField] private GameObject canvasContainer;
        [SerializeField] private Image interactableImage;
        [SerializeField] private TMP_Text interactableDescriptionText;
        [SerializeField] private TMP_Text questQuoteText;

        private RectTransform _descriptionMenuRectTransform;
        private RectTransform _canvasRect;

        private const float XPadding = 50;

        private void Awake()
        {
            _descriptionMenuRectTransform = descriptionMenu.GetComponent<RectTransform>();
            _canvasRect = canvasContainer.GetComponent<RectTransform>();
                
            Interactable.EntityClicked += OpenDescriptionMenu;
            
            enterSequence.Init();
            exitSequence.Init();
        
            exitSequence.OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
            gameObject.SetActive(false);
        }
        

        // Need refactor for using the information class/struct 
        private void SetValues(IEntity entity, QuestHandlerBase questHandler)
        {

            var descriptionMenuHalfWidth = _descriptionMenuRectTransform.rect.width / 2;
            var itemXCoordinateOnScreen = Camera.main.WorldToScreenPoint(entity.Transform.position).x;
            if (itemXCoordinateOnScreen - descriptionMenuHalfWidth <= 0)
            {
                _descriptionMenuRectTransform.anchoredPosition = new Vector2(descriptionMenuHalfWidth + XPadding, _descriptionMenuRectTransform.position.y);
            }
            else if (itemXCoordinateOnScreen + descriptionMenuHalfWidth >= _canvasRect.rect.width)
            {
                _descriptionMenuRectTransform.anchoredPosition = new Vector2(_canvasRect.rect.width - descriptionMenuHalfWidth - XPadding, _descriptionMenuRectTransform.position.y);
            }
            else _descriptionMenuRectTransform.anchoredPosition = new Vector2(itemXCoordinateOnScreen, _descriptionMenuRectTransform.position.y);

            interactableImage.sprite = entity.SpriteRenderer.sprite;
            interactableImage.material = entity.SpriteRenderer.material;
            interactableDescriptionText.text = "Nombre: " + entity.Name;
            questQuoteText.text = questHandler != null ? "Misión:" + questHandler.QuestQuote : "";
            OpenPage();
        }

        private void OpenDescriptionMenu(GameObject target)
        {
            exitSequence.Complete();
            var entity = target.GetComponent<IEntity>();
            var questHandler = target.GetComponent<QuestHandlerBase>();
            if (questHandler == null) questHandler = target.GetComponentInChildren<QuestHandlerBase>();
            SetValues(entity, questHandler);
        }

        protected override void Open()
        {
            gameObject.SetActive(true);
            exitSequence.Rewind();
            enterSequence.Restart();
        }

        protected override void Close()
        {
            //enterSequence.Rewind();
            exitSequence.Restart();
        }
    }
}
