using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager instance;

    public GameObject pausePanel;

    public bool IsPaused { get; private set; }

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    public void TogglePause()
    {
        IsPaused = !IsPaused;

        pausePanel.SetActive(IsPaused);
        Time.timeScale = IsPaused ? 0f : 1f;
    }

    public void Resume()
    {
        if (IsPaused) TogglePause();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }
}