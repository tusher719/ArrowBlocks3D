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
    [SerializeField] TextMeshProUGUI[] nextLevelTexts;

    private int remainingMoves;
    private int totalBlocks;
    private int clearedBlocks = 0;
    private int collectedKeys = 0;

    private GameObject currentLevel;
    private int currentLevelIndex = 0;

    [HideInInspector] public bool allowBlockInput = true;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        LoadLevel(0);
        if (MenuPanel) MenuPanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);
        if (losePanel) losePanel.SetActive(false);
    }

    // ------------------------
    //   LEVEL LOAD
    // ------------------------
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
            allowBlockInput = true;

            totalBlocks = FindObjectsOfType<Block>().Length;
            UpdateUI();
        }
    }

    // ------------------------
    //   GAMEPLAY UPDATES
    // ------------------------
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

            // If last move used — disable all idle blocks
            if (remainingMoves <= 0)
            {
                allowBlockInput = false;
                Block.DisableAllIdleBlocks();
            }
        }
    }

    public void BlockCleared()
    {
        clearedBlocks++;
        if (blockText != null)
            blockText.text = "Blocks: " + (totalBlocks - clearedBlocks);

        // All blocks cleared → Win
        if (clearedBlocks >= totalBlocks)
        {
            StartCoroutine(ShowWinWithDelay());
            return;
        }

        // Moves ended but blocks remain → Lose
        if (remainingMoves <= 0 && clearedBlocks < totalBlocks)
        {
            StartCoroutine(ShowLoseWithDelay());
        }
    }

    private void UpdateUI()
    {
        UpdateMoveUI();
        if (blockText) blockText.text = "Blocks: " + totalBlocks;
        if (keyText) keyText.text = "Keys: " + collectedKeys;
        if (levelText) levelText.text = "Level: " + (currentLevelIndex + 1);
    }

    private void UpdateMoveUI()
    {
        if (moveText != null)
            moveText.text = remainingMoves + " Moves";
    }

    // ------------------------
    //   FINAL RESULT CHECK
    // ------------------------
    private IEnumerator ShowWinWithDelay()
    {
        yield return new WaitForSeconds(resultDelay);
        if (winPanel) winPanel.SetActive(true);
    }

    private IEnumerator ShowLoseWithDelay()
    {
        yield return new WaitForSeconds(resultDelay);
        if (losePanel) losePanel.SetActive(true);
    }

    // ------------------------
    //   MENU / REWARD
    // ------------------------
    public void CollectRewardAndShowMenu()
    {
        if (PlayerData.instance != null)
            PlayerData.instance.AddRewards(1, 5);

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
        if (MenuPanel) MenuPanel.SetActive(true);
        if (winPanel) winPanel.SetActive(false);
    }

    // ------------------------
    //   LEVEL CONTROL
    // ------------------------
    public void PlayAgain()
    {
        StartCoroutine(ResetAndReloadLevel());
    }

    private IEnumerator ResetAndReloadLevel()
    {
        Block[] oldBlocks = FindObjectsOfType<Block>();
        foreach (Block b in oldBlocks)
            Destroy(b.gameObject);

        if (currentLevel != null)
        {
            Destroy(currentLevel);
            currentLevel = null;
        }

        yield return null;

        clearedBlocks = 0;
        collectedKeys = 0;
        totalBlocks = 0;

        LoadLevel(currentLevelIndex);

        if (MenuPanel) MenuPanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);
        if (losePanel) losePanel.SetActive(false);
    }

    public void NextLevel()
    {
        int nextIndex = currentLevelIndex + 1;
        if (nextIndex < levels.Length)
        {
            LoadLevel(nextIndex);
            if (winPanel) winPanel.SetActive(false);
            if (MenuPanel) MenuPanel.SetActive(false);
        }
    }
}
