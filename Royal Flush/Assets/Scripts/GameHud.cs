using UnityEngine;
using UnityEngine.UI;

public class GameHud : MonoBehaviour
{
    [SerializeField] private BallWall ballWall;
    [SerializeField] private float restartDelay = 4f;
    [SerializeField] private int waveTextSize = 26;
    [SerializeField] private int gameOverTextSize = 60;

    private Text waveText;
    private Text gameOverText;
    private float restartLeft;

    private void Start()
    {
        GameObject canvasObject = new GameObject("HudCanvas");

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 200;

        waveText = CreateText(canvasObject.transform, "WaveText",
            waveTextSize, TextAnchor.UpperCenter);

        gameOverText = CreateText(canvasObject.transform, "GameOverText",
            gameOverTextSize, TextAnchor.MiddleCenter);
    }

    private void Update()
    {
        if (ballWall == null)
        {
            return;
        }

        if (ballWall.IsGameOver)
        {
            waveText.text = "";

            restartLeft -= Time.deltaTime;
            gameOverText.text = "GAME OVER\n\n" + Mathf.CeilToInt(restartLeft);

            if (restartLeft <= 0f)
            {
                ballWall.Restart();
            }

            return;
        }

        restartLeft = restartDelay;

        gameOverText.text = "";

        int seconds = Mathf.CeilToInt(ballWall.TimeUntilNextWave);

        if (seconds < 0)
        {
            seconds = 0;
        }

        waveText.text = "NEXT WAVE IN " + seconds;
    }

    private Text CreateText(Transform parent, string objectName, int size, TextAnchor anchor)
    {
        GameObject textObject = new GameObject(objectName);
        textObject.transform.SetParent(parent, false);

        Text text = textObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = size;
        text.alignment = anchor;
        text.color = new Color(1f, 0.85f, 0.8f);
        text.raycastTarget = false;

        RectTransform rect = text.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(0f, 0f);
        rect.offsetMax = new Vector2(0f, -24f);

        return text;
    }
}
