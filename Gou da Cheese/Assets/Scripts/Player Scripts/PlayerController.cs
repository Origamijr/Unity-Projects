using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	public float walkSpeed = 1f;

	private UIManager uiManager;

	private GameObject model;
	private Vector3 movement;
	private Animator anim;
	private Rigidbody rb;
	private GameObject playerCompass;

	private int floorMask;
	private float camRayLength = 100f;
	private Vector3 mouseLoc = Vector3.zero;

	private float doubleTap = 0.5f;
	private float buttonCooler = 0.5f;
	private int buttonCount = 0;

	private float speed, runSpeed;

	void Awake() {
		uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();

		model = transform.Find("Gou").gameObject;
		playerCompass = transform.Find("Player Compass").gameObject;
		floorMask = LayerMask.GetMask("Floor");
		anim = model.GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();

		speed = walkSpeed;
		runSpeed = walkSpeed * 2;
	}

	void FixedUpdate() {
		if (!(UIManager.GetPaused())) {
			//check input
			bool active = false;
			float h = 0.0f;
			float v = 0.0f;

			if (Input.GetKey(KeyCode.W)) {
				v++;
				active = true;
			}
			if (Input.GetKey(KeyCode.A)) {
				h--;
				active = true;
			}
			if (Input.GetKey(KeyCode.S)) {
				v--;
				active = true;
			}
			if (Input.GetKey(KeyCode.D)) {
				h++;
				active = true;
			}
			if (Input.GetMouseButtonDown(0) && !(uiManager.MouseOnUI())) {
				active = true;
			}

			if (Input.GetMouseButton(1) && Vector3.Distance(mouseLoc, transform.position) < 0.1f) {
				v = 0.0f;
				h = 0.0f;
			}
			bool moving = h != 0.0f || v != 0.0f;

			//control step
			Turning(active);
			Move(h, v);
			Animating(moving);

			//basic fixes
			//model.transform.localEulerAngles = Vector3.zero;
			//model.transform.localPosition = Vector3.zero;
		}
	}

	void Update() {
		//check for double tap
		if (Input.GetKeyDown(KeyCode.W)) {
			if (buttonCooler > 0.0f && buttonCount == 1) {
				speed = runSpeed;
				anim.SetFloat("Speed", 1.0f);
			} else {
				buttonCooler = doubleTap;
				buttonCount++;
			}
		}
			
		//reset vars for next frame
		if (buttonCooler > 0) {
			buttonCooler -= Time.deltaTime;
		} else if (Input.GetKey(KeyCode.S) || !(Input.GetKey(KeyCode.W))) {
			speed = walkSpeed;
			anim.SetFloat("Speed", 0.2f);
		} else {
			buttonCount = 0;
		}
	}

	void Move(float h, float v) {
		movement = h * transform.right + v * transform.forward;
		movement = movement.normalized * speed * Time.deltaTime;
		rb.MovePosition(transform.position + movement);
		model.transform.localPosition = Vector3.zero;
	}

	void Turning(bool active) {
		if (active) {
			if (Input.GetMouseButton(1)) {
				Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit floorHit;

				if (Physics.Raycast(camRay, out floorHit, camRayLength, floorMask)) {
					mouseLoc = floorHit.point;
					Vector3 playerToMouse = floorHit.point - transform.position;
					playerToMouse.y = 0.0f;

					Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
					rb.MoveRotation(newRotation);
				}
			} else if (playerCompass.transform.localEulerAngles.y != 0.0f) {
				rb.MoveRotation(Quaternion.Euler(0.0f, playerCompass.transform.eulerAngles.y, 0.0f));
				playerCompass.transform.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
			}
		}
	}

	void Animating(bool moving) {
		anim.SetBool("isMoving", moving);
	}
}
