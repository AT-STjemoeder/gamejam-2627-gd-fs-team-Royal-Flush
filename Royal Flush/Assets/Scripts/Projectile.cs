using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float maxTravelDistance = 15f;

    private Vector3 direction;
    private Vector3 startPosition;
    private float speed;
    private BallWall wall;

    public void Launch(Vector3 newDirection, float newSpeed, BallWall targetWall)
    {
        direction = newDirection.normalized;
        speed = newSpeed;
        startPosition = transform.position;
        wall = targetWall;
    }

    private void Update()
    {
        transform.position += direction * (speed * Time.deltaTime);

        if (wall != null && TouchingWall())
        {
            Stick();
            return;
        }

        if (Vector3.Distance(startPosition, transform.position) > maxTravelDistance)
        {
            Destroy(gameObject);
        }
    }

    private bool TouchingWall()
    {
        float myRadius = wall.BallSize * 0.5f;

        Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, myRadius);

        foreach (Collider2D other in nearby)
        {
            if (wall.IsWallBall(other.transform))
            {
                return true;
            }
        }

        return false;
    }

    private void Stick()
    {
        transform.SetParent(wall.transform);

        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        if (collider != null)
        {
            collider.enabled = true;
        }

        AmmoBall ball = GetComponent<AmmoBall>();

        Destroy(this);
        wall.BallStuck(ball);
    }
}
