using UnityEngine;
using UnityEngine.InputSystem;

public class Turret : MonoBehaviour
{
    [Header("Head")]
    [SerializeField] private Transform head;
    [SerializeField] private float turnSpeed = 540f;

    [Header("Laser")]
    [SerializeField] private LaserBeam laserBeam;
    [SerializeField] private float laserRange = 12f;
    [SerializeField] private float shotCooldown = 0.25f;

    [Header("Knocked ball")]
    [SerializeField] private float ballSpeed = 9f;

    [Header("Gamepad")]
    [SerializeField] private float stickDeadZone = 0.2f;

    private float cooldownLeft;

    private void Update()
    {
        if (head == null)
        {
            return;
        }

        AimHead();

        cooldownLeft -= Time.deltaTime;

        if (ShootPressed() && cooldownLeft <= 0f)
        {
            FireLaser();
            cooldownLeft = shotCooldown;
        }
    }

    private void AimHead()
    {
        Vector2 aim = GetAimDirection();
        if (aim.sqrMagnitude < 0.001f)
        {
            return;
        }

        float targetAngle = (Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg) - 90f;

        float newAngle = Mathf.MoveTowardsAngle(
            head.eulerAngles.z, targetAngle, turnSpeed * Time.deltaTime);

        head.rotation = Quaternion.Euler(0f, 0f, newAngle);
    }

    private Vector2 GetAimDirection()
    {
        if (Gamepad.current != null)
        {
            Vector2 stick = Gamepad.current.rightStick.ReadValue();
            if (stick.magnitude > stickDeadZone)
            {
                return stick;
            }
        }

        if (Mouse.current == null || Camera.main == null)
        {
            return Vector2.zero;
        }

        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

        Vector3 viewportPoint = new Vector3(
            mouseScreenPosition.x / Screen.width,
            mouseScreenPosition.y / Screen.height,
            0f);

        Vector3 mouseWorldPosition = Camera.main.ViewportToWorldPoint(viewportPoint);

        return (Vector2)(mouseWorldPosition - head.position);
    }

    private bool ShootPressed()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            return true;
        }

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            return true;
        }

        if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            return true;
        }

        return false;
    }

    private void FireLaser()
    {
        Vector2 direction = head.up;
        RaycastHit2D hit = Physics2D.Raycast(head.position, direction, laserRange);

        float beamLength = laserRange;

        if (hit.collider != null)
        {
            beamLength = hit.distance;

            AmmoBall ball = hit.collider.GetComponent<AmmoBall>();
            if (ball != null)
            {
                KnockBallLoose(ball, direction);
            }
        }

        if (laserBeam != null)
        {
            laserBeam.Show(beamLength);
        }
    }

    private void KnockBallLoose(AmmoBall ball, Vector2 direction)
    {
        ball.transform.SetParent(null);
        ball.GetComponent<CircleCollider2D>().enabled = false;

        Projectile projectile = ball.gameObject.AddComponent<Projectile>();
        projectile.Launch(direction, ballSpeed);
    }

    private void OnDrawGizmosSelected()
    {
        if (head == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawLine(head.position, head.position + (head.up * laserRange));
    }
}
