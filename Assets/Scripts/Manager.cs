using UnityEngine;

public class Manager : MonoBehaviour
{
    [Header("Settings")]

    [Tooltip("Blocks color that will spawn are selected Randomly from this array")]
    public  Color [] _colors;

    [Header("Elements")]
    [SerializeField] private GameObject blocksControllerPrefab;
    public bool isGamePlaying; // TODO : [HideInInspector]
    private int score;

    [Header("Managers")]
    [HideInInspector] public UI_Manager _ui_Manager;
    [HideInInspector] public Audio_Manager _audio_Manager;
    [HideInInspector] public BlocksController _blocksController;

    private void Awake()
    {
        _ui_Manager = FindAnyObjectByType<UI_Manager>();
        _audio_Manager = FindAnyObjectByType<Audio_Manager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // TODO : Loading Screen

        StartNewGame();
    }

    public void StartNewGame()
    {
        score = 0;

        // Reset UI elements
        _ui_Manager.ResetUIElement();

        if (_blocksController != null)   // is BlockController Instansitated before
        {
            Destroy(_blocksController.gameObject);  // Destroy Blocks 
            Debug.Log("blocksController destroyed..");
        }

        // Instansitate New BlockController
        GameObject BlocksController = Instantiate(blocksControllerPrefab);
        BlocksController.name = "BlocksController";

        _blocksController = BlocksController.GetComponent<BlocksController>();
    }

    public void RefreshScore(int points)
    {
        score += points;
        _ui_Manager.scoreText.text = "Score = " + score.ToString();
    }

}