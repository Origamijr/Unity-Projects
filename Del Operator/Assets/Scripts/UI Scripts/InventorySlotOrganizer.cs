using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotOrganizer : MonoBehaviour, IDropHandler {
	public int slotNumber;

	private InventoryController inventory;

	void Start() {
		inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryController>();
	}

	public void OnDrop(PointerEventData eventData) {
		ItemData droppedItem = eventData.pointerDrag.GetComponent<ItemData>();
		if (droppedItem != null && droppedItem.GetDragging()) {
			inventory.MoveItem(droppedItem.slotNumber, slotNumber);
			droppedItem.SetDragging(false);
		}
	}
}
