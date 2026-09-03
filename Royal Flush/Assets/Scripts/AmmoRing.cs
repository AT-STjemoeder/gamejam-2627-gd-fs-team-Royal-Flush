using UnityEngine;

public class AmmoRing : MonoBehaviour
{
    [Header("Ring")]
    [SerializeField] private int ballCount = 18;
    [SerializeField] private float radius = 2.8f;
    [SerializeField] private float ballSize = 1f;

    [Header("Spinning")]
    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private bool spinClockwise;

    [Header("Refill")]
    [SerializeField] private float refillDelay = 0.5f;

    [Header("Ball sprites (same order as AmmoColor)")]
    [SerializeField] private Sprite[] ballSprites = new Sprite[6];

    private float refillTimer;

    public float Radius
    {
        get { return radius; }
    }

    private void Start()
    {
        BuildRing();
    }

    private void Update()
    {
        float direction = spinClockwise ? -1f : 1f;
        transform.Rotate(0f, 0f, rotationSpeed * direction * Time.deltaTime);

        KeepBallsUpright();

        if (BallsLeft() > 0)
        {
            refillTimer = refillDelay;
            return;
        }

        refillTimer -= Time.deltaTime;

        if (refillTimer <= 0f)
        {
            BuildRing();
        }
    }

    private void KeepBallsUpright()
    {
        foreach (Transform child in transform)
        {
            child.rotation = Quaternion.identity;
        }
    }

    public int BallsLeft()
    {
        int count = 0;

        foreach (Transform child in transform)
        {
            if (child.GetComponent<AmmoBall>() != null)
            {
                count++;
            }
        }

        return count;
    }

    private void BuildRing()
    {
        if (ballSprites.Length == 0 || ballSprites[0] == null)
        {
            Debug.LogError("AmmoRing: assign the ball sprites in the Inspector.", this);
            enabled = false;
            return;
        }

        float angleStep = 360f / ballCount;

        for (int i = 0; i < ballCount; i++)
        {
            float angle = i * angleStep;

            AmmoColor color = (AmmoColor)(i % ballSprites.Length);
            AmmoBall ball = CreateBall(color, transform);

            ball.transform.localPosition = PositionOnRing(angle);
        }
    }

    public AmmoBall CreateBall(AmmoColor color, Transform parent)
    {
        int spriteIndex = (int)color;
        if (spriteIndex >= ballSprites.Length)
        {
            Debug.LogError("AmmoRing: no sprite for colour " + color, this);
            return null;
        }

        GameObject ballObject = new GameObject("Ball");
        ballObject.transform.SetParent(parent);
        ballObject.transform.rotation = Quaternion.identity;
        ballObject.transform.localScale = Vector3.one * ballSize;

        ballObject.AddComponent<SpriteRenderer>();

        AmmoBall ball = ballObject.AddComponent<AmmoBall>();
        ball.SetUp(color, ballSprites[spriteIndex]);

        ballObject.AddComponent<CircleCollider2D>();

        return ball;
    }

    private Vector3 PositionOnRing(float angleInDegrees)
    {
        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;

        float x = Mathf.Cos(angleInRadians) * radius;
        float y = Mathf.Sin(angleInRadians) * radius;

        return new Vector3(x, y, 0f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
