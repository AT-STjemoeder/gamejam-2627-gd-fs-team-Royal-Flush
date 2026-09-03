using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites = new Sprite[4];

    public int ColorCount
    {
        get { return sprites.Length; }
    }

    public AmmoColor RandomColor()
    {
        return (AmmoColor)Random.Range(0, sprites.Length);
    }

    public AmmoBall Spawn(AmmoColor color, Transform parent, float size)
    {
        int index = (int)color;
        if (index >= sprites.Length || sprites[index] == null)
        {
            Debug.LogError("BallSpawner: missing sprite for " + color, this);
            return null;
        }

        GameObject ballObject = new GameObject("Ball");
        ballObject.transform.SetParent(parent);
        ballObject.transform.rotation = Quaternion.identity;
        ballObject.transform.localScale = Vector3.one * size;

        ballObject.AddComponent<SpriteRenderer>();

        AmmoBall ball = ballObject.AddComponent<AmmoBall>();
        ball.SetUp(color, sprites[index]);

        ballObject.AddComponent<CircleCollider2D>();

        return ball;
    }
}
