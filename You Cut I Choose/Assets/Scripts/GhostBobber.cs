using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostBobber : MonoBehaviour {

    private float y0;

    public Texture smile;
    public Texture frown;

	// Use this for initialization
	void Start () {
        y0 = transform.localPosition.y;
	}
	
	// Update is called once per frame
	void Update () {
        transform.localPosition = new Vector3(transform.localPosition.x,
            y0 + 0.3f * Mathf.Sin(5.0f * Time.time), transform.localPosition.z);
    }

    public void Smile(bool smiling) {
        if (smiling) {
            gameObject.GetComponentInChildren<Renderer>().material.mainTexture = smile;
        } else {
            gameObject.GetComponentInChildren<Renderer>().material.mainTexture = frown;
        }
    }
}
