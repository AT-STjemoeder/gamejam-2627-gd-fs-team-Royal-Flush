using TMPro;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// Arcade score HUD: combo -> multiplier -> score, plus a persisted high score.
public class ScoreHUD : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text highScoreText;
    [SerializeField] TMP_Text comboText;

    [Header("Combo tuning")]
    [Tooltip("Seconden dat je hebt voor de volgende hit voordat de combo breekt.")]
    [SerializeField] float comboWindow = 2f;
    [Tooltip("Hits nodig per +1 multiplier.")]
    [SerializeField] int hitsPerMultiplier = 4;
    [SerializeField] int maxMultiplier = 8;
    [Tooltip("Spatie = fake hit, om de HUD te testen in play mode.")]
    [SerializeField] bool debugSpaceToScore;

    const string HighScoreKey = "HighScore";
    const int BarSegments = 12;

    public int Score { get; private set; }
    public int Combo { get; private set; }
    public int HighScore { get; private set; }
    public int Multiplier => Mathf.Clamp(1 + Combo / Mathf.Max(1, hitsPerMultiplier), 1, maxMultiplier);

    float comboTimer;
    int lastDrawnSegments = -1;

    void Awake()
    {
        HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        Refresh();
    }

    void Update()
    {
#if ENABLE_INPUT_SYSTEM
        if (debugSpaceToScore && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            AddHit();
#endif
        if (Combo <= 0) return;

        comboTimer -= Time.deltaTime;
        if (comboTimer <= 0f) { BreakCombo(); return; }
        if (FilledSegments != lastDrawnSegments) Refresh();
    }

    /// Roep dit aan bij elke scorende actie (kill, pickup, hit).
    public void AddHit(int basePoints = 100)
    {
        Combo++;
        comboTimer = comboWindow;
        Score += basePoints * Multiplier;

        if (Score > HighScore)
        {
            HighScore = Score;
            PlayerPrefs.SetInt(HighScoreKey, HighScore);
        }
        Refresh();
    }

    /// Combo kwijt (bij schade, mis, etc.). Score blijft staan.
    public void BreakCombo()
    {
        Combo = 0;
        comboTimer = 0f;
        Refresh();
    }

    /// Nieuwe run: score op 0, high score blijft.
    public void ResetRun()
    {
        Score = 0;
        BreakCombo();
    }

    int FilledSegments => Mathf.CeilToInt(BarSegments * comboTimer / comboWindow);

    void Refresh()
    {
        lastDrawnSegments = Combo > 0 ? FilledSegments : -1;

        if (scoreText) scoreText.text = $"SCORE\n{Score:000000}";
        if (highScoreText) highScoreText.text = $"HI\n{HighScore:000000}";
        if (!comboText) return;

        if (Combo <= 0)
        {
            comboText.text = string.Empty;
            return;
        }

        string bar = new string('|', lastDrawnSegments).PadRight(BarSegments, '.');
        comboText.text = $"x{Multiplier}\n<size=40%>{Combo} COMBO\n{bar}</size>";
    }
}
