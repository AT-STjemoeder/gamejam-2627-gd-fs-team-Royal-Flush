using UnityEngine;

public class AmmoRing : MonoBehaviour
{
    [Header("Ring")]
    [SerializeField] private BallSpawner spawner;
    [SerializeField] private float radius = 0.9f;
    [SerializeField] private float ballSize = 0.6f;

    [Header("Spinning")]
    [SerializeField] private float rotationSpeed = 40f;
    [SerializeField] private bool spinClockwise;

    [Header("Refill")]
    [SerializeField] private float refillDelay = 0.5f;


    private float refillTimer;

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

    private int BallsThatFit()
    {
        return Mathf.Max(6, Mathf.CeilToInt(2f * Mathf.PI * radius / ballSize));
    }

    public bool IsRingBall(Transform ball)
    {
        return ball.parent == transform;
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
        if (spawner == null)
        {
            Debug.LogError("AmmoRing: assign the Ball Spawner in the Inspector.", this);
            enabled = false;
            return;
        }

        int ballCount = BallsThatFit();
        float angleStep = 360f / ballCount;

        for (int i = 0; i < ballCount; i++)
        {
            float angle = i * angleStep;

            AmmoColor color = (AmmoColor)(i % spawner.ColorCount);
            AmmoBall ball = CreateBall(color, transform);

            ball.transform.localPosition = PositionOnRing(angle);
        }
    }

    public AmmoBall CreateBall(AmmoColor color, Transform parent)
    {
        return spawner.Spawn(color, parent, ballSize);
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
