using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject howToPanel;

    void Start()
    {

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (howToPanel != null)
            howToPanel.SetActive(false);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Test Scene");
    }

    public void OpenHowTo()
    {
        if (howToPanel != null)
            howToPanel.SetActive(true);
    }

    public void CloseHowTo()
    {
        if (howToPanel != null)
            howToPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
