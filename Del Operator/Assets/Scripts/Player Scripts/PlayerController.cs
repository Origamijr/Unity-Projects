using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    // Declare a variable for the UI Manager
    // private UIManager uiManager;

    // Declare private variable's for the player's avatar
	private GameObject model;
	private Vector3 movement;
	private Animator anim;
	private Rigidbody rb;
    
    // Constants for the walk and run speed
    private const float WALK_SPEED = 1f;
    private const float RUN_SPEED = WALK_SPEED * 2;
    private float speed;

    // Direction variable
    private Quaternion direction;

    /*
     * Method Name: Awake()
     * Description: Constructs the instance by assigning values to variables
     */
	void Awake() {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            
		// uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();

		model = transform.Find("Gou").gameObject;
		anim = model.GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        
        speed = WALK_SPEED;
	}

    /*
     * Method Name: Update()
     * Description: Updates every cycle to check for input from devices and move
     *              the character accordingly.
     */
	void Update() {
		if (!(UIManager.GetPaused())) {
			// Check input
			float horizontal = 0.0f;
			float vertical = 0.0f;

			if (Input.GetKey(KeyCode.W)) {
				vertical++;
			}
			if (Input.GetKey(KeyCode.A)) {
				horizontal--;
			}
			if (Input.GetKey(KeyCode.S)) {
				vertical--;
			}
			if (Input.GetKey(KeyCode.D)) {
				horizontal++;
			}

			bool moving = horizontal != 0.0f || vertical != 0.0f;

			//control step
			Turn(horizontal, vertical);
			Move(horizontal, vertical);

            // Animate movement
            anim.SetBool("isMoving", moving);
        }
	}

	void Move(float h, float v) {
        // Normalize the movement vector
		movement = h * transform.right + v * transform.forward;

        // Scale the movement vector
		movement = movement.normalized * speed * Time.deltaTime;

        // Move the player model
		rb.MovePosition(transform.position + movement);

        // Zero the location
		model.transform.localPosition = Vector3.zero;
	}

    void MoveTo(float x, float z) {

    }

	void Turn(float h, float v) {
		
	}
}
