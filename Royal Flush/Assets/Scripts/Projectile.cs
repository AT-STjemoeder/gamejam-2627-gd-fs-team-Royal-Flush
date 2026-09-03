using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 9f;
    [SerializeField] private float maxTravelDistance = 15f;

    private Vector3 direction;
    private Vector3 startPosition;

    public void Launch(Vector3 newDirection, float newSpeed)
    {
        direction = newDirection.normalized;
        speed = newSpeed;
        startPosition = transform.position;
    }

    private void Update()
    {
        transform.position += direction * (speed * Time.deltaTime);

        if (Vector3.Distance(startPosition, transform.position) > maxTravelDistance)
        {
            Destroy(gameObject);
        }
    }
}
