using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler {
    public Item item;
    public int stack_size; // defaults to 1
    public bool world_interactable = false;

    private RectTransform rt;
    private Canvas canvas;
    private CanvasGroup cg;
    private GameObject stack_size_text;
    private GameObject slot_container;
    public GameObject parent_ui; // the ui object this slot belongs to

    void Start(){
        canvas = GameObject.Find("HUD").GetComponent<Canvas>();
        rt = GetComponent<RectTransform>();
        cg = GetComponent<CanvasGroup>();
        stack_size = 0;
        if(!world_interactable){
            stack_size_text = gameObject.transform.GetChild(2).gameObject;
        }
        slot_container = gameObject.transform.parent.gameObject;
    }

    void Update(){
        RenderStackSize();
    }

    public InventorySlot(Item _item, int _stack_size){
        item = _item;
        stack_size = _stack_size;
    }

    public void OnPointerDown(PointerEventData eventData){
        if(eventData.button==PointerEventData.InputButton.Right){
            // let's drop the item
            if(item!=null){
                slot_container.GetComponent<SlotContainer>().DropItem();
            }else{
                Debug.Log("Pointer down item not null: " + item.name);
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData){
        if(item!=null){
            cg.alpha = .6f;
            cg.blocksRaycasts = false;
        }
    }

    public void OnEndDrag(PointerEventData eventData){
        cg.alpha = 1f;
        cg.blocksRaycasts = true;
        eventData.pointerDrag.transform.localPosition = new Vector3(0, 0, 0);   // this returns the pointer back to its original position
    }

    public void OnDrag(PointerEventData eventData){
        if(item!=null){
            rt.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    private void RenderStackSize(){
        if(!world_interactable){
            if(stack_size > 0 && !stack_size_text.activeSelf){
                stack_size_text.SetActive(true);
            }else if(stack_size <= 0 && stack_size_text.activeSelf){
                stack_size_text.SetActive(false);
            }
        }
    }

    public void SetParentUI(GameObject _parent){
        parent_ui = _parent;
    }
}
