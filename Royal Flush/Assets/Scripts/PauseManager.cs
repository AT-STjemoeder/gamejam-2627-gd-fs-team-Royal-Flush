using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
[SerializeField] private GameObject pauseScreen;

private bool isPaused = false;

private void Start()
{
    pauseScreen.SetActive(false);
    Time.timeScale = 1f;
}

private void Update()
{
    if (Input.GetKeyDown(KeyCode.Escape))
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }
}

public void PauseGame()
{
    isPaused = true;
    pauseScreen.SetActive(true);
    Time.timeScale = 0f;
}

public void ResumeGame()
{
    isPaused = false;
    pauseScreen.SetActive(false);
    Time.timeScale = 1f;
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
