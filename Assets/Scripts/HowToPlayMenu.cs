using UnityEngine;

public class HowToPlayMenu : MonoBehaviour
{
    private static bool GameIsPaused;
    [SerializeField] private GameObject HideHowToPlayMenuUI;

    public void HideHowToPlayMenu()
    {
        HideHowToPlayMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void ShowHowToPlayMenu()
    {
        HideHowToPlayMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
}