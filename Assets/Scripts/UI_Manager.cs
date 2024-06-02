using TMPro;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    [Header("Settings")]
    public TextMeshProUGUI startMessage;
    public TextMeshProUGUI scoreText, rowText;
    public GameObject gameOverPanel;

    [Header("Managers")]
    private Manager _manager;

    private void Awake()
    {
        _manager = FindAnyObjectByType<Manager>();
    }

    public void ScreenTapped()
    {
        if (_manager.isGamePlaying 
            && _manager._blocksController != null)
               _manager._blocksController.isScreenTapped = true;
    }
    public void NewGameBtn()
    {
        gameOverPanel.SetActive(false);
        _manager.StartNewGame();
    }

    public void ResetUIElement()
    {
        scoreText.text = "Score = 0";
        rowText.text = "Row = 1";
    }
    public void NB_Btn()
    {
        Application.OpenURL("https://play.google.com/store/apps/dev?id=6171470959010710848");
    }
}
