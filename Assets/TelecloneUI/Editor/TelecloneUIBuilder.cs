using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Teleclone.UI.Editor
{
    public static class TelecloneUIBuilder
    {
        public const string PrefabPath = "Assets/TelecloneUI/Prefabs/TelecloneUI.prefab";
        public const string ScenePath = "Assets/TelecloneUI/Scenes/TelecloneUIPreview.unity";
        static TMP_FontAsset font;

        [MenuItem("Teleclone/UI/Build UI and open preview")]
        public static void BuildPreview()
        {
            if (Application.isPlaying) return;
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            Directory.CreateDirectory("Assets/TelecloneUI/Prefabs");
            Directory.CreateDirectory("Assets/TelecloneUI/Scenes");
            font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var camera = new GameObject("Camera", typeof(Camera), typeof(AudioListener)).GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.white;
            var root = new GameObject("TelecloneUI", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            root.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var background = root.AddComponent<Image>();
            background.color = Color.white;
            background.raycastTarget = false;
            var scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);
            scaler.matchWidthOrHeight = .5f;
            var view = root.AddComponent<TelecloneUIView>();
            view.phaseText = Label(root.transform, "Stage", "drawer stage", 24);
            SetRect(view.phaseText.rectTransform, new Vector2(0, 1), Vector2.one, new Vector2(80, -78), new Vector2(-80, -32));
            var frame = Rect(root.transform, "Frame");
            SetRect(frame, Vector2.zero, Vector2.one, new Vector2(80, 80), new Vector2(-80, -100));
            Border(frame, "Top", new Vector2(0, 1), Vector2.one, new Vector2(0, -1), Vector2.zero);
            Border(frame, "Bottom", Vector2.zero, new Vector2(1, 0), Vector2.zero, new Vector2(0, 1));
            Border(frame, "Left", Vector2.zero, new Vector2(0, 1), Vector2.zero, new Vector2(1, 0));
            Border(frame, "Right", new Vector2(1, 0), Vector2.one, new Vector2(-1, 0), Vector2.zero);
            var draw = Rect(frame, "Drawing");
            view.drawingPanel = draw.gameObject;
            var reference = Rect(draw, "Reference");
            SetRect(reference, Vector2.zero, new Vector2(.5f, 1), new Vector2(8, 8), new Vector2(-8, -8));
            view.referencePanel = reference.gameObject;
            view.referenceImage = Artwork(reference, "Reference image");
            var drawingArea = Rect(draw, "Drawing area");
            view.drawingArea = drawingArea;
            SetRect(drawingArea, Vector2.zero, Vector2.one, new Vector2(8, 8), new Vector2(-8, -8));
            view.drawingImage = Artwork(drawingArea, "Drawing image");
            var judge = Rect(frame, "Judge");
            view.judgePanel = judge.gameObject;
            var left = Rect(judge, "Left choice");
            SetRect(left, Vector2.zero, new Vector2(.5f, 1), new Vector2(8, 8), new Vector2(-8, -8));
            view.leftJudgeImage = Artwork(left, "Left drawing");
            view.leftJudgeButton = InvisibleButton(left);
            var right = Rect(judge, "Right choice");
            SetRect(right, new Vector2(.5f, 0), Vector2.one, new Vector2(8, 8), new Vector2(-8, -8));
            view.rightJudgeImage = Artwork(right, "Right drawing");
            view.rightJudgeButton = InvisibleButton(right);
            Border(judge, "Divider", new Vector2(.5f, 0), new Vector2(.5f, 1), Vector2.zero, new Vector2(1, 0));
            var scores = Rect(frame, "Scoreboard");
            view.resultPanel = scores.gameObject;
            view.playerScoreTexts = new TMP_Text[3];
            for (int i = 0; i < 3; i++)
            {
                view.playerScoreTexts[i] = Label(scores, "Score " + (i + 1), "0", 30);
                SetRect(view.playerScoreTexts[i].rectTransform, new Vector2(i / 3f, 0), new Vector2((i + 1) / 3f, 1), Vector2.zero, Vector2.zero);
            }
            judge.gameObject.SetActive(false);
            scores.gameObject.SetActive(false);
            PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            var surface = view.drawingImage.gameObject.AddComponent<Demo.DemoSketchSurface>();
            surface.Target = view.drawingImage;
            surface.View = view;
            var demo = root.AddComponent<Demo.TelecloneUIDemo>();
            demo.View = view;
            demo.SketchSurface = surface;
            var events = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            events.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            Selection.activeGameObject = root;
        }

        static RectTransform Rect(Transform parent, string name)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            SetRect(rect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            return rect;
        }
        static TMP_Text Label(Transform parent, string name, string value, int size)
        {
            var text = Rect(parent, name).gameObject.AddComponent<TextMeshProUGUI>();
            text.font = font;
            text.text = value;
            text.fontSize = size;
            text.color = Color.black;
            text.alignment = TextAlignmentOptions.Center;
            text.raycastTarget = false;
            return text;
        }
        static RawImage Artwork(Transform parent, string name)
        {
            var art = Rect(parent, name).gameObject.AddComponent<RawImage>();
            art.raycastTarget = false;
            var aspect = art.gameObject.AddComponent<AspectRatioFitter>();
            aspect.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            aspect.aspectRatio = 1.6f;
            return art;
        }
        static Button InvisibleButton(RectTransform parent)
        {
            var image = parent.gameObject.AddComponent<Image>();
            image.color = Color.clear;
            var button = parent.gameObject.AddComponent<Button>();
            button.transition = Selectable.Transition.None;
            return button;
        }
        static void Border(Transform parent, string name, Vector2 min, Vector2 max, Vector2 lower, Vector2 upper)
        {
            var line = Rect(parent, name).gameObject.AddComponent<Image>();
            line.color = Color.black;
            line.raycastTarget = false;
            SetRect(line.rectTransform, min, max, lower, upper);
        }
        static void SetRect(RectTransform rect, Vector2 min, Vector2 max, Vector2 lower, Vector2 upper)
        {
            rect.anchorMin = min; rect.anchorMax = max; rect.offsetMin = lower; rect.offsetMax = upper;
        }
    }
}
