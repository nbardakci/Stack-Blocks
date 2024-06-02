using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlocksController : MonoBehaviour
{
    [Header("Settings")]

    [Tooltip("Waiting time for the block to move to the next cell")]
    [SerializeField] private float movementWaitTime;  // 0.4f // Starting at 0.4f // more the slower. 

    [Tooltip("Minimum waiting time for the block to move to the next cell")]
    [SerializeField] private float movementWaitTimeMin; // 0.12f //will decrease to 0.12f 

    [Tooltip("Waiting time for the next block to destroy while the screen is cleared")]
    [SerializeField] private float blocksDestroyWaitTime; // 0.05f //more the slower

    [Header("Elements")]
    [SerializeField] private Block blockPrefab;
    [HideInInspector] public int movingBlockCount;  // It is changing from Block.cs . When moving a block, moveBlockCount increases by 1. When the block movement is completed, it is decreased by 1.

    [Tooltip("After how many rows will the blocks be shifted down? [5-10]")]
    [SerializeField] private int firstTargetNumber = 8;

    [Tooltip("When the blocks reach the top, how many lines should be destroyed from below? ")]
    [SerializeField] private int numberOfRowsToDestroy = 1; // it should be lower than firstTargetNumber-1

    public List<Vector2Int> positions = new List<Vector2Int>(); // blocks spawn coordinates
    private List<Block> blocksList = new List<Block>();          // List of All blocks on the screen
    private List<Block> activeBlocksList = new List<Block>();    // List of blocks in the active row.

    [HideInInspector] public bool isScreenTapped;

    public int rowNumber, targetRowNumber; // TODO : [HideInInspector]

    [Header("Managers")]
    private Manager _manager;
    private Board _board;

    private void Awake()
    {
        _manager = FindAnyObjectByType<Manager>();
        _board = FindAnyObjectByType<Board>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("----- BlocksController created .. New Game is Starting -----");

        blocksList.Clear(); // Clear the List of All blocks on the screen

        //  Prepare the first Blocks
        // 4 Blocks will spawn on the board => Targets (2,0),(3,0),(4,0),(5,0)
        positions.Clear();
        positions.Add(new Vector2Int(2, 0));
        positions.Add(new Vector2Int(3, 0));
        positions.Add(new Vector2Int(4, 0));
        positions.Add(new Vector2Int(5, 0));

        movementWaitTime = 0.4f;
        rowNumber = 1;
        targetRowNumber = firstTargetNumber;
        
       _manager._ui_Manager.startMessage.gameObject.SetActive(true); // display startMessage

        CreateBlocks();
    }

    private void CreateBlocks()
    {
        Debug.Log("----- The blocks are creating -----");

        // Select a random Color from the array
        int randomColor = Random.Range(0, _manager._colors.Count());
        Color blockColor = _manager._colors[randomColor];

        activeBlocksList.Clear(); // Clear List of blocks in the active row.
        for (int i = 0; i < positions.Count; i++)
        {
            Vector3 spawnPosition = _board.boardTiles[positions[i].x, positions[i].y].position;

            // bloğu oluştur.
            Block _block = Instantiate(blockPrefab, spawnPosition, Quaternion.identity, transform);
            _block.name = "Block - " + positions[i].x + "," + positions[i].y; // Set Block Name
            _block.GetComponent<SpriteRenderer>().color = blockColor;  // Set Block Color
            _block.position = positions[i];
            _block.targetPosition = positions[i];

            blocksList.Add(_block); // Add this block to the All blocks List.
            activeBlocksList.Add(_block); // Also, Add this block to the activeBlocksList 
        }

        _manager.isGamePlaying = true;  // Game is playing

        StartMoving();
    }

    private void StartMoving()
    {
        Debug.Log("----- The blocks are moving -----");

        isScreenTapped = false;

        // Randomly select movement direction
        int movementDirection = UnityEngine.Random.Range(0, 2);

        if (movementDirection == 0) StartCoroutine(MoveToLeftCo());
        else StartCoroutine(MoveToRightCo());
    }

    private IEnumerator MoveToLeftCo()
    {
        while (activeBlocksList.Count > 0
                && activeBlocksList[0].position.x > 0 // if the first block in the list is not the leftmost 
                && !isScreenTapped
                && _manager.isGamePlaying)
        {
            if (isScreenTapped) break;

            for (int i = 0; i < activeBlocksList.Count; i++)
            {
                activeBlocksList[i].position.x--;
                Transform targetTileTr = _board.boardTiles[activeBlocksList[i].position.x, activeBlocksList[i].position.y];
                activeBlocksList[i].transform.position = targetTileTr.position;
            }

            if (isScreenTapped) break;
            yield return new WaitForSeconds(movementWaitTime);
        }
        if (isScreenTapped) CheckBlocksLocations();
        else if (_manager.isGamePlaying) StartCoroutine(MoveToRightCo());
    }

    private IEnumerator MoveToRightCo()
    {
        while (activeBlocksList.Count > 0
                && activeBlocksList[activeBlocksList.Count - 1].position.x < _board.boardSize.x - 1 // if the last block in the list is not the rightmost 
                && !isScreenTapped
                && _manager.isGamePlaying)
        {
            if (isScreenTapped) break;

            for (int i = 0; i < activeBlocksList.Count; i++)
            {
                activeBlocksList[i].position.x++;
                Transform targetTileTr = _board.boardTiles[activeBlocksList[i].position.x, activeBlocksList[i].position.y];
                activeBlocksList[i].transform.position = targetTileTr.transform.position;
            }

            if (isScreenTapped) break;
            yield return new WaitForSeconds(movementWaitTime);
        }
        if (isScreenTapped) CheckBlocksLocations();
        else if (_manager.isGamePlaying) StartCoroutine(MoveToLeftCo());
    }


    private void CheckBlocksLocations()
    {
        Debug.Log("----- The blocks locations are checking -----");

        positions.Clear(); // clear the list for new blocks to spawn above the blocks

        List<Block> _missedBlockList = new List<Block>(); // List of Blocks not on target 

        for (int i = 0; i < activeBlocksList.Count; ++i) //  Are the blocks on target? Check it for each block in activeBlocksList. 
        {
            bool _isBlockOnTarget = false;

            for (int l = 0; l < activeBlocksList.Count; l++)
            {
                if (rowNumber == 1 // It doesn't matter where it is stopped in the first line. Wherever the blocks stand, they will stack up.
                    || activeBlocksList[i].position == activeBlocksList[l].targetPosition)  // If the block is on the target that is any block's target in the list
                {
                    // Add the position of the cell above the block to the positions list. The list of new blocks's positions to be created 
                    Vector2Int position = new Vector2Int(activeBlocksList[i].position.x, activeBlocksList[i].position.y + 1);
                    positions.Add(position);

                    _isBlockOnTarget = true;
                    break;
                }
            }

            if (!_isBlockOnTarget)
                _missedBlockList.Add(activeBlocksList[i]); // Add the block that is not on target to the Missed Blocks list
        }

        // Point Missed Blocks on the screen
        if (_missedBlockList.Count > 0)  
        {
            Debug.Log("_missedBlockList.Count = " + _missedBlockList.Count);

            for (int i = 0; i < _missedBlockList.Count; i++)
            {
                _missedBlockList[i].ImageX(true); // Make the Cross image on the block visible

                // Fade the color of the block.
                Color _tmpColor = _missedBlockList[i].GetComponent<SpriteRenderer>().color;
                _tmpColor.a = .5f;   // .05f  =  Alpha = 125
                _missedBlockList[i].GetComponent<SpriteRenderer>().color = _tmpColor;
            }
            _manager._audio_Manager.PlayAudio("MissedBlock");
        }

        // is There a block placed on the targets
        if (positions.Count > 0) 
        {
            // Calculate the points got 
            int points = 10 * positions.Count * rowNumber;

            // Refresh total score
            _manager.RefreshScore(points);

            if (positions.Count == activeBlocksList.Count) // if all active Blocks on the target 
                _manager._audio_Manager.PlayAudio("AllBlockOnTarget");

            CreateBlocksOnTheUpperRow();// Create new blocks
        }
        else  // Any block is not on the target. GAME OVER
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        Debug.Log("----- Game Over -----");

        _manager.isGamePlaying = false;

        _manager._ui_Manager.gameOverPanel.SetActive(true);
        _manager._audio_Manager.PlayAudio("GameOver");
    }

    private void CreateBlocksOnTheUpperRow()
    {
        Debug.Log("----- The blocks are creating on the upper row -----");

        rowNumber++;
        _manager._ui_Manager.rowText.text = "Row = " + rowNumber.ToString(); // Refresh the Row Text on the UI

        // reduce the waiting time. thus increasing the speed of the block
        if (movementWaitTime > movementWaitTimeMin)
            movementWaitTime -= 0.01f;

        if (rowNumber > targetRowNumber) // If the target is passed, The blocks will be cleared on the screen 
        {
            targetRowNumber += numberOfRowsToDestroy; // Set new target

            _manager._ui_Manager.startMessage.gameObject.SetActive(false); // Display message text only up to the first target row

            StartCoroutine(DestroyBlocksCo()); // Clear Blocks
        }
        else // Create new blocks
        {
            CreateBlocks(); 
        }
    }


    private IEnumerator DestroyBlocksCo()
    {
        // The blocks on the top row will be dropped to the bottom row, the other blocks will be destroyed.

        #region Blocks are Destroying 

        Debug.Log("----- The blocks are destroying -----");

        List<Block> _destroyBlocksList = new List<Block>();
        bool isBlockListCheckingAgain = true;
        do
        {
            isBlockListCheckingAgain = false;

            for (int i = 0; i < blocksList.Count; i++)
            {
                // Add blocks to the _destroyBlocksList in order from the bottom.
                if (blocksList[i].position.y < numberOfRowsToDestroy)   
                {
                    Block _block = blocksList[i];
                    blocksList.Remove(blocksList[i]);  // Remove the block from the blocksList

                    _destroyBlocksList.Add(_block); // Add the block to the _destroyBlocksList

                    // A block removed the blocksList and the list's order changed. So Restart the progress.
                    isBlockListCheckingAgain = true;
                    break;
                }
            }
        } while (isBlockListCheckingAgain);

        // Destroy the blocks in the _destroyBlocksList.
        for (int i = 0; i < _destroyBlocksList.Count; i++)
        {
            _destroyBlocksList[i].DestroyBlock();

            yield return new WaitForSeconds(blocksDestroyWaitTime);
        }

        Debug.Log("----- The blocks destroyed -----");

        #endregion

        #region Blocks is moving top to down

        Debug.Log("----- Blocks is moving top to down -----");

        for (int i = 0; i < blocksList.Count; i++)
        {
            //Set the position values ​​of the block to be the first row
            blocksList[i].position.y -= numberOfRowsToDestroy;
            blocksList[i].targetPosition = blocksList[i].position;
            blocksList[i].name = "Block - " + blocksList[i].position.x + "," + blocksList[i].position.y;

            // calculate the new position where the block will move down
            Vector3 _newPosition = _board.boardTiles[blocksList[i].position.x, blocksList[i].position.y].transform.position;
            blocksList[i].MoveBlokToDown(_newPosition); // Start the block's movement down.
        }

        // for new blocks, refresh the values in the positions list.
        for (int i = 0; i < positions.Count; i++)
        {
            int x = positions[i].x;
            int y = positions[i].y - numberOfRowsToDestroy;

            positions[i] = new Vector2Int(x, y);
        }

        // Wait for block movements to complete
        do
        {
            yield return new WaitForSeconds(0.5f); // wait a bit

        } while (movingBlockCount > 0);

        Debug.Log("----- Blocks moved top to down -----");

        CreateBlocks(); // Create new blocks and contiune the game

        #endregion
    }


}