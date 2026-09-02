using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public enum BallColor
    {
        Red,
        Blue,
        Green,
        Yellow
    }

    [SerializeField] private BallColor ballColor;
    [SerializeField] private bool Bullet;
    [SerializeField] private bool ChainReaction = true;
    [SerializeField] private float NeighborRadius = 1.1f;
    [SerializeField] private float ChainDelay = 0.05f;

    private bool ClearStarted;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Bullet || ClearStarted)
            return;

        Ball hitBall = collision.GetComponent<Ball>();

        if (hitBall == null)
            return;
        if (hitBall == this)
            return;
        if (hitBall.ballColor != ballColor)
            return;

        ClearStarted = true;

        StartCoroutine(ClearBalls(hitBall));
    }


    private IEnumerator ClearBalls(Ball firstBall)
    {
        HashSet<Ball> checkedBalls = new HashSet<Ball>();
        List<Ball> currentWave = new List<Ball>();

        currentWave.Add(firstBall);
        checkedBalls.Add(firstBall);

        if (!ChainReaction)
        {
            Destroy(firstBall.gameObject);
            Destroy(gameObject);
            yield break;
        }

        while (currentWave.Count > 0)
        {
            List<Ball> nextWave = new List<Ball>();

            foreach (Ball currentBall in currentWave)
            {
                if (currentBall == null)
                    continue;

                Collider2D[] nearbyBalls =
                    Physics2D.OverlapCircleAll(
                        currentBall.transform.position,
                        NeighborRadius
                    );

                foreach (Collider2D nearbyCollider in nearbyBalls)
                {
                    Ball nearbyBall = nearbyCollider.GetComponent<Ball>();

                    if (nearbyBall == null)
                        continue;

                    if (nearbyBall.Bullet)
                        continue;

                    if (checkedBalls.Contains(nearbyBall))
                        continue;

                    if (nearbyBall.ballColor != ballColor)
                        continue;

                    checkedBalls.Add(nearbyBall);
                    nextWave.Add(nearbyBall);
                }
            }

            foreach (Ball ball in currentWave)
            {
                if (ball != null)
                    Destroy(ball.gameObject);
            }

            currentWave = nextWave;

            yield return new WaitForSeconds(ChainDelay);
        }
        Destroy(gameObject);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, NeighborRadius);
    }
}