using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {

    private HexTile[][][] board;
    public GameObject tilePrefab;

    private int xLen = 11;
    private int yLen = 11;
    private int zLen = 11;

    private int cenX = 5;
    private int cenY = 5;
    private int cenZ = 5;

    private class Node : IComparable<Node> {
        public HexTile tile;
        public float f, g, h;
        public Node prev;

        public Node(HexTile tile, float g, float h, Node prev) {
            this.tile = tile;
            this.g = g;
            this.h = h;
            this.f = g + h;
            this.prev = prev;
        }

        public HexTile GetTile() {
            return tile;
        }

        public int CompareTo(Node other) {
            if (this.f < other.f) {
                return 1;
            }

            return -1;
        }
    }

    // Use this for initialization
    void Start() {
        BoardWithRadius(20);

        SetCell(board, false);

        SetFlag(board, true);

        PopulateWithCells(2, 10, 2, 15);
        
        CutInactive();
    }

    void BoardWithRadius(int r) {
        cenX = cenY = cenZ = r;
        xLen = yLen = zLen = 2 * r + 1;

        board = new HexTile[xLen][][];
        for (int i = 0, x = -cenX; i < xLen; i++, x++) {
            board[i] = new HexTile[yLen][];
            for (int j = 0, y = -cenY; j < yLen; j++, y++) {
                board[i][j] = new HexTile[zLen];
                for (int k = 0, z = -cenZ; k < zLen; k++, z++) {
                    if (x + y + z == 0) {// && (Math.Abs(x) + Math.Abs(y) + Math.Abs(z)) / 2 > r / 2) {
                        // Create new tile
                        GameObject tile = Instantiate(tilePrefab);

                        // Put the tile's script into the tile array
                        board[i][j][k] = tile.GetComponent<HexTile>();
                        board[i][j][k].SetAxial(x, y, z);
                        board[i][j][k].SetBoard(this);
                    }
                }
            }
        }
    }

    void PopulateWithCells(int minR, int maxR, int gap, int maxCells) {
        
        ArrayList cells = new ArrayList();
        ArrayList radii = new ArrayList();
        for (int circles = 0; circles < maxCells; circles++) {
            // Check if I can still place a circle
            ArrayList slots = new ArrayList();
            bool open = false;
            for (int i = minR; i < xLen - minR; i++) {
                for (int j = minR; j < yLen - minR; j++) {
                    for (int k = minR; k < zLen - minR; k++) {
                        if (board[i][j][k] && board[i][j][k].flag) {
                            open = true;
                            slots.Add(board[i][j][k]);
                        }
                    }
                }
            }

            if (open) {
                int rand = UnityEngine.Random.Range(0, slots.Count - 1);
                HexTile center = (HexTile) slots[rand];

                // Find the maximum radius
                maxR = Mathf.Min(maxR, Mathf.Min(cenX + center.GetX(), xLen - (cenX + center.GetX())),
                    Mathf.Min(cenY + center.GetY(), yLen - (cenY + center.GetY())),
                    Mathf.Min(cenZ + center.GetZ(), zLen - (cenZ + center.GetZ())));
                for (int i = 0; i < cells.Count; i++) {
                    int dist = ((HexTile) cells[i]).GridDistanceFrom(center) - (int) radii[i] - gap;
                    maxR = Mathf.Min(maxR, dist);
                }

                int radius = UnityEngine.Random.Range(minR, maxR);

                cells.Add(center);
                radii.Add(radius);

                // Set the cell
                HexTile[][][] cell = GetCell(center.GetX(), center.GetY(), center.GetZ(), radius);
                SetCell(cell, true);

                HexTile[][][] influence = GetCell(center.GetX(), center.GetY(), center.GetZ(), radius + minR + gap);
                SetFlag(influence, false);
            } else {
                break;
            }
        }


        // Roughly connect cells
        for (int i = 0; i < cells.Count - 1; i++) {
            LinkedList<HexTile> path = FindPathAStar((HexTile) (cells[i]), (HexTile) (cells[i + 1]));
            foreach (HexTile tile in path) {
                tile.SetActive(true);
            }
        }

    }

    void CutInactive() {
        for (int i = 0; i < xLen; i++) {
            for (int j = 0; j < yLen; j++) {
                for (int k = 0; k < zLen; k++) {
                    if (board[i][j][k] && !(board[i][j][k].GetActive())) {
                        board[i][j][k].Delete();
                        board[i][j][k] = null;
                    }
                }
            }
        }
    }


    bool InBoard(int x, int y, int z) {
        return (x >= 0 && x < xLen) && (y >= 0 && y < yLen) && (z >= 0 && z < zLen);
    }

    HexTile GetTile(int x, int y, int z) {
        if (InBoard(cenX + x, cenY + y, cenZ + z)) {
            HexTile tile = board[x + cenX][y + cenY][z + cenZ];
            if (tile && tile.GetActive()) {
                return tile;
            }
        }

        return null;
    }

    /**
     * Extracts out a hex cell from the board
     */
    HexTile[][][] GetCell(int x, int y, int z, int r) {
        // Bound the cell to the board
        int minX = Mathf.Max(0, cenX + x - r);
        int minY = Mathf.Max(0, cenY + y - r);
        int minZ = Mathf.Max(0, cenZ + z - r);

        int maxX = Mathf.Min(xLen - 1, cenX + x + r);
        int maxY = Mathf.Min(yLen - 1, cenY + y + r);
        int maxZ = Mathf.Min(zLen - 1, cenZ + z + r);

        int xr = maxX - minX + 1;
        int yr = maxY - minY + 1;
        int zr = maxZ - minZ + 1;

        HexTile[][][] cell = new HexTile[xr][][];
        for (int i = 0, i2 = minX; i < xr; i++, i2++) {
            cell[i] = new HexTile[yr][];
            for (int j = 0, j2 = minY; j < yr; j++, j2++) {
                cell[i][j] = new HexTile[zr];
                for (int k = 0, k2 = minZ; k < zr; k++, k2++) {
                    cell[i][j][k] = board[i2][j2][k2];
                }
            }
        }
        
        return cell;
    }

    void SetCell(HexTile[][][] cell, bool active) {
        for (int i = 0; i < cell.Length; i++) {
            for (int j = 0; j < cell[i].Length; j++) {
                for (int k = 0; k < cell[i][j].Length; k++) {
                    if (cell[i][j][k]) {
                        cell[i][j][k].SetActive(active);
                    }
                }
            }
        }
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

    public ArrayList GetNeighbors(HexTile tile) {
        return GetNeighbors(tile.GetX(), tile.GetY(), tile.GetZ());
    } 

    void SetFlag(HexTile[][][] cell, bool flag) {
        for (int i = 0; i < cell.Length; i++) {
            for (int j = 0; j < cell[i].Length; j++) {
                for (int k = 0; k < cell[i][j].Length; k++) {
                    if (cell[i][j][k]) {
                        cell[i][j][k].flag = flag;
                    }
                }
            }
        }
    }


    public LinkedList<HexTile> FindPathAStar(HexTile from, HexTile dest) {
        LinkedList<HexTile> path = new LinkedList<HexTile>();

        // Set all the tiles to unvisited
        SetFlag(board, false);

        // Create a priority queue based on sum of distance and euler distance
        PriorityQueue<Node> queue = new PriorityQueue<Node>();
        Node curr = new Node(from, 0, from.EulDistanceFrom(dest), null);
        queue.Push(curr);
        
        while (!(queue.IsEmpty())) {
            Node popped = queue.Pop();
            if (popped.tile == dest) {
                curr = popped;
                break;
            }
            ArrayList neighbors = GetNeighbors(popped.tile);

            foreach (HexTile neighbor in neighbors) {
                if (!(neighbor.flag)) {
                    neighbor.flag = true;
                    queue.Push(new Node(neighbor, popped.g + 1,
                        neighbor.EulDistanceFrom(dest), popped));
                }
            }
        }

        while (curr != null) {
            path.AddFirst(curr.tile);
            curr = curr.prev;
        }

        return path;
    }
}