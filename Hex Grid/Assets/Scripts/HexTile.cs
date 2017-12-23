using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTile : MonoBehaviour {

    // Axial coordinates
    private int x, y, z;

    // Cartesian Coordinates
    private float geomX, geomY;
    public float radius = 0.5f;

    // Data
    private int data;
    public bool flag = false;

    private Board board;
    private ArrayList affected;

    // GameObjects
    public GameObject tileObject;

    // Constants
    private float SQRT_3 = Mathf.Sqrt(3f);

    // Static variables
    private static HexTile focusTile = null;
    private static HexTile currTile = null;


    /**
     * Method called upon instantiation
     */
    void Start() {
        SetData(0);
    }

    public void Delete() {
        Destroy(gameObject);
    }

    public int GetX() {
        return x;
    }

    public int GetY() {
        return y;
    }

    public int GetZ() {
        return z;
    }

    public float GetGeomX() {
        return geomX;
    }

    public float GetGeomY() {
        return geomY;
    }

    public bool GetActive() {
        return this.gameObject.activeSelf;
    }

    public HexTile GetFocus() {
        return focusTile;
    }

    public void SetBoard(Board board) {
        this.board = board;
        affected = new ArrayList();
    }

    /**
     * Set the position of the tile given axial coordinates
     * @param x - the x coord
     * @param y - the y coord
     * @param z - the z coord
     */
    public void SetAxial(int x, int y, int z) {
        this.x = x;
        this.y = y;
        this.z = z;

        // Geometric conversion
        geomX = x * radius - z * radius;
        geomY = y * 3 * radius / SQRT_3;

        this.transform.position = new Vector3(geomX, 0, geomY);
    }

    /**
     * Mutator method to set the data variable of the tile
     */
    public void SetData(int data) {
        this.data = data;
        if (data != 0) {
            tileObject.GetComponent<Renderer>().material.color
                = data == 0 ? Color.white : new Color(Mathf.Max(255f - data * 100, 0),
                Mathf.Max(255f - data * 100, 0), Mathf.Max(255f - data * 100, 0));
        } else {
            tileObject.GetComponent<Renderer>().material.color
                = Color.HSVToRGB((30 * (x+50) % 360) / 360f, 0.90f, 0.75f);
        }
    }

    public void SetActive(bool active) {
        this.gameObject.SetActive(active);
    }

    /**
     * Find the gridded distance between this tile and the other
     * @param other - the tile to measure distance between
     * @return the length of the shortest path between the two tiles
     */
    public int GridDistanceFrom(HexTile other) {
        return (Mathf.Abs(this.x - other.GetX()) + Mathf.Abs(this.y - other.GetY()) + Mathf.Abs(this.z - other.GetZ())) / 2;
    }

    /**
     * Find the Euclidean distance between this tile and the other
     * @param other - the tile to measure distance between
     * @return the length of the shortest path between the two tiles
     */
    public float EulDistanceFrom(HexTile other) {
        return Mathf.Sqrt((this.GetGeomX() - other.GetGeomX()) * (this.GetGeomX() - other.GetGeomX())
            + (this.GetGeomY() - other.GetGeomY()) * (this.GetGeomY() - other.GetGeomY()));
    }

    public override string ToString() {
        return ("(" + x + "," + y + "," + z + ") at (" + geomX + "," + geomY + "):\tdata" + data + "\tflag" + flag);
    }












    void OnMouseEnter() {
        if (currTile) {
            foreach (HexTile tile in currTile.affected) {
                tile.SetData(0);
            }
        }
        currTile = this;

        if (focusTile) {
            LinkedList<HexTile> path = board.FindPathAStar(focusTile, this);

            path.Remove(this);
            this.SetData(5);

            foreach (HexTile tile in path) {
                tile.SetData(5);
                affected.Add(tile);
            }
        } else {
            SetData(5);
        }
    }

    private void OnMouseDown() {
        if (focusTile) {
            focusTile = null;
        } else {
            focusTile = this;
        }
    }

    void OnMouseExit() {
        if (focusTile != this) {
            this.SetData(0);
        }
    }
}