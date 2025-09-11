using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Score : MonoBehaviour
{
    float point = 0;
    public TextMeshProUGUI scoreText;

    private void Start()
    {
        UpdateScoreUI();
    }

    public void AddPoint(int amount)
    {
        point += amount;
        UpdateScoreUI();
        WinScene();
    }

    void WinScene()
    {
        if (point == 45)
        {
            SceneManager.LoadSceneAsync(1);
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + point;
    }

    public void playAgain()
    {
        SceneManager.LoadSceneAsync(0);
    }

}
