using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerData : MonoBehaviour
{
    public static PlayerData instance;

    private int totalStars = 0;
    private int totalGems = 0;

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

    public void RegisterUI(TextMeshProUGUI starsText, TextMeshProUGUI gemsText)
    {
        if (starsText != null) starsText.text = "" + totalStars;
        if (gemsText != null) gemsText.text = "" + totalGems;
    }

    public void UpdateAllUI()
    {
        foreach (PlayerUI ui in FindObjectsOfType<PlayerUI>())
        {
            ui.UpdateUI(totalStars, totalGems);
        }
    }

}
