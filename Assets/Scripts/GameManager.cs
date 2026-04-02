using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private PlayerHealth playerHealth;
    private bool isGameOver;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
        playerHealth.OnDead += OnGameOver;
    }

    void OnGameOver()
    {
        isGameOver = true;
        PauseManager.instance?.gameObject.SetActive(false);
    }

    // Player의 Death 애니메이션 이벤트에서 호출됨
    public void RestartLevel()
    {
        ScoreManager.instance?.ResetScore();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}