using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject winPanel;
    public GameObject losePanel;

    [Header("Text")]
    public TMP_Text winMoneyText;   
    public TMP_Text loseMoneyText;  

    bool _ended = false;

    void Start()
    {
        HideAll();

    }

    public void ShowWin(int money)
    {
        if (_ended) return;
        _ended = true;

        HideAll();
        if (winPanel != null) winPanel.SetActive(true);

        if (winMoneyText != null)
            winMoneyText.text = $"Money Collected: ${money}";

        UnlockCursor();
        Time.timeScale = 0f; // pause game
    }

    public void ShowLose(int money)
    {
        if (_ended) return;
        _ended = true;

        HideAll();
        if (losePanel != null) losePanel.SetActive(true);

        if (loseMoneyText != null)
            loseMoneyText.text = $"Money Collected: ${money}";

        UnlockCursor();
        Time.timeScale = 0f; // pause game
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    void HideAll()
    {
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
