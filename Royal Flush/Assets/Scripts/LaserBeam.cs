using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    [SerializeField] private Sprite beamSprite;
    [SerializeField] private float thickness = 0.15f;
    [SerializeField] private float visibleTime = 0.08f;
    [SerializeField] private int sortingOrder = 3;

    private Transform beam;
    private float timeLeft;

    private void Awake()
    {
        if (beamSprite == null)
        {
            Debug.LogError("LaserBeam: assign the beam sprite in the Inspector.", this);
            return;
        }

        CreateBeam();
        beam.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (beam == null || !beam.gameObject.activeSelf)
        {
            return;
        }

        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0f)
        {
            beam.gameObject.SetActive(false);
        }
    }

    public void Show(float length)
    {
        if (beam == null)
        {
            return;
        }

        float spriteSize = beamSprite.bounds.size.x;

        beam.localScale = new Vector3(thickness / spriteSize, length / spriteSize, 1f);

        beam.localPosition = new Vector3(0f, length * 0.5f, 0f);

        beam.gameObject.SetActive(true);
        timeLeft = visibleTime;
    }

    private void CreateBeam()
    {
        GameObject beamObject = new GameObject("Beam");
        beamObject.transform.SetParent(transform);
        beamObject.transform.localRotation = Quaternion.identity;

        SpriteRenderer spriteRenderer = beamObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = beamSprite;
        spriteRenderer.sortingOrder = sortingOrder;

        beam = beamObject.transform;
    }
}
