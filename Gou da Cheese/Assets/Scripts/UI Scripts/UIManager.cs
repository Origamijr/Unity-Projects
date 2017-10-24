using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {
	public GameObject pausePanel;
	public GameObject inventoryPanel;
	public GameObject equipmentPanel;

	public int clickCount;
	private float clickCooler, comboCooler;
	public float doubleClick = 0.5f;

	private static bool paused;

	public List<GameObject> panels = new List<GameObject>();

	void Awake() {
		paused = false;

		panels.Add(pausePanel);
		panels.Add(inventoryPanel);
		panels.Add(equipmentPanel);

		foreach (GameObject panel in panels) {
			if (panel.GetComponent<WindowManager>() != null) {
				panel.GetComponent<WindowManager>().uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
			}
		}
	}

	void Update () {
		// pause handler
		if (paused) {
			Time.timeScale = 0.0f;
		} else {
			Time.timeScale = 1.0f;
		}

		// panel handler
		foreach (GameObject panel in panels) {
			if (panel.GetComponent<WindowManager>() != null) {
				OpenPanel(panel, panel.GetComponent<WindowManager>().GetOpen());
			}
		}

		// input handler
		if (Input.GetButtonDown("Cancel")) {
			TogglePause();
		}
		if (!paused) {
			if (Input.GetKeyDown(KeyCode.I)) {
				OpenPanel(inventoryPanel, !(inventoryPanel.GetComponent<WindowManager>().GetOpen()));
			}
			if (Input.GetKeyDown(KeyCode.U)) {
				OpenPanel(equipmentPanel, !(equipmentPanel.GetComponent<WindowManager>().GetOpen()));
			}

			// combo input handler
			if (Input.GetMouseButtonDown(0)) {
				if (clickCooler > 0.0f) {
					clickCount++;
				} else {
					clickCount = 1;
				}
				clickCooler = doubleClick;
				comboCooler = doubleClick;
			}
				
			if (clickCooler > 0.0f) {
				clickCooler -= Time.deltaTime;
			} else if (!(Input.GetMouseButton(0)) &&comboCooler > 0.0f) {
				comboCooler -= Time.deltaTime;
			} else if (!(Input.GetMouseButton(0))){
				clickCount = 0;
			}

			if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0) {
				clickCooler = 0.0f;
				clickCount = 0;
			}
		}
	}

	public bool MouseOnUI() {
		bool inside = false;
		foreach (GameObject panel in panels) {
			if (panel.GetComponent<WindowManager>() != null) {
				if (panel.GetComponent<WindowManager>().MouseInside()) {
					inside = true;
				}
			}
		}
		return inside;
	}

	public void TogglePause() {
		paused = !paused;
		OpenPanel(pausePanel, !(pausePanel.GetComponent<WindowManager>().GetOpen()));
	}

	public static bool GetPaused() {
		return paused;
	}

	void OpenPanel(GameObject panel, bool open) {
		if (panel.GetComponent<WindowManager>() != null) {
			panel.GetComponent<WindowManager>().SetOpen(open);
		}
	}

	public void DeactivateWindows() {
		foreach (GameObject panel in panels) {
			if (panel.GetComponent<WindowManager>() != null) {
				panel.GetComponent<WindowManager>().SetActive(false);
			}
		}
	}

	public void QuitGame() {
		Application.Quit();
	}
}
