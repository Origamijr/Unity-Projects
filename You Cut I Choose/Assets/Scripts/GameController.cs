using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    // GameObject Variables
    public GameObject neWall;
    public GameObject player;
    public GameObject tileObject;
    public GameObject edgeObject;
    public GameObject ghost;
    public Camera mainCamera;

    // Board variables
    public int boardSize = 11;
    private TileManager[][] board;
    private EdgeManager[][] edges;
    public float floorDip = -0.1f;
    private int lowRow, highRow, lowCol, highCol;

    // Cursor control
    private TileManager currTile;
    private EdgeManager currEdge;
    private int currRow, currCol;
    private int hori, vert;

    private float cursorTick = 0.1f;
    private float buttonCooler = 0.0f;
    private bool moved = false;
    private bool inputBlocked = false;

    // Player Action variables
    private int playerRow, playerCol;
    private bool playerMoving = false;
    private Vector3 playerMoveVec;
    private float speed = 2.5f;
    private float error = 0.1f;

    private bool wallBuilding = false;
    private bool wallRaising = false;
    private EdgeManager nextEdge = null;
    private int nextRow, nextCol;
    private int memRow, memCol;

    // Gameflow variables
    private bool playerTurn = false;
    private bool firstTurn;

    // Score stuff
    private int score;
    public Text scoreText;
    public int[,] tileScores = {
        {10, 6, 7, 6, 8, 9, 8, 6, 7, 6, 10},
        {6, 5, 6, 8, 7, 5, 7, 8, 6, 5, 6},
        {7, 6, 5, 6, 5, 6, 5, 6, 5, 6, 7},
        {6, 8, 6, 5, 7, 8, 7, 5, 6, 8, 6},
        {8, 7, 5, 7, 5, 6, 5, 7, 5, 7, 8},
        {9, 5, 6, 8, 6, 5, 6, 8, 6, 5, 9},
        {8, 7, 5, 7, 5, 6, 5, 7, 5, 7, 8},
        {6, 8, 6, 5, 7, 8, 7, 5, 6, 8, 6},
        {7, 6, 5, 6, 5, 6, 5, 6, 5, 6, 7},
        {6, 5, 6, 8, 7, 5, 7, 8, 6, 5, 6},
        {10, 6, 7, 6, 8, 9, 8, 6, 7, 6, 10}
    };

    // Edge indexing
    private int[] idTable;
    private string idString;

    // UI Stuff
    public Text turnText;
    public GameObject gameOverScreen;
    public GameObject instructionScreen;
    public InputField idField;
    public Text gmScore;

    // Camera stuff
    private Vector3 cameraMovVec;
    private bool panning;
    private bool panned;

    // Audio stuff
    public AudioClip rumble;
    public AudioClip gameOver;
    public AudioClip whistle;
    private AudioSource audioSource;

    // Use this for initialization
    void Start() {
        GenerateIds();
        MakeBoard();

        // Enable ui
        audioSource = gameObject.GetComponent<AudioSource>();
        idField.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(true);
        turnText.gameObject.SetActive(true);
        gameOverScreen.SetActive(false);
        panning = panned = false;

        // Move the NEWall to the right location
        neWall.transform.position = new Vector3(boardSize, 0, boardSize);

        // Put gamepiece on board
        playerRow = playerCol = 0;
        currTile = board[playerRow][playerCol];
        board[playerRow][playerCol].SnapObject(player);
        ghost.GetComponent<GhostBobber>().Smile(true);

        // Set the player bounds
        lowRow = lowCol = 0;
        highRow = highCol = boardSize - 1;

        // Set Gameflow
        firstTurn = true;
        playerTurn = false;
        turnText.text = "Edgelord's Turn!";
        FindNextEdge();
        score = 0;
    }


    // Update is called once per frame
    void Update() {
        // Check if input is allowed
        if (!inputBlocked) {
            // Check if it's the cursor's time to move
            if (buttonCooler == 0) {
                
                // Update coordinates
                hori = (int) Input.GetAxis("Horizontal");
                vert = (int) Input.GetAxis("Vertical");
                if (currRow != (currRow += hori) || currCol != (currCol += vert)) {
                    moved = true;
                }

                // Check whose turn it is to control the cursor
                if (playerTurn) {
                    // Correct coordinates
                    PlayerCoordCorrect();

                    // Update current tile
                    currTile.Highlight(false);
                    currTile = board[currRow][currCol];
                    currTile.Highlight(true);
                } else {
                    // It is edgelord's turn
                    // correct coordinates
                    EdgelordCoordCorrect();

                    // Update current edge
                    currEdge.Highlight(false);
                    currEdge = edges[currRow][currCol];
                    currEdge.Highlight(true);
                }

                // Set buttonCooler on a cooldown
                if (moved) {
                    buttonCooler = cursorTick;
                    moved = false;
                }
            }

            // Check if the enter key was pressed
            if (Input.GetKeyDown(KeyCode.Return)) {
                if (playerTurn) {
                    // Sets up the player to move
                    playerMoving = true;
                    playerMoveVec = new Vector3(currRow - playerRow, currCol - playerCol, 0);

                    //Rotate the player
                    if (!(playerMoveVec.Equals(Vector3.zero))) {
                        audioSource.PlayOneShot(whistle, 0.3f);
                        player.transform.rotation = Quaternion.LookRotation(
                            new Vector3(-playerMoveVec.x, 0, -playerMoveVec.y));
                    }

                    inputBlocked = true;
                } else {
                    idString = idField.text;
                    bool found = false;

                    // Check if an id was used
                    if (idString.Length > 0) {
                        int id = int.Parse(idString);

                        // find id
                        for (int i = 0; i < edges.Length; i++) {
                            for (int j = 0; j < edges[i].Length; j++) {
                                if (edges[i][j].GetId() == id) {
                                    found = true;

                                    currRow = i;
                                    currCol = j;
                                }
                            }
                        }

                        idField.text = "";
                    } 
                    if (found || idString.Length == 0) {
                        // The edgelord prepares to BUILD A FRIGGIN WALL
                        currEdge.Highlight(false);

                        // Find out if the wall is vertical or horizontal, then get start point
                        if (currRow % 2 == 0) {
                            // Horizontal, to transverse left to right
                            while (currRow > 0 && !(edges[currRow - 1][currCol].HasWall())) {
                                currRow -= 2;
                            }

                        } else {
                            // Vertical, to transverse up to down
                            while (currCol < boardSize - 1 && !(edges[currRow - 1][currCol].HasWall())) {
                                currCol++;
                            }
                        }

                        nextRow = currRow;
                        nextCol = currCol;
                        nextEdge = null;

                        memRow = currRow;
                        memCol = currCol;

                        if (firstTurn) {
                            playerTurn = true;
                            turnText.text = "Ghost's Turn!";
                            currRow = playerRow;
                            currCol = playerCol;

                            firstTurn = false;
                        } else {
                            currRow = playerRow;
                            currCol = playerCol;
                            playerTurn = true;
                            turnText.text = "Ghost's Turn!";
                        }
                    }
                }
            }
        }

        // Show/hide ids
        if (Input.GetKeyDown(KeyCode.Tab)) {
            for (int i = 0; i < edges.Length; i++) {
                for (int j = 0; j < edges[i].Length; j++) {
                    edges[i][j].ShowId(true);
                }
            }
        } else if (Input.GetKeyUp(KeyCode.Tab)) {
            for (int i = 0; i < edges.Length; i++) {
                for (int j = 0; j < edges[i].Length; j++) {
                    edges[i][j].ShowId(false);
                }
            }
        }

        // Show/hide instructions
        if (Input.GetKeyDown(KeyCode.I)) {
            instructionScreen.SetActive(true);
        } else if (Input.GetKeyUp(KeyCode.I)) {
            instructionScreen.SetActive(false);
        }

        // Handle input tick cooldowns
        if (buttonCooler > 0) {
            buttonCooler -= Time.deltaTime;
        } else if (buttonCooler <= 0) {
            buttonCooler = 0;
        }

        // Handle player movement
        if (playerMoving) {
            // Move if still reasonably far from target location
            if (Vector3.Distance(playerMoveVec, Vector3.zero) > error) {
                Vector3 stepVec = playerMoveVec.normalized * speed * Time.deltaTime;
                player.transform.localPosition += stepVec;
                playerMoveVec -= stepVec;
            } else {
                // Stop moving and set position
                board[currRow][currCol].SnapObject(player);
                player.transform.rotation = Quaternion.Euler(0, 0, 0);
                playerRow = currRow;
                playerCol = currCol;

                playerMoving = false;

                // Record score
                score += board[currRow][currCol].GetScore();
                scoreText.text = "Score: " + score;
                board[currRow][currCol].SetScore(0);

                // Switch turn to the edgelord
                playerTurn = false;
                turnText.text = "Edgelord's Turn!";
                board[currRow][currCol].Highlight(false);

                // Build wall
                wallBuilding = true;
                audioSource.PlayOneShot(rumble);
                wallRaising = false;

                FindNextEdge();
            }
        }

        // Handle wall building
        if (wallBuilding) {
            if (!wallRaising) {
                // Raise a wall if no wall is being raised
                edges[memRow][memCol].Wall(true);
                wallRaising = true;

                // Find out if the wall is vertical or horizontal, then find the next wall to raise
                if (memRow % 2 == 0) {
                    // Horizontal
                    nextRow = memRow + 2;
                    if (nextRow >= edges.Length || edges[nextRow - 1][memCol].HasWall()) {
                        nextEdge = null;
                    } else {
                        nextEdge = edges[nextRow][memCol];
                    }
                } else {
                    // Vertical
                    nextCol = memCol - 1;
                    if (nextCol < 0 || edges[memRow - 1][nextCol].HasWall()) {
                        nextEdge = null;
                    } else {
                        nextEdge = edges[memRow][nextCol];
                    }
                }
            } else {
                // Wait til wall is not being raised
                if (edges[memRow][memCol].HasWall()) {
                    wallRaising = false;

                    if (nextEdge) {
                        memRow = nextRow;
                        memCol = nextCol;
                    } else {
                        // stop audio
                        audioSource.Stop();

                        if (memRow % 2 == 0) {
                            // Limit player vertically
                            if (memRow / 2 >= lowRow && memRow / 2 <= highRow) {
                                if (memCol < playerCol) {
                                    lowCol = memCol + 1;
                                } else {
                                    highCol = memCol;
                                }
                            }
                        } else {
                            // Limit player horizontally
                            if (memCol >= lowCol && memCol <= highCol) {
                                if (memRow / 2 < playerRow) {
                                    lowRow = memRow / 2 + 1;
                                } else {
                                    highRow = memRow / 2;
                                }
                            }
                        }

                        // Pass turn to edgelord
                        wallBuilding = false;
                        playerTurn = false;
                        turnText.text = "Edgelord's Turn!";
                        inputBlocked = false;

                        FindNextEdge();
                    }
                }
            }
        }

        // Check Win Condition
        if (lowRow == highRow && lowCol == highCol) {
            inputBlocked = true;

            // Disable ui
            idField.gameObject.SetActive(false);
            scoreText.gameObject.SetActive(false);
            turnText.gameObject.SetActive(false);

            if (panning) {
                if(Vector3.Distance(cameraMovVec, Vector3.zero) > 5 * error) {
                    Vector3 stepVec = cameraMovVec.normalized * 25.0f * Time.deltaTime;
                    mainCamera.transform.localPosition += stepVec;
                    cameraMovVec -= stepVec;
                } else {
                    // Stop moving and set position
                    panned = true;
                    panning = false;
                }
            } else if (panned) {
                mainCamera.transform.position = player.transform.position + (new Vector3(0, 10, -1));
                ghost.GetComponent<GhostBobber>().Smile(false);
                gmScore.text = "Score: " + score;
                gameOverScreen.SetActive(true);
                if (Input.GetKeyDown(KeyCode.R)) {
                    Scene loadedLevel = SceneManager.GetActiveScene();
                    SceneManager.LoadScene(loadedLevel.buildIndex);
                }
            } else {
                // Play gameover audio
                audioSource.PlayOneShot(gameOver);

                cameraMovVec = player.transform.position + (new Vector3(0, 10, -1)) - mainCamera.transform.position;
                panning = true;
            }
            
        }

        // Check Quitting you quitter
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    // Create a list of random ids
    void GenerateIds() {
        idTable = new int[2 * boardSize * (boardSize - 1)];

        for (int i = 0; i < idTable.Length; i++) {
            bool reroll = false;
            int rand = (int) Random.Range(1000.0f, 9999.999f);
            for (int j = 0; j < i; j++) {
                if (idTable[j] == rand) {
                    reroll = true;
                }
            }
            if (reroll) {
                i--;
            } else {
                idTable[i] = rand;
            }
        }
    }

    // Create the board and edges as arrays
    void MakeBoard() {
        // Make and set tiles
        board = new TileManager[boardSize][];

        for (int row = 0; row < boardSize; row++) {
            board[row] = new TileManager[boardSize];

            for (int col = 0; col < boardSize; col++) {
                // Create new tile
                GameObject tile = Instantiate(tileObject);

                // Put the tile's script into the tile array
                board[row][col] = tile.GetComponent<TileManager>();

                // set the score and texture
                tile.GetComponent<TileManager>().SetScore(tileScores[row, col]);
                tile.GetComponent<TileManager>().Highlight(false);

                // Move the tile to the appropriate position and rotation
                tile.transform.position = new Vector3(row, 0, col);
                tile.transform.rotation = Quaternion.Euler(90, 0, 0);
                tile.transform.localScale = new Vector3(0.9f, 0.9f, 1);
            }
        }

        // Make and set edges
        edges = new EdgeManager[2 * boardSize - 1][];
        int i = 0;

        for (int row = 0; row < 2 * boardSize - 1; row++) {
            // Create a jagged array of edges
            if (row % 2 == 0) {
                // Case for vertical edges
                edges[row] = new EdgeManager[boardSize - 1];

                for (int col = 0; col < boardSize - 1; col++) {
                    // Create new edge
                    GameObject edge = Instantiate(edgeObject);

                    // Put the edge's script into the edge array
                    edges[row][col] = edge.GetComponent<EdgeManager>();
                    edges[row][col].SetId(idTable[i]);
                    i++;

                    // Move the edge to the appropriate position and rotation
                    edge.transform.position = new Vector3(row / 2, floorDip, col + 0.5f);
                    edge.transform.rotation = Quaternion.Euler(90, -45, 0);
                    edge.transform.localScale = new Vector3(0.70711f, 0.70711f, 1);
                }
            } else {
                // Case for horizontal edges
                edges[row] = new EdgeManager[boardSize];

                for (int col = 0; col < boardSize; col++) {
                    // Create new edge
                    GameObject edge = Instantiate(edgeObject);

                    // Put the edge's script into the edge array
                    edges[row][col] = edge.GetComponent<EdgeManager>();
                    edges[row][col].SetId(idTable[i]);
                    i++;

                    // Move the edge to the appropriate position and rotation
                    edge.transform.position = new Vector3(row / 2 + 0.5f, floorDip, col);
                    edge.transform.rotation = Quaternion.Euler(90, 45, 0);
                    edge.transform.localScale = new Vector3(0.70711f, 0.70711f, 1);
                }
            }
        }
    }


    // Correct the player's coordinates to land within bounds
    void PlayerCoordCorrect() {
        // Check in bounds
        if (currRow < lowRow) {
            currRow = lowRow;
        } else if (currRow > highRow) {
            currRow = highRow;
        }
        if (currCol < lowCol) {
            currCol = lowCol;
        } else if (currCol > highCol) {
            currCol = highCol;
        }
    }


    // Correct the edgelord's coordinates to land within validity
    void EdgelordCoordCorrect() {
        // Edgelord edgecase correction
        if (hori != 0 && currRow % 2 == 0 && currCol == boardSize - 1) {
            if (hori > 0) {
                currRow++;
            } else {
                currRow--;
            }
        }

        // Left jump
        while (currRow >= 0 && hori < 0 && edges[currRow][currCol].HasWall()) {
            if (currRow % 2 != 0 && currCol == boardSize - 1) {
                currRow -= 2;
            } else {
                currRow--;
            }
        }

        // Left Bound
        while (currRow < 0 || hori < 0 && edges[currRow][currCol].HasWall()) {
            if (currRow % 2 != 0 && currCol == boardSize - 1) {
                currRow += 2;
            } else {
                currRow++;
            }
        }

        // Right jump
        while (currRow < edges.Length && hori > 0 && edges[currRow][currCol].HasWall()) {
            if (currRow % 2 != 0 && currCol == boardSize - 1) {
                currRow += 2;
            } else {
                currRow++;
            }
        }

        // Right Bound
        while (currRow >= edges.Length || hori > 0 && edges[currRow][currCol].HasWall()) {
            if (currRow % 2 != 0 && currCol == boardSize - 1) {
                currRow -= 2;
            } else {
                currRow--;
            }
        }

        // Down jump
        while (currCol >= 0 && vert < 0 && edges[currRow][currCol].HasWall()) {
            currCol--;
        }

        // Left Bound
        while (currCol < 0 || vert < 0 && edges[currRow][currCol].HasWall()) {
            currCol++;
        }

        // Up jump
        while (currCol < edges[currRow].Length && vert > 0 && edges[currRow][currCol].HasWall()) {
            currCol++;
        }

        // up Bound
        while (currCol >= edges[currRow].Length || vert > 0 && edges[currRow][currCol].HasWall()) {
            currCol--;
        }
    }


    // Find the next valid edge
    void FindNextEdge() {
        /*bool found = false;
        for (int row = 0; row < edges.Length; row++) {
            for (int col = 0; col < edges[row].Length; col++) {
                if (!(edges[row][col].HasWall())) {
                    currEdge = edges[row][col];
                    currRow = row;
                    currCol = col;
                    found = true;
                    break;
                }
            }

            if (found) {
                break;
            }
        }*/

        if (playerCol - 1 >= 0 && !(edges[2 * playerRow][playerCol - 1].HasWall())) {
            currEdge = edges[2 * playerRow][playerCol - 1];
            currRow = 2 * playerRow;
            currCol = playerCol - 1;
        } else if (2 * playerRow - 1 >= 0 && !(edges[2 * playerRow - 1][playerCol].HasWall())) {
            currEdge = edges[2 * playerRow - 1][playerCol];
            currRow = 2 * playerRow - 1;
            currCol = playerCol;
        } else if (2 * playerRow + 1 < edges.Length && !(edges[2 * playerRow + 1][playerCol].HasWall())) {
            currEdge = edges[2 * playerRow + 1][playerCol];
            currRow = 2 * playerRow + 1;
            currCol = playerCol;
        } else if (playerCol < edges[2 * playerRow].Length && !(edges[2 * playerRow][playerCol].HasWall())) {
            currEdge = edges[2 * playerRow][playerCol];
            currRow = 2 * playerRow;
            currCol = playerCol;
        }
    }

}
