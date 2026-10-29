
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;

public class Draw : MonoBehaviour
{
    public RectTransform drawingCanvas;
    public GameObject dotBrush;
    public GameObject strokeBrush;
    public float brushSize = 8f;
    private bool isDrawing;
    private Vector2 lastPosition;
    private List<GameObject> drawnObjects = new List<GameObject>();

    public void ClearDrawing()
    {
        for (int i = 0; i < drawnObjects.Count; i++)
        {
            if (drawnObjects[i] != null)
            {
                Destroy(drawnObjects[i]);
            }
        }
        drawnObjects.Clear();
    }

    public void SetBrushSize(float size)
    {
        brushSize = size;
    }

    void Update()
    {
        if (Mouse.current == null || drawingCanvas == null)
        {
            return;
        }

        Vector2 screenPosition = Mouse.current.position.ReadValue();
        Canvas canvas = drawingCanvas.GetComponentInParent<Canvas>();
        Camera uiCamera = null;

        if (canvas != null && canvas.rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            uiCamera = canvas.rootCanvas.worldCamera;
        }

        bool insideCanvas = RectTransformUtility.ScreenPointToLocalPointInRectangle(drawingCanvas, screenPosition, uiCamera, out Vector2 localPosition) && drawingCanvas.rect.Contains(localPosition);
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            isDrawing = insideCanvas;
            if (isDrawing)
            {
                lastPosition = localPosition;
                CreateDot(localPosition);
            }
        }

        if (Mouse.current.leftButton.isPressed && isDrawing)
        {
            if (!insideCanvas)
            {
                isDrawing = false;
                return;
            }
            float distance = Vector2.Distance(lastPosition, localPosition);
            if (distance > 0.5f)
            {
                CreateStroke(lastPosition, localPosition);
                lastPosition = localPosition;
            }
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isDrawing = false;
        }
    }

    void CreateDot(Vector2 position)
    {
        GameObject dot = Instantiate(dotBrush, drawingCanvas, false);
        RectTransform rect = dot.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = Vector2.one * brushSize;
        drawnObjects.Add(dot);
    }

    void CreateStroke(Vector2 start, Vector2 end)
    {
        float distance = Vector2.Distance(start, end);
        float spacing = Mathf.Max(1f, brushSize * 0.2f);
        int steps = Mathf.CeilToInt(distance / spacing);
        for (int i = 1; i <= steps; i++)
        {
            float t = (float)i / steps;
            Vector2 position = Vector2.Lerp(start, end, t);
            CreateDot(position);
        }
    }
}
