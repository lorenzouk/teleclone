using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Teleclone.UI
{
    // Shows supplied pictures and scores. Game rules stay outside this class.
    public class TelecloneUIView : MonoBehaviour
    {
        public GameObject drawingPanel;
        public GameObject judgePanel;
        public GameObject resultPanel;
        public GameObject referencePanel;
        public TMP_Text phaseText;
        public RawImage drawingImage;
        public RectTransform drawingArea;
        public RawImage referenceImage;
        public RawImage leftJudgeImage;
        public RawImage rightJudgeImage;
        public Button leftJudgeButton;
        public Button rightJudgeButton;
        public TMP_Text[] playerScoreTexts = new TMP_Text[3];
        public JudgeChoiceEvent onJudgeChoice = new JudgeChoiceEvent();

        public TelecloneStage CurrentStage { get; private set; }
        private bool interactionEnabled = true;
        private bool choiceSubmitted;

        public bool CanDraw
        {
            get
            {
                return isActiveAndEnabled && interactionEnabled &&
                    (CurrentStage == TelecloneStage.Drawer || CurrentStage == TelecloneStage.Clone);
            }
        }

        private void OnEnable()
        {
            if (leftJudgeButton != null) leftJudgeButton.onClick.AddListener(ChooseLeft);
            if (rightJudgeButton != null) rightJudgeButton.onClick.AddListener(ChooseRight);
            UpdateControls();
        }

        private void OnDisable()
        {
            if (leftJudgeButton != null) leftJudgeButton.onClick.RemoveListener(ChooseLeft);
            if (rightJudgeButton != null) rightJudgeButton.onClick.RemoveListener(ChooseRight);
        }

        public void ShowDrawer(Texture drawing)
        {
            SetTexture(drawingImage, drawing);
            SetTexture(referenceImage, null);
            SetStage(TelecloneStage.Drawer);
        }

        public void ShowClone(Texture reference, Texture drawing)
        {
            SetTexture(referenceImage, reference);
            SetTexture(drawingImage, drawing);
            SetStage(TelecloneStage.Clone);
        }

        public void ShowJudge(Texture left, Texture right)
        {
            choiceSubmitted = false;
            SetTexture(leftJudgeImage, left);
            SetTexture(rightJudgeImage, right);
            SetStage(TelecloneStage.Judge);
        }

        public void ShowScoreboard(PlayerData[] players)
        {
            for (int i = 0; playerScoreTexts != null && i < playerScoreTexts.Length; i++)
            {
                int score = 0;
                if (players != null && i < players.Length && players[i] != null)
                    score = players[i].score;
                if (playerScoreTexts[i] != null) playerScoreTexts[i].text = score.ToString();
            }
            SetStage(TelecloneStage.Scoreboard);
        }

        // The visibility controller can select a stage without changing game rules.
        public void SetStage(TelecloneStage stage)
        {
            CurrentStage = stage;
            bool drawing = stage == TelecloneStage.Drawer || stage == TelecloneStage.Clone;
            bool showReference = stage == TelecloneStage.Clone &&
                referenceImage != null && referenceImage.texture != null;
            SetActive(drawingPanel, drawing);
            SetActive(judgePanel, stage == TelecloneStage.Judge);
            SetActive(resultPanel, stage == TelecloneStage.Scoreboard);
            SetActive(referencePanel, showReference);

            if (drawingArea != null)
            {
                drawingArea.anchorMin = new Vector2(showReference ? .5f : 0f, 0f);
                drawingArea.anchorMax = Vector2.one;
                drawingArea.offsetMin = new Vector2(8, 8);
                drawingArea.offsetMax = new Vector2(-8, -8);
            }
            if (phaseText != null)
            {
                switch (stage)
                {
                    case TelecloneStage.Drawer: phaseText.text = "drawer stage"; break;
                    case TelecloneStage.Clone: phaseText.text = "clone stage"; break;
                    case TelecloneStage.Judge: phaseText.text = "judge stage"; break;
                    default: phaseText.text = ""; break;
                }
            }
            UpdateControls();
        }

        public void SetInteractionEnabled(bool enabled)
        {
            interactionEnabled = enabled;
            UpdateControls();
        }

        public void ChooseLeft() { Choose(0); }
        public void ChooseRight() { Choose(1); }

        private void Choose(int side)
        {
            if (!CanChoose()) return;
            choiceSubmitted = true;
            UpdateControls();
            if (onJudgeChoice != null) onJudgeChoice.Invoke(side);
        }

        private bool CanChoose()
        {
            return isActiveAndEnabled && interactionEnabled && !choiceSubmitted &&
                CurrentStage == TelecloneStage.Judge &&
                leftJudgeImage != null && leftJudgeImage.texture != null &&
                rightJudgeImage != null && rightJudgeImage.texture != null;
        }

        private void UpdateControls()
        {
            if (drawingImage != null) drawingImage.raycastTarget = CanDraw;
            bool canChoose = CanChoose();
            if (leftJudgeButton != null) leftJudgeButton.interactable = canChoose;
            if (rightJudgeButton != null) rightJudgeButton.interactable = canChoose;
        }

        private static void SetTexture(RawImage image, Texture texture)
        {
            if (image == null) return;
            image.texture = texture;
            image.enabled = texture != null;
            image.raycastTarget = false;
            var aspect = image.GetComponent<AspectRatioFitter>();
            if (aspect != null && texture != null && texture.height > 0)
                aspect.aspectRatio = (float)texture.width / texture.height;
        }

        private static void SetActive(GameObject panel, bool active)
        {
            if (panel != null) panel.SetActive(active);
        }
    }
}
