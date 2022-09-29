using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler {
    public Equipment.Type type;
    public Equipment item;
    public int stack_size; // defaults to 1
    public bool world_interactable = false;

    private RectTransform rt;
    private Canvas canvas;
    private CanvasGroup cg;
    private GameObject stack_size_text;

    void Awake(){
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        rt = GetComponent<RectTransform>();
        cg = GetComponent<CanvasGroup>();
    }

    void Start(){
        stack_size = 0;
        if(!world_interactable){
            stack_size_text = gameObject.transform.GetChild(2).gameObject;
            stack_size_text.GetComponent<Text>().text = type.ToString();
        }
    }

    void Update(){
        RenderStackSize();
    }

    public EquipmentSlot(Equipment _item, int _stack_size){
        item = _item;
        stack_size = _stack_size;
    }

    public void OnPointerDown(PointerEventData eventData){
        Debug.Log("Pointer Down From Inventory Slot");
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

    private void OnTriggerEnter(Collider other){
        if(other.tag=="Player"){
            other.gameObject.GetComponent<PlayerInteraction>().CanInteractWith(item);
        }
    }

    private void OnTriggerExit(Collider other){
        if(other.tag=="Player"){
            other.gameObject.GetComponent<PlayerInteraction>().CanNotInteractWith(item);
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
}
