using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

    [SerializeField]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    //number of columns on the gameboard
    public int columns;
    //number of rows on the gameboard
    public int rows;
    public Count wallCount = new Count(10, 20);
    public Count foodCount = new Count(2, 5);
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;

    //Every object in a scene has a Transform. It's used to store and manipulate the position, rotation and scale of the object. 
    //Every Transform can have a parent, which allows you to apply position, rotation and scale hierarchically
    private Transform boardHolder;

    //A list of Vector gridpositions
    private List<Vector3> gridPositions = new List<Vector3>();

    /// <summary>
    /// Clear out and create a list of Vector3 grid positions for each 'grid item' in the game
    /// </summary>
    void InitializeList()
    {
        gridPositions.Clear();

        for (int x = -4; x < columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    void BoardSetup()
    {
        //creates an object to act as a 'transform' parent object
        boardHolder = new GameObject("Board").transform;

        //iterates through all the positions on the GameBoard
        // x and y represent the actual position on the Gameboard
        for (int x = -5; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                //creates a floor tile
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];

                //if we're on the edge of the board create a wall instead of a floor itle
                if(x == -5 || x == columns || y == -1 || y == rows)
                {
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                }

                //instantiates the object to be placed on the game
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                //sets a parent the boardholder as the parent object
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    /// <summary>
    /// Gets a random Vector3 position, then removes that from the list of gridpositions
    /// </summary>
    /// <returns></returns>
    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);

        Vector3 randomPosition = gridPositions[randomIndex];

        gridPositions.RemoveAt(randomIndex);

        return randomPosition;
    }

    /// <summary>
    /// Lays objects around the gameboard at random
    /// </summary>
    /// <param name="tileArray"></param>
    /// <param name="minimum"></param>
    /// <param name="maximum"></param>
    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);

        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    public void SetupScene(int level)
    {
        BoardSetup();
        InitializeList();

        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);

        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
    }
}
