using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WindowManager : MonoBehaviour, IPointerDownHandler {

	public UIManager uiManager;

	private bool open;
	private bool active;

	public void OnPointerDown(PointerEventData data) {
		SetActive(true);
	}

	public bool MouseInside() {
		return EventSystem.current.IsPointerOverGameObject();
	}

	public void SetOpen(bool open) {
		if (!this.open) {
			if (open) {
				uiManager.DeactivateWindows();
			}

			gameObject.SetActive(open);
			SetActive(open);
		}
		this.open = open;
	}

	public void SetActive(bool active) {
		this.active = active;
		if (active) {
			transform.SetAsLastSibling();
		}
	}

	public bool GetActive() {
		return active;
	}

	public bool GetOpen() {
		return open;
	}
}
