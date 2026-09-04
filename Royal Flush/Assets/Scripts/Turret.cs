using UnityEngine;
using UnityEngine.InputSystem;

public class Turret : MonoBehaviour
{
    [Header("Parts")]
    [SerializeField] private Transform head;
    [SerializeField] private LaserBeam laserBeam;
    [SerializeField] private AmmoRing ammoRing;
    [SerializeField] private BallWall ballWall;

    [Header("Aiming")]
    [SerializeField] private float turnSpeed = 110f;

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

        RaycastHit2D hit = Physics2D.Raycast(head.position, head.up, laserRange);

        float beamLength = laserRange;

        if (hit.collider != null)
        {
            beamLength = hit.distance;
        }

        laserBeam.SetLength(beamLength);

        cooldownLeft -= Time.deltaTime;

        if (ShootPressed() && cooldownLeft <= 0f)
        {
            Shoot(hit);
            cooldownLeft = shotCooldown;
        }
    }

    private void Aim()
    {
        float turn = GetTurnInput();

        if (turn != 0f)
        {
            head.Rotate(0f, 0f, turn * turnSpeed * Time.deltaTime);
        }
    }

    private float GetTurnInput()
    {
        float turn = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            {
                turn = turn + 1f;
            }

            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            {
                turn = turn - 1f;
            }
        }

        if (Gamepad.current != null)
        {
            turn = turn - Gamepad.current.leftStick.x.ReadValue();
            turn = turn - Gamepad.current.dpad.x.ReadValue();
        }

        return Mathf.Clamp(turn, -1f, 1f);
    }

    private bool ShootPressed()
    {
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

    private void Shoot(RaycastHit2D hit)
    {
        laserBeam.Flash();

        if (hit.collider == null)
        {
            return;
        }

        AmmoBall ball = hit.collider.GetComponent<AmmoBall>();

        if (ball != null && ammoRing.IsRingBall(ball.transform))
        {
            Launch(ball);
        }
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
