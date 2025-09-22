using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI blockText;
    [SerializeField] TextMeshProUGUI moveText;
    [SerializeField] TextMeshProUGUI keyText;
    [SerializeField] TextMeshProUGUI levelText;

    [Header("Result Panels")]
    [SerializeField] GameObject MenuPanel;
    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject losePanel;

    [Header("Delays")]
    [SerializeField] float resultDelay = 1f;

    [Header("Levels")]
    [SerializeField] LevelData[] levels;
    [Header("Menu Panel UI")]
    // [SerializeField] TextMeshProUGUI nextLevelText;
    // [SerializeField] TextMeshProUGUI nextLevelsText;
    [SerializeField] TextMeshProUGUI[] nextLevelTexts;

    private int remainingMoves;
    private int totalBlocks;
    private int clearedBlocks = 0;
    private int collectedKeys = 0;

    private GameObject currentLevel;
    private int currentLevelIndex = 0;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        LoadLevel(0);

        if (MenuPanel != null) MenuPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
    }

    public void LoadLevel(int index)
    {
        if (currentLevel != null)
            Destroy(currentLevel);

        if (index >= 0 && index < levels.Length)
        {
            currentLevel = Instantiate(levels[index].levelPrefab, Vector3.zero, Quaternion.identity);
            currentLevelIndex = index;

            remainingMoves = levels[index].totalMoves;

            clearedBlocks = 0;
            collectedKeys = 0;

            totalBlocks = FindObjectsOfType<Block>().Length;

            UpdateUI();
        }
    }

    public void AddKey()
    {
        collectedKeys++;
        if (keyText != null)
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
            blockText.text = "Blocks: " + (totalBlocks - clearedBlocks);

        if (clearedBlocks >= totalBlocks)
            StartCoroutine(ShowWinWithDelay());
    }

    private void UpdateUI()
    {
        UpdateMoveUI();

        if (blockText != null)
            blockText.text = "Blocks: " + totalBlocks;

        if (keyText != null)
            keyText.text = "Keys: " + collectedKeys;

        if (levelText != null)
            levelText.text = "Level: " + (currentLevelIndex + 1);
    }

    private void UpdateMoveUI()
    {
        if (moveText != null)
            moveText.text = remainingMoves + " Moves";
    }

    // ------------------------
    //   WIN / LOSE HANDLING
    // ------------------------

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

    public void CollectRewardAndShowMenu()
    {
        if (PlayerData.instance != null)
        {
            PlayerData.instance.AddRewards(1, 5);
        }

        // int nextIndex = currentLevelIndex + 1;
        // if (nextLevelText != null)
        // {
        //     if (nextIndex < levels.Length)
        //         nextLevelText.text = "Next Level: " + (nextIndex + 1);
        //     else
        //         nextLevelText.text = "All Levels Completed 🎉";
        // }


        // if (nextLevelsText != null)
        // {
        //     string nextLevels = "";
        //     for (int i = 1; i <= 3; i++) // next 3 levels দেখাবে (2,3,4)
        //     {
        //         int nextIndex = currentLevelIndex + i;
        //         if (nextIndex < levels.Length)
        //             nextLevels += "Level " + (nextIndex + 1) + "\n";
        //     }

        //     if (string.IsNullOrEmpty(nextLevels))
        //         nextLevelsText.text = "All Levels Completed 🎉";
        //     else
        //         nextLevelsText.text = nextLevels;
        // }

        UpdateNextLevelUI();

        StartCoroutine(ShowMenuAfterDelay(1f));
    }

    private void UpdateNextLevelUI()
    {
        for (int i = 0; i < nextLevelTexts.Length; i++)
        {
            int nextIndex = currentLevelIndex + (i + 1);

            if (nextIndex < levels.Length)
            {
                nextLevelTexts[i].text = "" + (nextIndex + 1);
                nextLevelTexts[i].gameObject.SetActive(true);
            }
            else
            {
                nextLevelTexts[i].text = "";
                nextLevelTexts[i].gameObject.SetActive(false);
            }
        }
    }


    private IEnumerator ShowMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (MenuPanel != null) MenuPanel.SetActive(true);
        if (winPanel != null) winPanel.SetActive(false);
    }


    // ------------------------
    //   LEVEL CONTROL
    // ------------------------
    public void PlayAgain()
    {
        LoadLevel(currentLevelIndex);
        if (MenuPanel != null) MenuPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
    }

    public void NextLevel()
    {
        int nextIndex = currentLevelIndex + 1;
        if (nextIndex < levels.Length)
        {
            LoadLevel(nextIndex);
            if (winPanel != null) winPanel.SetActive(false);
            if (MenuPanel != null) MenuPanel.SetActive(false);
        }
    }
}
