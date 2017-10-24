using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour {
    // Walkable tile
    public GameObject tileObject;

    // Array of tiles forming the terrain
    private TileManager[][] tileData;

    //Temp setting for public test variables
    public int rows;
    public int cols;

    public GameObject gou, del;

    // Use this for initialization
    void Start() {
        // Initialize the grid double array to be zero filled
        int[][] grid = new int[rows][];
        for (int row = 0; row < rows; row++) {
            grid[row] = new int[cols];

            for (int col = 0; col < cols; col++) {
                grid[row][col] = 0;
            }
        }

        tileData = GenerateTiles(grid);
        tileData[2][1].SnapObject(gou);
        tileData[1][1].SnapObject(del);
    }

    // Update is called once per frame
    void Update() {

    }

    /*
     * Generate tile grid from a two-dimensional array
     */
    TileManager[][] GenerateTiles(int[][] grid) {
        TileManager[][] tiles = new TileManager[grid.Length][];

        for (int row = 0; row < grid.Length; row++) {
            tiles[row] = new TileManager[grid[row].Length];

            for (int col = 0; col < grid[row].Length; col++) {
                // Create new tile
                GameObject tile = Instantiate(tileObject);

                // Put the tile's script into the tile array
                tiles[row][col] = tile.GetComponent<TileManager>();

                // Move the tile to the appropriate position and rotation
                tile.transform.position = new Vector3(row, 0.05f, col);
                tile.transform.rotation = Quaternion.Euler(90, 0, 0);

                // hide grid if inactive
                tiles[row][col].ShowGrid(grid[row][col] == 0);
            }
        }

        return tiles;
    }
}
