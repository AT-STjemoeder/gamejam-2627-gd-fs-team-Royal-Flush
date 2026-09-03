using UnityEngine;
using UnityEngine.InputSystem;

public class Turret : MonoBehaviour
{
    [Header("Parts")]
    [SerializeField] private Transform head;
    [SerializeField] private LaserBeam laserBeam;
    [SerializeField] private AmmoRing ammoRing;
    [SerializeField] private BallWall ballWall;

    [Header("Shooting")]
    [SerializeField] private float laserRange = 12f;
    [SerializeField] private float shotCooldown = 0.25f;
    [SerializeField] private float ballSpeed = 9f;

    private float cooldownLeft;

    private void Awake()
    {
        if (head == null || laserBeam == null || ammoRing == null || ballWall == null)
        {
            Debug.LogError("Turret is missing something in the Inspector", this);
        }
    }

    private void Update()
    {
        if (ballWall.IsGameOver)
        {
            return;
        }

        Aim();

        cooldownLeft -= Time.deltaTime;

        if (ShootPressed() && cooldownLeft <= 0f)
        {
            Shoot();
            cooldownLeft = shotCooldown;
        }
    }

    private void Aim()
    {
        Vector2 aim = GetAimDirection();

        if (aim.magnitude > 0.1f)
        {
            head.up = aim;
        }
    }

    private Vector2 GetAimDirection()
    {
        if (Gamepad.current != null)
        {
            Vector2 stick = Gamepad.current.rightStick.ReadValue();

            if (stick.magnitude > 0.2f)
            {
                return stick;
            }
        }

        if (Mouse.current == null)
        {
            return Vector2.zero;
        }

        Vector2 mousePixels = Mouse.current.position.ReadValue();
        Vector3 mouseInWorld = Camera.main.ScreenToWorldPoint(mousePixels);

        return mouseInWorld - head.position;
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

    private void Shoot()
    {
        RaycastHit2D hit = Physics2D.Raycast(head.position, head.up, laserRange);

        float beamLength = laserRange;

        if (hit.collider != null)
        {
            beamLength = hit.distance;

            AmmoBall ball = hit.collider.GetComponent<AmmoBall>();

            if (ball != null && ammoRing.IsRingBall(ball.transform))
            {
                Launch(ball);
            }
        }

        laserBeam.Show(beamLength);
    }

    private void Launch(AmmoBall ball)
    {
        ball.transform.SetParent(null);
        ball.transform.localScale = Vector3.one * ballWall.BallSize;
        ball.GetComponent<CircleCollider2D>().enabled = false;

        Projectile projectile = ball.gameObject.AddComponent<Projectile>();
        projectile.Launch(head.up, ballSpeed, ballWall);
    }
}
