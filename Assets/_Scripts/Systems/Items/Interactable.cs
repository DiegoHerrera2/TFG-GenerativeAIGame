using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Systems.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Scripts.Systems.Items
{
    public class Interactable : MonoBehaviour
    {
        public bool Attached { get; private set; }

        private Collider2D _coll;
        private SpriteRenderer _spriteRenderer;

        private ContactFilter2D _contactFilter;
        private Vector3 _mouseOffset;
        private Vector3 _originalPosition;
        private Vector3 _lastPosition;

        [Header("Drag Variables")]
        [SerializeField] private bool canBeDragged = true;
        private bool _isDragging;
        private bool _ignoreClick;
        [SerializeField] private float threshold = 0.075f;
        
        [Header("Layer Masks")]
        [SerializeField] private LayerMask attachableLayer;
        [SerializeField] private LayerMask uiLayer;
        
        public event Action<ItemData> ItemAttached;

        public static event Action<GameObject> EntityClicked;
        public static event Action EntityDragged;
        public static event Action EntityDropped;
        

        private void Awake()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _contactFilter.useTriggers = true;
            _contactFilter.SetLayerMask(attachableLayer);
            // the object has two colliders, one is trigger and the other is not. We use the trigger one to check if the object is intersecting with another object
            _coll = GetComponent<Collider2D>();
        }
        private static Vector3 GetMouseWorldPos()
        {
            var mousePos = Input.mousePosition;
            mousePos.z = 10;
            return Camera.main.ScreenToWorldPoint(mousePos);
        }
        public void OnMouseDown()
        {
            if (Attached) return; // Aquí cuando lo haga como pegatinas tendrá que despegarse
            var raycastResults = GetRaycastResultsOnUI();
            if (raycastResults.Any(x => (uiLayer | 1 << x.gameObject.layer) == uiLayer))
            {
                _ignoreClick = true;
                return;
            }
            _ignoreClick = false;
            var position = transform.position;
            _mouseOffset = position - GetMouseWorldPos();
            _originalPosition = position;
        }
        public void OnMouseDrag()
        {
            if (!canBeDragged || Attached || _ignoreClick) return;
            
            var mousePos = GetMouseWorldPos() + _mouseOffset;
            transform.position = mousePos;
            if (!_isDragging && Vector2.Distance(transform.position, _originalPosition) > threshold)
            {
                _isDragging = true;
                EntityDragged?.Invoke();
            }
            
            LookBehind();
        }
        public void OnMouseUp()
        {
            if (Attached || _ignoreClick) return;

            if (!_isDragging || !canBeDragged)
            {
                EntityClicked?.Invoke(gameObject);
                return;
            }

            // This detects if the mouse is over an UI element that can handle the drop  
            if (Camera.main != null) Camera.main.ScreenPointToRay(Input.mousePosition);
            var raycastResults = GetRaycastResultsOnUI();
            
            if (raycastResults.Any(x => (uiLayer | 1 << x.gameObject.layer) == uiLayer))
            {
                    raycastResults[0].gameObject.GetComponent<IDroppingHandler>()!.HandleDrop(gameObject);
            }
            // If the mouse is over an attachable object, we attach it to it. So throw a raycast to check if the mouse is over an attachable object
            else if (_coll.IsTouchingLayers(attachableLayer))
            {
                var results = new Collider2D[1];
                _coll.OverlapCollider(_contactFilter, results);
                foreach (var result in results)
                {
                    if (result == null) continue;
                    // We take the first object that is not the object itself
                    var target = result.GetComponent<Interactable>();
                    if (target != this)
                    {
                        target.GetAttached(this);
                        break;
                    }
                }
            }

            EntityDropped?.Invoke();
            _isDragging = false;
        }

        private static List<RaycastResult> GetRaycastResultsOnUI()
        {
            var raycastResults = new List<RaycastResult>();
            var pointerEventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);
            return raycastResults;
        }
        
        private void LookBehind()
        {
            _spriteRenderer.color = _coll.IsTouchingLayers(attachableLayer) ? Color.yellow : Color.white;
        }

        private void GetAttached(Interactable targetToAttach)
        {
            var targetTransform = targetToAttach.transform;
            targetTransform.SetParent(transform);
            targetTransform.localPosition = Vector3.zero;
            targetToAttach.Attached = true;
            targetToAttach._spriteRenderer.color = Color.white;
            
            ItemAttached?.Invoke(targetToAttach.GetComponent<ItemInstance>().ItemData);
        }
        
        public void Reset()
        {
            Attached = false;
            transform.SetParent(null);
            _spriteRenderer.color = Color.white;
        }
    }
}
