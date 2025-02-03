using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Blocks : MonoBehaviour
{
    public Vector3 rotationPoint;
    private float previousTime;
    public float fallTime = 0.8f;
    public static int height = 20;
    public static int width = 10;
    private static Transform[,] grid = new Transform[width, height];

    private static float lastSpawnTime = 0f; // Track last spawn time
    private float cooldown = .20f; // cooldown, Prevents too fast of respawn (Reaching the top)

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) // Move Left
        {
            transform.position += new Vector3(-1, 0, 0);
            if (!ValidMove()) transform.position -= new Vector3(-1, 0, 0);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) // Move Right
        {
            transform.position += new Vector3(1, 0, 0);
            if (!ValidMove()) transform.position -= new Vector3(1, 0, 0);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow)) // Rotate
        {
            transform.RotateAround(transform.TransformPoint(rotationPoint), Vector3.forward, 90);
            if (!ValidMove()) transform.RotateAround(transform.TransformPoint(rotationPoint), Vector3.forward, -90);
        }

        // Falling logic
        if (Time.time - previousTime > (Input.GetKey(KeyCode.DownArrow) ? fallTime / 10 : fallTime))
        {
            transform.position += new Vector3(0, -1, 0);
            if (!ValidMove())
            {
                transform.position -= new Vector3(0, -1, 0);
                AddToGrid();
                CheckForLines();
                this.enabled = false;

                // Check cooldown before spawning the next piece
                float currentTime = Time.time;
                if (currentTime - lastSpawnTime < cooldown)
                {
                    Debug.Log("Game Over! Spawned too fast.");
                    SceneManager.LoadScene(1); // Reset the screen
                }
                else
                {
                    lastSpawnTime = currentTime; // Update spawn time
                    FindAnyObjectByType<BlockSpawner>()?.NewTetromino();
                }
            }
            previousTime = Time.time;
        }
    }

    void CheckForLines()
    {
        for (int i = height - 1; i >= 0; i--)
        {
            if (HasLine(i)) // Removed incorrect semicolon
            {
                DeleteLine(i);
                RowDown(i);
            }
        }
    }

    bool HasLine(int i) // Check if a full line exists
    {
        for (int j = 0; j < width; j++)
        {
            if (grid[j, i] == null) return false;
        }
        return true;
    }

    void DeleteLine(int i) // Remove full line
    {
        bool hasETetromino = false; // Track if "E-Tetromino" exists in the row

        for (int j = 0; j < width; j++)
        {
            if (grid[j, i] != null && grid[j, i].parent.name.Contains("E-Tetromino"))
            {
                hasETetromino = true; // Found an "E-Tetromino"
            }
        }

        // Assign score based on presence of "E-Tetromino"
        BlockSpawner.score += hasETetromino ? 500 : 100; // If true add 500, if not add 100

        for (int j = 0; j < width; j++)
        {
            Destroy(grid[j, i].gameObject);
            grid[j, i] = null;
        }

    }

        void RowDown(int i) // Move all rows down
        {
        for (int y = i; y < height - 1; y++) 
        {
            for (int j = 0; j < width; j++)
            {
                if (grid[j, y + 1] != null)
                {
                    grid[j, y] = grid[j, y + 1];
                    grid[j, y + 1] = null;
                    grid[j, y].transform.position -= new Vector3(0, 1, 0); // Move block down
                }
            }
        }
        }

    void AddToGrid()
    {
        foreach (Transform children in transform)
        {
            int roundedX = Mathf.RoundToInt(children.position.x);
            int roundedY = Mathf.RoundToInt(children.position.y);

            grid[roundedX, roundedY] = children;
        }
    }

    bool ValidMove() // Check if the movement is valid
    {
        foreach (Transform children in transform)
        {
            int roundedX = Mathf.RoundToInt(children.position.x);
            int roundedY = Mathf.RoundToInt(children.position.y);

            if (roundedX < 0 || roundedX >= width || roundedY < 0 || roundedY >= height)
            {
                return false;
            }
            if (grid[roundedX, roundedY] != null)
            {
                return false;
            }
        }
        return true;
    }
}
