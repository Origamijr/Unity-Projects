using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /*
     * Hide the grid based on the passed parameter
     */
    public void ShowGrid(bool visible) {
        this.gameObject.SetActive(visible);
    }

    /* 
     * Snaps an object to this tile
     */
    public void SnapObject(GameObject thing) {
        thing.transform.SetParent(this.transform);
        thing.transform.localPosition = new Vector3(0, 0, 0);
    }
}
