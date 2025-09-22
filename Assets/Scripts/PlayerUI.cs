using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI starsText;
    public TextMeshProUGUI gemsText;

    private void OnEnable()
    {
        if (PlayerData.instance != null)
        {
            PlayerData.instance.RegisterUI(starsText, gemsText);
        }
    }

    public void UpdateUI(int stars, int gems)
    {
        if (starsText != null) starsText.text = "" + stars;
        if (gemsText != null) gemsText.text = "" + gems;
    }
}
