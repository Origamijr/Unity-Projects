using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour {
	public GameObject[] initialInventory;

	public GameObject inventoryPanel;
	public GameObject equipmentPanel;
	private GameObject inventorySlotPanel;
	private GameObject equipmentSlotPanel;
	public GameObject panelSlot;
	public GameObject panelItem;

	private static int capacity = 20;
	private static int maxEquip = 1;
	private int totalSize = capacity + maxEquip;
	public GameObject[] slots;
	public GameObject[] items;
	public GameObject[] icons;

	private GameObject weapon;
	private int weaponSlotNumber = capacity;

	void Start () {
		slots = new GameObject[totalSize];
		items = new GameObject[totalSize];
		icons = new GameObject[totalSize];

		inventorySlotPanel = inventoryPanel.transform.Find("Slot Panel").gameObject;
		equipmentSlotPanel = equipmentPanel.transform.Find("Slot Panel").gameObject;

		for (int i = 0; i < capacity; i++) {
			slots[i] = (GameObject)Instantiate(panelSlot);
			slots[i].transform.SetParent(inventorySlotPanel.transform);
			slots[i].transform.GetComponent<InventorySlotOrganizer>().slotNumber = i;
		}

		for (int i = capacity; i < totalSize; i++) {
			slots[i] = (GameObject)Instantiate(panelSlot);
			slots[i].transform.SetParent(equipmentSlotPanel.transform);
			slots[i].transform.GetComponent<InventorySlotOrganizer>().slotNumber = i;
		}

		if (initialInventory.Length > slots.Length) {
			Debug.LogError("Initial inventory too long");
		} else {
			for (int i = 0; i < initialInventory.Length; i++) {
				if (initialInventory[i] != null) {
					AddItem(initialInventory[i], i);
				}
			}
		}
	}

	public void AddItem(GameObject item) {
		for (int i = 0; i < capacity; i++) {
			if (items[i] != null) {
				AddItem(item, i);
				break;
			}
		}
	}

	void AddItem(GameObject item, int slotNumber) {
		if (item.GetComponent<Stashable>() != null) {
			items[slotNumber] = item;
			Sprite itemSprite = item.GetComponent<Stashable>().sprite;

			icons[slotNumber] = (GameObject)Instantiate(panelItem);
			icons[slotNumber].transform.SetParent(slots[slotNumber].transform);
			icons[slotNumber].transform.GetComponent<Image>().sprite = itemSprite;
			icons[slotNumber].GetComponent<ItemData>().item = item;
			icons[slotNumber].GetComponent<ItemData>().slotNumber = slotNumber;
		}
	}

	public void MoveItem(int fromSlot, int toSlot) {
		if (items[fromSlot] != null && fromSlot != toSlot) {
			GameObject tempIcon = icons[toSlot];
			GameObject tempItem = items[toSlot];
			if (tempIcon != null) {
				tempIcon.transform.GetComponent<ItemData>().slotNumber = fromSlot;
			}

			icons[fromSlot].transform.GetComponent<ItemData>().SnapToSlot(toSlot);

			if (tempIcon != null) {
				icons[fromSlot] = tempIcon;
				items[fromSlot] = tempItem;
				tempIcon.transform.GetComponent<ItemData>().SnapToSlot(fromSlot);
			}
		}
	}

	public void ActivateItem(int slotNumber) {
		if (items[slotNumber].GetComponent<WeaponManager>() != null) {
			if (weapon != null) {
				Destroy(weapon);
			}
			weapon = (GameObject)Instantiate(items[slotNumber]);
			weapon.GetComponent<WeaponManager>().Equip(true);
			MoveItem(slotNumber, weaponSlotNumber);
		}
	}

	public GameObject GetWeapon() {
		return weapon;
	}
}
