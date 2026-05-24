// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-08-15
// Description: Manages the game over state, displaying appropriate panels based on the reason for game over (freeze, starve, or win).

using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [Header("Panel References")]
    public GameObject gameOverPanelFreeze;
    public GameObject gameOverPanelStarve;
    public GameObject gameOverPanelWin;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }


        ShowGameOver(GameManager.Instance.gameOverReason);
    }

    public void ShowGameOver(string reason)
    {

        switch (reason)
        {
            case "freeze":
                gameOverPanelFreeze.SetActive(true);
                break;

            case "starve":
                gameOverPanelStarve.SetActive(true);
                break;

            case "win":
                gameOverPanelWin.SetActive(true);
                break;
        }

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnMainMenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }
}
