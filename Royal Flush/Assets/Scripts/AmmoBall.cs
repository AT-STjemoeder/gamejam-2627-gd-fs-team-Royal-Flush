using UnityEngine;

public enum AmmoColor
{
    Ember,
    Gold,
    Forest,
    Blue
}

[RequireComponent(typeof(SpriteRenderer))]
public class AmmoBall : MonoBehaviour
{
    [SerializeField] private AmmoColor colorType;

    public AmmoColor ColorType
    {
        get { return colorType; }
    }

    public void SetUp(AmmoColor newColor, Sprite sprite)
    {
        colorType = newColor;

        GetComponent<SpriteRenderer>().sprite = sprite;

        gameObject.tag = GetTagFor(newColor);
        gameObject.name = "Ball_" + newColor;
    }

    public static string GetTagFor(AmmoColor color)
    {
        return "Ball" + color;
    }
}
