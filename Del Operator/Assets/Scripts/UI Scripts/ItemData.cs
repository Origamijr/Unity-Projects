using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemData : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler {
	public GameObject item;
	public int slotNumber;

	private UIManager uiManager;
	private InventoryController inventory;

	private Vector2 offset = Vector2.zero;
	private bool dragging;

	void Start() {
		inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryController>();
		uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();

		dragging = false;
	}

	public void OnPointerDown(PointerEventData eventData) {
		
	}

	public void OnPointerUp(PointerEventData eventData) {
		
	}

	public void OnPointerClick(PointerEventData eventData) {
		if (uiManager.clickCount == 2) {
			inventory.ActivateItem(slotNumber);
		}
	}

	public void OnBeginDrag(PointerEventData eventData) {
		if (item != null) {
			dragging = true;
			//transform.parent.SetAsLastSibling();
			offset = eventData.position - new Vector2(transform.position.x, transform.position.y);
			transform.position = eventData.position - offset;
			transform.GetComponent<CanvasGroup>().blocksRaycasts = false;
		}
	}

	public void OnDrag(PointerEventData eventData) {
		if (item != null) {
			transform.position = eventData.position - offset;
		}
	}

	public void OnEndDrag(PointerEventData eventData) {
		transform.position = inventory.slots[slotNumber].transform.position;
		transform.GetComponent<CanvasGroup>().blocksRaycasts = true;
		dragging = false;
	}

	public bool GetDragging() {
		return dragging;
	}

	public void SetDragging(bool dragging) {
		this.dragging = dragging;
	}

	public void SnapToSlot(int slot) {
		if (slotNumber != slot)	{
			inventory.icons[slot] = inventory.icons[slotNumber];
			inventory.items[slot] = inventory.items[slotNumber];
			inventory.icons[slotNumber] = null;
			inventory.items[slotNumber] = null;
			slotNumber = slot;
		}
		transform.SetParent(inventory.slots[slotNumber].transform);
		transform.position = inventory.slots[slotNumber].transform.position;
	}
}
