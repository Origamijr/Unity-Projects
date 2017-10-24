using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMover : MonoBehaviour {
	public Rigidbody rb;
	private float speed = 0.0f;
	private Vector3 direction = Vector3.zero;

	public float GetSpeed() {
		return this.speed;
	}

	public void SetSpeed(float speed) {
		this.speed = speed;
		rb.velocity = direction * speed;
	}

	public void SetDirection(Vector3 direction) {
		this.direction = direction;
		rb.velocity = direction * speed;
	}

	void Awake() {
		rb = transform.GetComponent<Rigidbody>();
		rb.velocity = direction * speed;
	}

	void OnCollisionEnter() {
		SetSpeed(0.0f);
		rb.isKinematic = true;
		if (this.GetComponent<FixedJoint>() != null) {
			this.GetComponent<FixedJoint>().connectedBody.velocity = Vector3.zero;
			this.GetComponent<FixedJoint>().connectedBody.isKinematic = true;
		}
	}
}
