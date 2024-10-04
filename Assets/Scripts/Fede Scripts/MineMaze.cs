using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MineMaze : MonoBehaviour
{
    [SerializeField] private GameObject minePrefab;
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private float cellSize = 4.0f;

    [SerializeField] private TextAsset mazeText;

    private int[,] defaultMaze = new int[,]
    {
        {1, 1, 1, 1, 1, 1, 1, 1, 1},
        {1, 0, 0, 0, 0, 0, 0, 0, 1},
        {1, 0, 0, 0, 0, 0, 0, 0, 1},
        {1, 0, 0, 0, 0, 0, 0, 0, 1},
        {1, 0, 0, 0, 0, 0, 0, 0, 1},
        {1, 0, 0, 0, 0, 0, 0, 0, 1},
        {1, 0, 0, 0, 0, 0, 0, 0, 1},
        {1, 0, 0, 0, 0, 0, 0, 0, 1},
        {1, 2, 0, 0, 0, 0, 0, 0, 1},
        {1, 1, 1, 1, 1, 1, 1, 1, 1},
    };

    private int[,] maze;

    void Start()
    {
        LoadMazeFromText();

        if (maze != null)
        {
            GenerateMaze();
        }
        else
        {
            Debug.Log("ERROR : Impossible to generate the maze from text\n -- Generating Default Maze -- ");
            maze = defaultMaze;
            GenerateMaze();
        }

    }

    private void GenerateMaze()
    {
        if (maze == null)
        {
            return;
        }

        int rows = maze.GetLength(0);
        int cols = maze.GetLength(1);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (maze[row, col] == 1)
                {
                    Vector3 position = new Vector3(col * cellSize, 0.0f, row * cellSize);

                    Instantiate(minePrefab, position, Quaternion.identity);
                }

                if (maze[row, col] == 2)
                {
                    Vector3 position = new Vector3(col * cellSize, 0.0f, row * cellSize);

                    Instantiate(portalPrefab, position, Quaternion.identity);
                }
            }
        }
    }

    private void LoadMazeFromText()
    {
        if (mazeText != null)
        {
            string[] lines = mazeText.text.Split('\n');

            int rows = lines.Length;
            int cols = lines[0].Split(' ').Length;

            maze = new int[rows, cols];

            for (int row = 0; row < rows; row++)
            {
                string[] values = lines[row].Trim().Split(' ');

                for (int col = 0; col < cols; col++)
                {
                    maze[row, col] = int.Parse(values[col]);
                }
            }
        }
        else
        {
            Debug.Log("ERROR : Text not found!");
        }
    }

    [ContextMenu("Generate Maze")]
    public void GenerateMazeInEditor()
    {
        LoadMazeFromText();
        GenerateMaze();
    }
}
