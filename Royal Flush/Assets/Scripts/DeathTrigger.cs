using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
[SerializeField] private DeathManager deathManager;

private void OnTriggerEnter2D(Collider2D collision)
{
    Ball ball = collision.GetComponent<Ball>();

    if (ball == null)
        return;

    deathManager.Die();
}

}
