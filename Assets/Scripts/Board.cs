using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [Header("Settings")]
    public Vector2Int boardSize; // width & height (8,10)
    [SerializeField] private Vector2 offset;  // to set the tile position
    [SerializeField] private Vector2 _const;  // to set the tile position

    [Header("Elements")]
    [SerializeField] private GameObject boardTilePrefab;
    [HideInInspector] public Transform[,] boardTiles;  // for locating the blocks


    // Start is called before the first frame update
    void Start()
    {
        boardTiles = new Transform[boardSize.x, boardSize.y];
        CreateBoardTiles();
    }

    private void CreateBoardTiles()
    {
        for (int y = 0; y < boardSize.y; y++) // Bottom to Top
        {
            for (int x = 0; x < boardSize.x; x++) // Left to Right
            {
                Vector2 position = new Vector2(x, y);

                // Create boardTile
                GameObject boardTile = Instantiate(boardTilePrefab, position, Quaternion.identity,transform);
                boardTile.name = "Board Tile - " + x + "," + y;

                //Set position using the Offsets 
                position = new Vector2(x * offset.x + _const.x, y * offset.y + _const.y);
                boardTile.transform.position = position;

                boardTiles[x, y] = boardTile.transform; // put boardTile.transform in the array
            }
        }
    }
}