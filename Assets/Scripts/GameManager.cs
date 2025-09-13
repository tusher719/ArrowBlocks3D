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
    [SerializeField] float sceneLoadDelay = 2f;
    private int remainingMoves;
    private int totalBlocks;
    private int clearedBlocks = 0;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI moveText;

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
    }

    public void UseMove()
    {
        if (remainingMoves > 0)
        {
            remainingMoves--;
            UpdateMoveUI();

            if (remainingMoves <= 0 && clearedBlocks < totalBlocks)
            {
                LoseScene();
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
            StartCoroutine(WaitAndLoad("win", sceneLoadDelay));
        }
    }

    // Lost
    private void LoseScene()
    {
        SceneManager.LoadSceneAsync(2);
    }

    // Restart
    public void playAgain()
    {
        SceneManager.LoadSceneAsync(0);
    }

    // Win Delay
    IEnumerator WaitAndLoad(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadSceneAsync(sceneName);
    }
}
