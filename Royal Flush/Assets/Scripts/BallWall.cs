using System.Collections.Generic;
using UnityEngine;

public class BallWall : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private BallSpawner spawner;
    [SerializeField] private float startSize = 5.2f;
    [SerializeField] private float ballSize = 0.8f;
    [SerializeField] private int startRings = 3;

    [Header("Closing in")]
    [SerializeField] private float ringSpacing = 0.9f;
    [SerializeField] private float secondsPerWave = 15f;
    [SerializeField] private float loseSize = 1.6f;

    [Header("Matching")]
    [SerializeField] private int ballsNeeded = 4;
    [SerializeField] private float neighbourRadius = 1.1f;

    private float timer;
    private bool gameOver;

    public float BallSize
    {
        get { return ballSize; }
    }

    public bool IsGameOver
    {
        get { return gameOver; }
    }

    public float TimeUntilNextWave
    {
        get { return timer; }
    }

    private void Start()
    {
        for (int i = 0; i < startRings; i++)
        {
            AddWave();
        }

        timer = secondsPerWave;
    }

    private void Update()
    {
        if (gameOver)
        {
            return;
        }

        if (ClosestBallSize() <= loseSize)
        {
            gameOver = true;
            return;
        }

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            timer = secondsPerWave;
            AddWave();
        }
    }

    private float SquareSize(Vector3 position)
    {
        return Mathf.Max(Mathf.Abs(position.x), Mathf.Abs(position.y));
    }

    private float ClosestBallSize()
    {
        float closest = startSize;

        foreach (Transform child in transform)
        {
            float size = SquareSize(child.localPosition);

            if (size < closest)
            {
                closest = size;
            }
        }

        return closest;
    }

    private Vector3 PointOnSquare(float around, float size)
    {
        float side = around * 4f;

        if (side < 1f)
        {
            return new Vector3(Mathf.Lerp(-size, size, side), size, 0f);
        }

        if (side < 2f)
        {
            return new Vector3(size, Mathf.Lerp(size, -size, side - 1f), 0f);
        }

        if (side < 3f)
        {
            return new Vector3(Mathf.Lerp(size, -size, side - 2f), -size, 0f);
        }

        return new Vector3(-size, Mathf.Lerp(-size, size, side - 3f), 0f);
    }

    private void AddWave()
    {
        MoveWallInward();
        SpawnRing(startSize);
        RemoveFloatingBalls();
    }

    public void Restart()
    {
        List<Transform> balls = new List<Transform>();

        foreach (Transform child in transform)
        {
            balls.Add(child);
        }

        foreach (Transform ball in balls)
        {
            RemoveBall(ball);
        }

        gameOver = false;
        timer = secondsPerWave;

        for (int i = 0; i < startRings; i++)
        {
            AddWave();
        }
    }

    private void RemoveBall(Transform ball)
    {
        ball.SetParent(null);
        Destroy(ball.gameObject);
    }

    private void MoveWallInward()
    {
        List<Transform> balls = new List<Transform>();

        foreach (Transform child in transform)
        {
            balls.Add(child);
        }

        float squeezedOut = 0f;

        foreach (Transform ball in balls)
        {
            float size = SquareSize(ball.localPosition);

            if (size <= ringSpacing)
            {
                continue;
            }

            float shrink = (size - ringSpacing) / size;

            squeezedOut += 1f - shrink;

            if (squeezedOut >= 1f)
            {
                squeezedOut -= 1f;
                RemoveBall(ball);
                continue;
            }

            ball.localPosition = ball.localPosition * shrink;
        }
    }

    private void SpawnRing(float size)
    {
        int count = Mathf.Max(8, Mathf.CeilToInt(8f * size / ballSize));

        for (int i = 0; i < count; i++)
        {
            AmmoBall ball = spawner.Spawn(spawner.RandomColor(), transform, ballSize);

            if (ball == null)
            {
                return;
            }

            ball.transform.localPosition = PointOnSquare((float)i / count, size);
        }
    }

    public bool IsWallBall(Transform ball)
    {
        return ball.parent == transform;
    }

    public void BallStuck(AmmoBall ball)
    {
        List<AmmoBall> group = FindSameColorGroup(ball);

        if (group.Count < ballsNeeded)
        {
            return;
        }

        foreach (AmmoBall matched in group)
        {
            RemoveBall(matched.transform);
        }

        RemoveFloatingBalls();
    }

    private void RemoveFloatingBalls()
    {
        Physics2D.SyncTransforms();

        List<AmmoBall> connected = new List<AmmoBall>();

        foreach (Transform child in transform)
        {
            AmmoBall ball = child.GetComponent<AmmoBall>();

            if (ball == null)
            {
                continue;
            }

            if (SquareSize(child.localPosition) >= startSize - ringSpacing * 0.5f)
            {
                connected.Add(ball);
            }
        }

        int checkIndex = 0;

        while (checkIndex < connected.Count)
        {
            AmmoBall current = connected[checkIndex];
            checkIndex++;

            Collider2D[] nearby = Physics2D.OverlapCircleAll(
                current.transform.position, neighbourRadius);

            foreach (Collider2D other in nearby)
            {
                AmmoBall otherBall = other.GetComponent<AmmoBall>();

                if (otherBall == null)
                {
                    continue;
                }

                if (otherBall.transform.parent != transform)
                {
                    continue;
                }

                if (connected.Contains(otherBall))
                {
                    continue;
                }

                connected.Add(otherBall);
            }
        }

        List<Transform> floating = new List<Transform>();

        foreach (Transform child in transform)
        {
            AmmoBall ball = child.GetComponent<AmmoBall>();

            if (ball != null && !connected.Contains(ball))
            {
                floating.Add(child);
            }
        }

        foreach (Transform ball in floating)
        {
            RemoveBall(ball);
        }
    }

    private List<AmmoBall> FindSameColorGroup(AmmoBall firstBall)
    {
        Physics2D.SyncTransforms();

        List<AmmoBall> group = new List<AmmoBall>();
        group.Add(firstBall);

        int checkIndex = 0;

        while (checkIndex < group.Count)
        {
            AmmoBall current = group[checkIndex];
            checkIndex++;

            Collider2D[] nearby = Physics2D.OverlapCircleAll(
                current.transform.position, neighbourRadius);

            foreach (Collider2D other in nearby)
            {
                AmmoBall otherBall = other.GetComponent<AmmoBall>();

                if (otherBall == null)
                {
                    continue;
                }

                if (otherBall.transform.parent != transform)
                {
                    continue;
                }

                if (otherBall.ColorType != firstBall.ColorType)
                {
                    continue;
                }

                if (group.Contains(otherBall))
                {
                    continue;
                }

                group.Add(otherBall);
            }
        }

        return group;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, Vector3.one * startSize * 2f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, Vector3.one * loseSize * 2f);
    }
}
