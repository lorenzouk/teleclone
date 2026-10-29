using UnityEngine;

public class HideCanvasButton : MonoBehaviour
{
    public GameObject Canvas;

    public void toggleCanvas()
    {
        Canvas.SetActive(!Canvas.activeSelf);
    }
}
