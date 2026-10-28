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

Open **Assets/Scenes/SampleScene.unity**, then enter Play Mode. Click inside the Game view (or the standalone game window) to give it keyboard focus. Stages wait for input; there is no automatic timer.

1. `drawer stage`: draw in the white area, then press and release **Space**.
2. `clone stage`: the reference is on the left; draw on the right, then press **Space** again.
3. `judge stage`: click the left or right picture. This starts the next round; Space does not submit a Judge choice.
4. Repeat for five rounds. The final scoreboard shows three numbers. **Space** restarts the demo.

The UI prefab is connected to the existing demo. The old Canvas is kept inactive, and the scene reuses its existing EventSystem.

The black pen and five-round flow are temporary demo components (`DemoSketchSurface` and `TelecloneUIDemo`). When the team's drawing and game scripts are ready, remove those two components from the scene and call the four methods above instead. The prefab itself contains no demo or scoring logic. Scores stay zero until Beck updates `PlayerData.score`; the existing `ScoringUI.addScore()` only changes text.

For a separate scratch scene, use **Teleclone > UI > Build UI and open preview**. This does not change SampleScene.

The source project stays on Unity **6000.3.27f1 (6.3)**, which has not been tested on this machine. Local verification uses a separate copy in Unity **6000.5.10f1 (6.5)**. Do not copy that preview project's version or package changes into the source project.
