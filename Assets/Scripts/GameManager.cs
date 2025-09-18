using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game Settings")]
    [SerializeField] int totalMoves = 10;
    [SerializeField] TextMeshProUGUI blockText;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI moveText;
    [SerializeField] TextMeshProUGUI keyText;

    [Header("Result Panels")]
    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject losePanel;

    [Header("Delays")]
    [SerializeField] float resultDelay = 1f;

    private int remainingMoves;
    private int totalBlocks;
    private int clearedBlocks = 0;
    private int collectedKeys = 0;


    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        remainingMoves = totalMoves;
        totalBlocks = FindObjectsOfType<Block>().Length;

        UpdateMoveUI();
        if (blockText != null)
        {
            blockText.text = "Blocks: " + totalBlocks;
        }

        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
    }

    
    public void AddKey()
    {
        collectedKeys++;
        keyText.text = "Keys: " + collectedKeys;
    }

    public void UseMove()
    {
        if (remainingMoves > 0)
        {
            remainingMoves--;
            UpdateMoveUI();

            if (remainingMoves <= 0 && clearedBlocks < totalBlocks)
            {
                StartCoroutine(ShowLoseWithDelay());
            }
        }
    }

    public void BlockCleared()
    {
        clearedBlocks++;
        if (blockText != null)
        {
            blockText.text = "Blocks: " + (totalBlocks - clearedBlocks);
        }
        WinScene();
    }

    private void UpdateMoveUI()
    {
        if (moveText != null)
            moveText.text = "Moves: " + remainingMoves;
    }

    // Win
    void WinScene()
    {
        if (clearedBlocks >= totalBlocks)
        {
            StartCoroutine(ShowWinWithDelay());
        }
    }

    // Lost
    private void LoseScene()
    {
        if (losePanel != null) losePanel.SetActive(true);
    }

    private IEnumerator ShowWinWithDelay()
    {
        yield return new WaitForSeconds(resultDelay);
        if (winPanel != null) winPanel.SetActive(true);
    }

    private IEnumerator ShowLoseWithDelay()
    {
        yield return new WaitForSeconds(resultDelay);
        if (losePanel != null) losePanel.SetActive(true);
    }

    // Restart
    public void PlayAgain()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
}
