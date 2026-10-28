using UnityEngine;
using UnityEngine.InputSystem;

namespace Teleclone.UI.Demo
{
    // Local prototype flow. Beck supplies scores; this script never adds points.
    public class TelecloneUIDemo : MonoBehaviour
    {
        public TelecloneUIView View;
        public DemoSketchSurface SketchSurface;
        public int round = 1;
        public int totalRounds = 5;
        public PlayerData[] players =
        {
            new PlayerData { name = "", playerNum = 1, score = 0 },
            new PlayerData { name = "", playerNum = 2, score = 0 },
            new PlayerData { name = "", playerNum = 3, score = 0 }
        };

        Texture2D original;
        Texture2D clone;
        bool judgeChoicePending;

        void Start()
        {
            StartGame();
        }

        void Update()
        {
            if (View == null || Keyboard.current == null) return;
            if (!Keyboard.current.spaceKey.wasPressedThisFrame) return;
            if (View.CurrentStage == TelecloneStage.Scoreboard) StartGame();
            else NextStage();
        }

        public void StartGame()
        {
            if (View == null || SketchSurface == null) return;
            // Remove first so restarting does not subscribe the same listener twice.
            View.onJudgeChoice.RemoveListener(ChooseDrawing);
            View.onJudgeChoice.AddListener(ChooseDrawing);
            SketchSurface.View = View;
            if (original == null)
            {
                original = new Texture2D(SketchSurface.Width, SketchSurface.Height);
                clone = new Texture2D(SketchSurface.Width, SketchSurface.Height);
            }
            round = 1;
            totalRounds = Mathf.Max(1, totalRounds);
            judgeChoicePending = false;
            StartRound();
        }

        void StartRound()
        {
            SketchSurface.Load(null);
            SketchSurface.CopyTo(original);
            SketchSurface.CopyTo(clone);
            View.SetInteractionEnabled(true);
            View.ShowDrawer(SketchSurface.CanvasTexture);
        }

        public void NextStage()
        {
            if (View == null || SketchSurface == null || !View.CanDraw) return;
            if (View.CurrentStage == TelecloneStage.Drawer)
            {
                SketchSurface.CopyTo(original);
                SketchSurface.Load(clone);
                View.ShowClone(original, SketchSurface.CanvasTexture);
            }
            else if (View.CurrentStage == TelecloneStage.Clone)
            {
                SketchSurface.CopyTo(clone);
                // Alternate the two display slots in this local preview.
                if (round % 2 == 1) View.ShowJudge(original, clone);
                else View.ShowJudge(clone, original);
            }
        }

        void ChooseDrawing(int side)
        {
            if (!isActiveAndEnabled || View.CurrentStage != TelecloneStage.Judge) return;
            if (side != 0 && side != 1) return;
            // 0 means left, 1 means right. Other listeners can score this same choice.
            judgeChoicePending = true;
        }

        void LateUpdate()
        {
            if (!judgeChoicePending) return;
            judgeChoicePending = false;
            // Wait until every onJudgeChoice listener has read the current round.
            if (round >= totalRounds) View.ShowScoreboard(players);
            else
            {
                round++;
                StartRound();
            }
        }

        void OnDestroy()
        {
            if (View != null) View.onJudgeChoice.RemoveListener(ChooseDrawing);
            if (original != null) Destroy(original);
            if (clone != null) Destroy(clone);
        }
    }
}
