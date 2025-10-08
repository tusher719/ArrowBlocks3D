using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI Panels")]
    [SerializeField] GameObject StartPanel;
    [SerializeField] GameObject MenuPanel;
    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject losePanel;

    [Header("UI Texts")]
    [SerializeField] TextMeshProUGUI blockText;
    [SerializeField] TextMeshProUGUI moveText;
    [SerializeField] TextMeshProUGUI keyText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI[] nextLevelTexts;

    [Header("Game Settings")]
    [SerializeField] LevelData[] levels;
    [SerializeField] float resultDelay = 1f;

    private int remainingMoves;
    private int totalBlocks;
    private int clearedBlocks = 0;
    private int collectedKeys = 0;

    private GameObject currentLevel;
    private int currentLevelIndex = 0;

    [HideInInspector] public bool allowBlockInput = true;
    private bool gameEnded = false;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Show only Start Panel at launch
        if (StartPanel) StartPanel.SetActive(true);
        if (MenuPanel) MenuPanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);
        if (losePanel) losePanel.SetActive(false);
    }

    // --------------------------------------------------
    // START GAME
    // --------------------------------------------------
    public void StartGame()
    {
        // Hide start panel & load level
        if (StartPanel) StartPanel.SetActive(false);
        LoadLevel(0);
    }

    // --------------------------------------------------
    // LEVEL MANAGEMENT
    // --------------------------------------------------
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
            gameEnded = false;

            totalBlocks = FindObjectsOfType<Block>().Length;
            UpdateUI();
            UpdateKeyUI();
        }
    }

    // --------------------------------------------------
    // GAMEPLAY UPDATES
    // --------------------------------------------------
    public void AddKey()
    {
        collectedKeys++;

        if (PlayerData.instance != null)
            PlayerData.instance.AddKeys(1);

        UpdateKeyUI();
    }

    private void UpdateKeyUI()
    {
        if (keyText != null)
        {
            if (PlayerData.instance != null)
                keyText.text = "" + PlayerData.instance.GetTotalKeys();
            else
                keyText.text = "0";
        }
    }

    public void UseMove()
    {
        if (remainingMoves > 0)
        {
            remainingMoves--;
            UpdateMoveUI();

            if (remainingMoves <= 0)
            {
                allowBlockInput = false;
                Block.DisableAllIdleBlocks();
                CheckLoseCondition();
            }
        }
    }

    public void BlockCleared()
    {
        if (gameEnded) return;

        clearedBlocks++;
        if (blockText != null)
            blockText.text = "" + (totalBlocks - clearedBlocks);

        if (clearedBlocks >= totalBlocks)
        {
            gameEnded = true;
            StartCoroutine(ShowWinWithDelay());
            return;
        }

        CheckLoseCondition();
    }

    private void CheckLoseCondition()
    {
        if (gameEnded) return;

        if (remainingMoves <= 0 && clearedBlocks < totalBlocks)
            StartCoroutine(CheckLoseAfterDelay(0.5f));
    }

    private IEnumerator CheckLoseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!gameEnded && remainingMoves <= 0 && clearedBlocks < totalBlocks)
        {
            gameEnded = true;
            StartCoroutine(ShowLoseWithDelay());
        }
    }

    // --------------------------------------------------
    // UI UPDATES
    // --------------------------------------------------
    private void UpdateUI()
    {
        UpdateMoveUI();
        if (blockText) blockText.text = "" + totalBlocks;
        UpdateKeyUI();
        if (levelText) levelText.text = "Level: " + (currentLevelIndex + 1);
    }

    private void UpdateMoveUI()
    {
        if (moveText != null)
            moveText.text = remainingMoves + " ";
    }

    // --------------------------------------------------
    // RESULT PANELS
    // --------------------------------------------------
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

    // --------------------------------------------------
    // MENU & REWARD
    // --------------------------------------------------
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

    // --------------------------------------------------
    // LEVEL CONTROL
    // --------------------------------------------------
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
