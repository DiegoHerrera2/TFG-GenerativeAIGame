using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using _Scripts.Systems.Items;
using _Scripts.Systems.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

namespace _Scripts.Systems.ImageRecognition
{
    public class ImageItemGenerator : MonoBehaviour
    {
        public Texture2D testImage;
        private ImageGenerator _imageGenerator;
        private ImageRecognizer _imageRecognizer;

        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private InputWindow inputWindow;
        
        private static ImageItemGenerator _instance;
        private ObjectPool<GameObject> _itemPool;
        public event Action ItemGenerated;
        public event Action ItemGenerationFailed;
        public void ReleaseItem(GameObject item)
        {
            _itemPool.Release(item);
        }
        public static ImageItemGenerator Instance
        {
            get
            {
                if (_instance != null) return _instance;
                var objs = FindObjectsOfType<ImageItemGenerator>();
                switch (objs.Length)
                {
                    case > 1:
                        Debug.LogError("There is more than one ImageItemGenerator in the scene");
                        break;
                    case 0:
                        Debug.LogError("There is no ImageItemGenerator in the scene");
                        break;
                    default:
                        _instance = objs[0];
                        break;
                }
                return _instance;
            }
        }
        private void Awake()
        {
            _itemPool = new ObjectPool<GameObject>(() =>
                {
                    var item = Instantiate(itemPrefab);
                    item.SetActive(false);
                    return item;
                },
                item => item.SetActive(true),
                item =>
                {
                    item.GetComponent<Interactable>().Reset();
                    item.SetActive(false);
                },
                null,
                true,
                100);
        }
        private void Start()
        {
            _imageGenerator = new ImageGenerator();
            _imageRecognizer = new ImageRecognizer();
        
            inputWindow.PromptEntered += OnPromptEntered;
        }
        
        private void OnPromptEntered(string prompt)
        {
            _ = OnPromptEnteredAsync(prompt);
        }

        private async Task OnPromptEnteredAsync(string prompt)
        {
            var generatedImage = await _imageGenerator.GenerateImageAsync(prompt);
            
            if (generatedImage == null)
            {
                ItemGenerationFailed?.Invoke();
                return;
            }
            
            var labelsDetected = await _imageRecognizer.AnalyzeImageAsync(generatedImage);
            
            if (labelsDetected == null)
            {
                ItemGenerationFailed?.Invoke();
                return;
            }
            
            var texture = new Texture2D(1, 1);
            var item = new ItemData(prompt, labelsDetected.ToList());
                
            texture.LoadImage(generatedImage);
            item.Sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                
                
            SpawnItemFromItemData(item);
            ItemGenerated?.Invoke();
        }
        
        public void SpawnItemFromItemData(ItemData itemData)
        {
            var itemObject = _itemPool.Get();
            itemObject.GetComponent<ItemInstance>().ItemData = itemData;
            var screenCenter = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f));
            itemObject.transform.position = new Vector3(screenCenter.x, screenCenter.y, 1);
        }
    }
}
