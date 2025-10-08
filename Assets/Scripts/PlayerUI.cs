using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI starsText;
    public TextMeshProUGUI gemsText;
    public TextMeshProUGUI keysText;

    private void OnEnable()
    {
        if (PlayerData.instance != null)
        {
            PlayerData.instance.RegisterUI(starsText, gemsText, keysText);
        }
    }

    public void UpdateUI(int stars, int gems, int keys)
    {
        if (starsText != null) starsText.text = "" + stars;
        if (gemsText != null) gemsText.text = "" + gems;
        if (keysText != null) keysText.text = "" + keys;
    }
}