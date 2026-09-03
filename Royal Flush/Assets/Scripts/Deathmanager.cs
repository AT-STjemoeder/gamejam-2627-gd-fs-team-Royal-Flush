using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathManager : MonoBehaviour
{
[SerializeField] private GameObject deathScreen;

private bool isDead = false;

private void Start()
{
    deathScreen.SetActive(false);
    Time.timeScale = 1f;
}

public void Die()
{
    if (isDead)
        return;

    isDead = true;

    deathScreen.SetActive(true);

    Time.timeScale = 0f;
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
