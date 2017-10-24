using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour {
    public Texture[] textures;

    private int score;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Snaps an object to this tile
    public void SnapObject(GameObject thing) {
        thing.transform.SetParent(this.transform);
        thing.transform.localPosition = new Vector3(0, 0, 0);
    }

    // Change color of tile
    public void Highlight(bool highlighted) {
        if (highlighted) {
            if (score == 0) {
                gameObject.GetComponent<Renderer>().material.mainTexture = textures[textures.Length - 1];
            } else {
                gameObject.GetComponent<Renderer>().material.mainTexture = textures[score + 2];
            }
        } else {
            if (score == 0) {
                gameObject.GetComponent<Renderer>().material.mainTexture = textures[0];
            } else {
                gameObject.GetComponent<Renderer>().material.mainTexture = textures[score - 4];
            }
        }
    }

    // Sets the score the tile gives
    public void SetScore(int i) {
        score = i;
    }

    // Get the score of the tile
    public int GetScore() {
        return score;
    }

}
