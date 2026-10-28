using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Teleclone.UI.Demo
{
    // Simple black pen for the preview scene, not the production drawing module.
    public class DemoSketchSurface : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public RawImage Target;
        public TelecloneUIView View;
        public int Width = 720;
        public int Height = 450;
        public Texture2D CanvasTexture { get; private set; }

        Color32[] pixels;
        Vector2 lastPoint;
        bool drawing;

        public void Load(Texture2D source)
        {
            if (CanvasTexture == null) CanvasTexture = new Texture2D(Width, Height);
            drawing = false;
            pixels = source == null ? new Color32[Width * Height] : source.GetPixels32();
            if (source == null) Clear();
            else Apply();
            Target.texture = CanvasTexture;
        }

        public void CopyTo(Texture2D destination)
        {
            destination.SetPixels32(pixels);
            destination.Apply();
        }

        public void Clear()
        {
            drawing = false;
            for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
            Apply();
        }

        public void OnPointerDown(PointerEventData data)
        {
            if (View == null || !View.CanDraw || data.button != PointerEventData.InputButton.Left) return;
            Vector2 point;
            if (!GetPoint(data, out point)) return;
            drawing = true;
            lastPoint = point;
            DrawDot(point);
            Apply();
        }

        public void OnDrag(PointerEventData data)
        {
            if (!drawing || View == null || !View.CanDraw) return;
            Vector2 point;
            if (!GetPoint(data, out point)) { drawing = false; return; }
            int steps = Mathf.Max(1, Mathf.CeilToInt(Vector2.Distance(lastPoint, point)));
            for (int i = 1; i <= steps; i++) DrawDot(Vector2.Lerp(lastPoint, point, (float)i / steps));
            lastPoint = point;
            Apply();
        }

        public void OnPointerUp(PointerEventData data)
        {
            drawing = false;
        }

        bool GetPoint(PointerEventData data, out Vector2 point)
        {
            point = Vector2.zero;
            Vector2 local;
            Rect rect = Target.rectTransform.rect;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(Target.rectTransform,
                data.position, data.pressEventCamera, out local) || !rect.Contains(local)) return false;
            point.x = (local.x - rect.xMin) / rect.width * (Width - 1);
            point.y = (local.y - rect.yMin) / rect.height * (Height - 1);
            return true;
        }

        void DrawDot(Vector2 point)
        {
            int centerX = Mathf.RoundToInt(point.x);
            int centerY = Mathf.RoundToInt(point.y);
            for (int y = centerY - 2; y <= centerY + 2; y++)
                for (int x = centerX - 2; x <= centerX + 2; x++)
                    if (x >= 0 && x < Width && y >= 0 && y < Height)
                        pixels[y * Width + x] = Color.black;
        }

        void Apply()
        {
            CanvasTexture.SetPixels32(pixels);
            CanvasTexture.Apply();
        }

        void OnDestroy()
        {
            if (CanvasTexture != null) Destroy(CanvasTexture);
        }
    }
}
