using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager: MonoBehaviour {
	public string activeLocation;
	private Transform activeTransform;
	public Vector3 activePosition;
	public Vector3 activeEulerAngles;

	public string inactiveLocation;
	private Transform inactiveTransform;
	public Vector3 inactivePosition;
	public Vector3 inactiveEulerAngles;

	private GameObject player;

	void Awake() {
		player = GameObject.FindWithTag("Player");
		activeTransform = Utility.FindChildWithName(player, activeLocation);
		inactiveTransform = Utility.FindChildWithName(player, inactiveLocation);
	}

	public void Equip(bool active) {
		if (active) {
			transform.SetParent(activeTransform);
			transform.localPosition = activePosition;
			transform.localEulerAngles = activeEulerAngles;
		} else {
			transform.SetParent(inactiveTransform);
			transform.localPosition = inactivePosition;
			transform.localEulerAngles = inactiveEulerAngles;
		}
	}
}
