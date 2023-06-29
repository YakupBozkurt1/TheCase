using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private RectTransform _rectTransform;
    private Vector2 pos;
    private CanvasGroup _canvasGroup;
    private BoxCollider2D coll;
    public bool dragState;
    private Image cardImage;
    
    private void Awake()
    {
        _rectTransform = this.gameObject.GetComponent<RectTransform>();
        _canvasGroup = this.gameObject.GetComponent<CanvasGroup>();
        coll = GetComponent<BoxCollider2D>();
        if (this.gameObject.transform.parent.tag == "Hand")
            dragState = true;
        else
            dragState = false;
        cardImage = this.gameObject.GetComponent<Image>();
        pos = Vector2.zero;
        
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (dragState)
        {
            pos = GetComponent<RectTransform>().anchoredPosition;
            _canvasGroup.alpha = .6f;
            _canvasGroup.blocksRaycasts = false;
            coll.enabled = false;
        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragState)
            _rectTransform.anchoredPosition += eventData.delta;
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragState)
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
            coll.enabled = true; 
        }
    }
    
    public void OnPointerDown(PointerEventData pointerEventData)
    {
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
  
        if (other.tag == "Hand")
        {
            dragState = true;
            if(pos != Vector2.zero)
                _rectTransform.anchoredPosition = pos;
        }
        else if (other.tag == "BotHand" && cardImage.sprite == null)
        {
            dragState = false;
        }
        else if (other.tag == "BotHand" && cardImage.sprite != null)
        {
            dragState = true;
            _rectTransform.anchoredPosition = pos;
        }
    }
}
