using UnityEngine;
using UnityEngine.UI;

public static class FullScreenImage
{
    public static RawImage Create(string objectName, int sortingOrder)
    {
        GameObject canvasObject = new GameObject(objectName + "Canvas");

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortingOrder;

        GameObject imageObject = new GameObject(objectName);
        imageObject.transform.SetParent(canvasObject.transform, false);

        RawImage image = imageObject.AddComponent<RawImage>();
        image.raycastTarget = false;

        RectTransform rect = image.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        return image;
    }
}
