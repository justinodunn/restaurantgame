using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Made by David Malaky
public class Item : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler {
    public ItemSlot slot;
    public CanvasGroup CG;
    public InventoryItem data;
    public int slotNum;
    public Transform currentParent;

    public void OnBeginDrag(PointerEventData eventData) {
        currentParent = transform.parent;
        transform.parent = null;
        CG.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData) {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData) {
        CG.blocksRaycasts = true;
        transform.parent = currentParent;
        transform.localPosition = Vector2.zero;
    }
}

public class ItemSlot : MonoBehaviour, IDropHandler {
    public Item currentItem;
    public int slotNum;
    public void OnDrop(PointerEventData eventData) {
        if (!currentItem) ;
    }
}