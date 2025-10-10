using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerData : MonoBehaviour
{
    public static PlayerData instance;

    private int totalStars = 0;
    private int totalGems = 0;
    private int totalKeys = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddRewards(int stars, int gems)
    {
        totalStars += stars;
        totalGems += gems;
        UpdateAllUI();
    }

    public void AddKeys(int keys)
    {
        totalKeys += keys;
        UpdateAllUI();
    }

    public void RegisterUI(TextMeshProUGUI starsText, TextMeshProUGUI gemsText, TextMeshProUGUI keysText)
    {
        if (starsText != null) starsText.text = "" + totalStars;
        if (gemsText != null) gemsText.text = "" + totalGems;
        if (keysText != null) keysText.text = "" + totalKeys;
    }

    public void UpdateAllUI()
    {
        foreach (PlayerUI ui in FindObjectsOfType<PlayerUI>())
        {
            ui.UpdateUI(totalStars, totalGems, totalKeys);
        }
    }

    // Getter methods
    public int GetTotalKeys() { return totalKeys; }
    public int GetTotalStars() { return totalStars; }
    public int GetTotalGems() { return totalGems; }
    
    // Setter method for rollback (when level fails)
    public void SetTotalKeys(int keys)
    {
        totalKeys = keys;
        UpdateAllUI();
    }
}
