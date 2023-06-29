using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private RectTransform _rectTransform;   //To update the position of the card while dragging
    private Vector2 pos;                    //To put the card to the previous position when dragging is not through the table
    private CanvasGroup _canvasGroup;       //To determine the transparency of the card while dragging and blocking raycast
    private BoxCollider2D coll;             //Card's collider
    public bool dragState;                  //Determines if a card is draggable
    private Image cardImage;                //Card's Image
    
    private void Awake()
    {
        //Gets the card's components
        _rectTransform = this.gameObject.GetComponent<RectTransform>();
        _canvasGroup = this.gameObject.GetComponent<CanvasGroup>();
        coll = GetComponent<BoxCollider2D>();
        cardImage = this.gameObject.GetComponent<Image>();
        pos = Vector2.zero;
        
        if (this.gameObject.transform.parent.tag == "Hand")     //If the card is belong to player, it is draggable
            dragState = true;
        else                                                    //Else, it is not draggable
            dragState = false;
    }
    
    public void OnBeginDrag(PointerEventData eventData)     //When the dragging begins
    {
        if (dragState)
        {
            pos = GetComponent<RectTransform>().anchoredPosition;   //gets the cards first position as it begins to drag
            _canvasGroup.alpha = .6f;                               //Changes the transparency of the card
            _canvasGroup.blocksRaycasts = false;
            coll.enabled = false;                   //When the cards is being dragged, it should not be able to collide so i disabled it
        }

    }

    public void OnDrag(PointerEventData eventData)  //While dragging
    {
        if (dragState)
            _rectTransform.anchoredPosition += eventData.delta;
    }
    
    public void OnEndDrag(PointerEventData eventData)    //When the dragging ends
    {
        if (dragState)
        {
            _canvasGroup.alpha = 1f;            //Changes the transparency of the card
            _canvasGroup.blocksRaycasts = true;
            coll.enabled = true;                //When the dragging ends, it should be able to collide so i enabled it
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
  
        if (other.tag == "Hand")    //If the collider happened in the area of player
        {
            dragState = true;       
            if(pos != Vector2.zero)
                _rectTransform.anchoredPosition = pos;  //It gets to the previous position
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
