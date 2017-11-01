using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTile : MonoBehaviour {

    // Axial coordinates
    public int x, y, z;

    // Cartesian Coordinates
    private int geomX, geomY;

    // Array Coordinates
    private int coordX, coordY;

	
    // Use this for initialization
	void Start () {
        coordX = ;
        coordY = -z;
	}


    // Set all coordinates with axial coordinate inputs
    public void SetAxial(int x, int y, int z) {
        // Geometric conversion

        // Array coord conversion
        coordY = y;
        coordX = y / 2 + x;
    }


    // Set all coordinates with geometric coordinate inputs
    public void SetGeometric(int x, int y) {
        // Axial conversion 

    }


    // Set all coordinates with array coordinate inputs
    public void SetCoord(int x, int y) {
        // Axial conversion

        // Geometric conversion
        geomY = coordY;
    }
	

	// Update is called once per frame
	void Update () {
		
	}
}
