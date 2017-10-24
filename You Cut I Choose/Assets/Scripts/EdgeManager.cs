using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeManager : MonoBehaviour {

    public GameObject wallObject;
    private bool wall = false;
    private Vector3 wallVec;
    public float speed = 10.0f;
    private float error = 0.1f;
    private bool moved;

    private int id;
    public GameObject idText;

    public GameObject dust;

    private AudioSource audioSource;
    public AudioClip slam;

	// Use this for initialization
	void Start () {
        wallVec = Vector3.zero;

        audioSource = gameObject.GetComponent<AudioSource>();
	}

    // Update is called once per frame
    void Update() {
        if (Vector3.Distance(wallVec, Vector3.zero) > error) {
            moved = true;
            Vector3 stepVec = wallVec.normalized * speed * Time.deltaTime;
            wallObject.transform.localPosition += stepVec;
            wallVec -= stepVec;
        } else if (moved) {
            if (wall) {
                wallObject.transform.localPosition = new Vector3(0, 0, 2);
            } else {
                wallObject.transform.localPosition = Vector3.zero;
            }

            wall = !wall;
            moved = false;
            dust.SetActive(false);
            audioSource.PlayOneShot(slam);
        }
    }

    // Change color of edge
    public void Highlight(bool highlighted) {
        if (highlighted) {
            gameObject.GetComponent<Renderer>().material.color = Color.red;
        } else {
            gameObject.GetComponent<Renderer>().material.color = Color.black;
        }
    }

    // Check wall status
    public bool HasWall() {
        return wall;
    }

    // Set wall status
    public void Wall(bool set) {
        if (set && !wall) {
            wallVec = new Vector3(0, 0, -2);
            dust.SetActive(true);
        } else if (!set && wall) {
            wallVec = new Vector3(0, 0, 2);
            dust.SetActive(true);
        }
    }

    // get the id value
    public int GetId() {
        return id;
    }

    // set the id value
    public void SetId(int i) {
        id = i;
        idText.GetComponent<TextMesh>().text = "" + id;
    }

    // Show the id text
    public void ShowId(bool active) {
        idText.SetActive(active);
    }
}
