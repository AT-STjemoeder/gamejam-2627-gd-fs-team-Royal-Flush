using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    [SerializeField] private Sprite beamSprite;
    [SerializeField] private float thickness = 0.06f;
    [SerializeField] private float flashTime = 0.08f;
    [SerializeField] private Color aimColor = new Color(1f, 0.4f, 0.3f, 0.3f);
    [SerializeField] private Color fireColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private int sortingOrder = 3;

    private Transform beam;
    private SpriteRenderer beamRenderer;
    private float flashLeft;

    private void Awake()
    {
        if (beamSprite == null)
        {
            Debug.LogError("LaserBeam: assign the beam sprite in the Inspector.", this);
            return;
        }

        CreateBeam();
    }

    private void Update()
    {
        if (beamRenderer == null)
        {
            return;
        }

        if (flashLeft > 0f)
        {
            flashLeft -= Time.deltaTime;
            beamRenderer.color = fireColor;
        }
        else
        {
            beamRenderer.color = aimColor;
        }
    }

    public void SetLength(float length)
    {
        if (beam == null)
        {
            return;
        }

        float spriteSize = beamSprite.bounds.size.x;

        beam.localScale = new Vector3(thickness / spriteSize, length / spriteSize, 1f);
        beam.localPosition = new Vector3(0f, length * 0.5f, 0f);
    }

    public void Flash()
    {
        flashLeft = flashTime;
    }

    private void CreateBeam()
    {
        GameObject beamObject = new GameObject("Beam");
        beamObject.transform.SetParent(transform);
        beamObject.transform.localRotation = Quaternion.identity;

        beamRenderer = beamObject.AddComponent<SpriteRenderer>();
        beamRenderer.sprite = beamSprite;
        beamRenderer.sortingOrder = sortingOrder;
        beamRenderer.color = aimColor;

        beam = beamObject.transform;
    }
}
