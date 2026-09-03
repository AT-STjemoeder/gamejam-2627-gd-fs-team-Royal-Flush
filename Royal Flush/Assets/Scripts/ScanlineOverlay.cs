using UnityEngine;
using UnityEngine.UI;

public class ScanlineOverlay : MonoBehaviour
{
    [SerializeField] private Texture scanlineTexture;
    [SerializeField] private float scale = 3f;
    [SerializeField] private float opacity = 0.45f;

    private RawImage overlayImage;
    private int builtWidth;
    private int builtHeight;

    private void Awake()
    {
        if (scanlineTexture == null)
        {
            Debug.LogError("ScanlineOverlay: assign the scanline texture.", this);
            enabled = false;
            return;
        }

        overlayImage = FullScreenImage.Create("Scanlines", 101);
        overlayImage.texture = scanlineTexture;
        overlayImage.color = new Color(1f, 1f, 1f, opacity);

        Rebuild();
    }

    private void Update()
    {
        if (Screen.width != builtWidth || Screen.height != builtHeight)
        {
            Rebuild();
        }
    }

    private void Rebuild()
    {
        builtWidth = Screen.width;
        builtHeight = Screen.height;

        float tileWidth = scanlineTexture.width * scale;
        float tileHeight = scanlineTexture.height * scale;

        overlayImage.uvRect = new Rect(
            0f, 0f, builtWidth / tileWidth, builtHeight / tileHeight);
    }
}
