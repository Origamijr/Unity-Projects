using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {

    private HexTile[][][] board;
    public GameObject tilePrefab;

    public int radius = 5;

    // Use this for initialization
    void Start() {
        BoardWithRadius(radius);
    }

    void BoardWithRadius(int r) {
        int diameter = 2 * r + 1;

        board = new HexTile[diameter][][];
        for (int i = 0, x = -r; i < diameter; i++, x++) {
            board[i] = new HexTile[diameter][];
            for (int j = 0, y = -r; j < diameter; j++, y++) {
                board[i][j] = new HexTile[diameter];
                for (int k = 0, z = -r; k < diameter; k++, z++) {
                    if (x + y + z == 0) {
                        // Create new tile
                        GameObject tile = Instantiate(tilePrefab);

                        // Put the tile's script into the tile array
                        board[i][j][k] = tile.GetComponent<HexTile>();
                        board[i][j][k].SetAxial(x, y, z);
                    }
                }
            }
        }
    }


    HexTile GetTile(int x, int y, int z) {
        if ((x >= -radius || x <= radius) && (y >= -radius || y <= radius) && (z >= -radius || z <= radius)) {
            return board[x + radius][y + radius][z + radius];
        }

        return null;
    }


    ArrayList GetNeighbors(int x, int y, int z) {
        ArrayList neighbors = new ArrayList();

        for (int i = -1; i <= 1; i++) {
            for (int j = -1; j <= 1; j++) {
                for (int k = -1; k <= 1; k++) {
                    if (i + j + k == 0) {
                        HexTile neighbor = GetTile(x + i, y + j, z + k);
                        if (neighbor) {
                            neighbors.Add(neighbor);
                        }
                    }
                }
            }
        }

        return neighbors;
    }

    
    ArrayList FindPathAStar(HexTile from, HexTile dest) {
        ArrayList path = new ArrayList();

        
    }
}