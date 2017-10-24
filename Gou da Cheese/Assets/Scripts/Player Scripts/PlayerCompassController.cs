using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCompassController : MonoBehaviour {
	//private Rigidbody rb;
	private int floorMask;
	private float camRayLength = 100f;

	void Awake() {
		floorMask = LayerMask.GetMask("Floor");
		//rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!(UIManager.GetPaused())) {
			Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit floorHit;

			if (Physics.Raycast(camRay, out floorHit, camRayLength, floorMask) && Input.GetMouseButton(1)) {
				Vector3 playerToMouse = floorHit.point - transform.position;
				playerToMouse.y = 0.0f;

				Vector3 newRotation = Quaternion.LookRotation(playerToMouse).eulerAngles;
				transform.rotation = Quaternion.Euler(90.0f, newRotation.y, 0.0f);
			}
		}
	}
}
