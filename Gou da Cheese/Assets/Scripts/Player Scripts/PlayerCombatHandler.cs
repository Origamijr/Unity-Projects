using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatHandler : MonoBehaviour {
	private UIManager uiManager;

	private GameObject weapon;
	private Animator anim;

	private float timer;
	private bool attacking;

	private GameObject projectile;

	void Awake() {
		uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
		anim = transform.Find("Gou").GetComponent<Animator>();

		attacking = false;
		timer = 0.0f;
	}

	void Update() {
		weapon = this.GetComponent<InventoryController>().GetWeapon();

		if (Input.GetMouseButtonDown(0) && !(uiManager.MouseOnUI()) || attacking) {
			if (weapon == null) {
				Punch();
			} else if (weapon.CompareTag("Singlestick")) {
				SwingArm();
			} else if (weapon.CompareTag("Bow")) {
				ShootBow();
			}
		}

		timer += Time.deltaTime;
	}

	void Punch() {
		if (!attacking) {
			attacking = true;
			AnimateWithWeaponBool(1, "isPunching", true);
		} else if (Input.GetMouseButtonUp(0)) {
			attacking = false;
			AnimateWithWeaponBool(1, "isPunching", false);
		}
	}

	void SwingArm() {
		if (!attacking) {
			attacking = true;
			AnimateWithWeaponBool(1, "isSwingingR", true);
		} else if (Input.GetMouseButtonUp(0)) {
			attacking = false;
			AnimateWithWeaponBool(1, "isSwingingR", false);
		}
	}

	void ShootBow() {
		if (!attacking) {
			attacking = true;
			timer = 0.0f;
			AnimateWithWeaponBool(1, "isShooting", true);
		} else if (Input.GetMouseButtonUp(0) && timer < 0.5f) {
			attacking = false;
			AnimateWithWeaponBool(1, "isShooting", false);
		} else if (Input.GetMouseButtonUp(0) && timer < 1.0f) {
			attacking = false;
			AnimateWithWeaponBool(1, "isShooting", false);
			ShootArrow(timer * 40.0f);
		} else if (Input.GetMouseButtonUp(0)) {
			attacking = false;
			AnimateWithWeaponBool(1, "isShooting", false);
			ShootArrow(50.0f);
		} else if (timer >= 0.5f && projectile == null) {
			projectile = Utility.FindChildWithName(weapon, "Dummy Arrow").gameObject;
			projectile.SetActive(true);
		}
	}

	void ShootArrow(float speed) {
		projectile.SetActive(false);
		GameObject arrow = Utility.FindChildWithName(weapon, "Arrow").gameObject;
		GameObject tail = Utility.FindChildWithName(weapon, "Tail").gameObject;
		projectile = (GameObject)Instantiate(arrow, arrow.transform.position, transform.rotation);
		GameObject dragger = (GameObject)Instantiate(tail, tail.transform.position, transform.rotation);
		projectile.SetActive(true);
		dragger.SetActive(true);
		projectile.GetComponent<FixedJoint>().connectedBody = dragger.GetComponent<Rigidbody>();
		projectile.GetComponent<ProjectileMover>().SetSpeed(speed);
		projectile.GetComponent<ProjectileMover>().SetDirection(transform.forward);
		projectile = null;
	}

	void AnimateWithWeaponBool(int layer, string boolVar, bool attacking) {
		if (weapon != null) {
			if (weapon.GetComponent<Animator>() != null) {
				weapon.GetComponent<Animator>().SetBool(boolVar, attacking);
			}
		}
		if (attacking) {
			anim.SetLayerWeight(layer, 1.0f);
		} else {
			anim.SetLayerWeight(layer, 0.0f);
		}
		anim.SetBool(boolVar, attacking);
	}
}
