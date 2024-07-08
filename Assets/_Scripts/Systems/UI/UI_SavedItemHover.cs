using System.Collections;
using System.Collections.Generic;
using _Scripts.Systems.ImageRecognition;
using _Scripts.Systems.Items;
using _Scripts.Systems.UI;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UI_SavedItemHover : MonoBehaviour
{
    public GameObject textObject;
    public GameObject trashCan;
    public GameObject magicWand;
    
    [SerializeField] private GameObject hoveringInfo;
    
    [HideInInspector] public ItemData itemData;
    [HideInInspector] public int index;
    private bool _isHovering = false;
    public Inventory playerInventory;
    
    private Vector3 _originalTextPosition;

    
    private TMP_Text _textField;
    private void Awake() {
        var itemHoverManager = GetComponent<ButtonBehaviour>();
        

        itemHoverManager.hoverEnter.AddListener(OnHoverEnter);
        itemHoverManager.hoverExit.AddListener(OnHoverExit);
        
        trashCan.GetComponent<ButtonBehaviour>().clicked.AddListener(() =>
        {
            if (_isHovering) playerInventory.RemoveItem(index);
            OnHoverExit();
        });
        magicWand.GetComponent<ButtonBehaviour>().clicked.AddListener(() =>
        {
            if (_isHovering) ImageItemGenerator.Instance.SpawnItemFromItemData(itemData);
        });
        _textField = textObject.GetComponent<TMP_Text>();
        _originalTextPosition = textObject.transform.localPosition;
        hoveringInfo.SetActive(false);
    }
    

    private void OnHoverEnter()
    {
        hoveringInfo.SetActive(true);
        hoveringInfo.transform.SetParent(transform,false);
        _isHovering = true;
        // Change the pivot to the center
        hoveringInfo.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        hoveringInfo.transform.localPosition = new Vector3(0, 0, -1);
        _textField.text = itemData.Name;
        textObject.transform.DOLocalMoveY(textObject.transform.localPosition.y + 5, 1.25f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
    }

    private void OnHoverExit()
    {
        hoveringInfo.SetActive(false);
        DOTween.Kill(textObject.transform);
        textObject.transform.localPosition = _originalTextPosition;
        _isHovering = false;
    }
}
