# Teleclone UI

The UI uses one fixed frame. It shows `drawer stage`, `clone stage`, or `judge stage`. The final scoreboard shows three score numbers only after the match ends.

## Show a stage

```csharp
view.ShowDrawer(drawingTexture);
view.ShowClone(referenceTexture, drawingTexture);
view.ShowJudge(leftTexture, rightTexture);
view.ShowScoreboard(players);
```

The pictures are Unity textures. The drawing code owns them. The UI does not decide which picture is original.

## Read the Judge's choice

```csharp
void OnEnable() { view.onJudgeChoice.AddListener(OnJudgeChoice); }
void OnDisable() { view.onJudgeChoice.RemoveListener(OnJudgeChoice); }

void OnJudgeChoice(int side)
{
    // 0 means left; 1 means right. Send this to the game/scoring code.
}
```

Each Judge stage accepts one choice. A new `ShowJudge` call allows a new choice. Beck's scoring code updates `PlayerData.score`; then the game calls `ShowScoreboard(players)` at the end. This UI does not calculate or award points.

## Preview

Use **Teleclone > UI > Build UI and open preview**, then enter Play Mode. In the five-round demo, Space advances Drawer and Cloner. Click a drawing for Judge. After the final scoreboard, Space restarts. Demo code is only for this separate preview scene and does not implement scoring.

The source project stays on Unity **6000.3.27f1 (6.3)**, which has not been tested on this machine. Local verification uses a separate copy in Unity **6000.5.10f1 (6.5)**. Do not copy that preview project's version or package changes into the source project.
