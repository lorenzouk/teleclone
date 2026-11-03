using TMPro;
using UnityEngine;

public class ScoringUI : MonoBehaviour
{
    public TextMeshProUGUI p1ScoreText;
    public TextMeshProUGUI p2ScoreText;
    public TextMeshProUGUI p3ScoreText;

    public PlayerData[] players =
    {
        new PlayerData { name = "Player 1", playerNum = 1, score = 0 },
        new PlayerData { name = "Player 2", playerNum = 2, score = 0 },
        new PlayerData { name = "Player 3", playerNum = 3, score = 0 }
    };

    void OnEnable()
    {
        RefreshScores();
    }

    // The button supplies player number 1, 2, or 3 in the Inspector.
    public void AddPointToPlayer(int playerNum)
    {
        ChangePlayerScore(playerNum, 1);
    }

    public void SubtractPointFromPlayer(int playerNum)
    {
        ChangePlayerScore(playerNum, -1);
    }

    public void ResetScores()
    {
        if (players == null) return;
        foreach (PlayerData player in players)
            if (player != null) player.score = 0;

        RefreshScores();
    }

    void ChangePlayerScore(int playerNum, int amount)
    {
        if (players == null || playerNum < 1 || playerNum > 3) return;

        foreach (PlayerData player in players)
        {
            if (player != null && player.playerNum == playerNum)
            {
                addScore(player, amount);
                return;
            }
        }
    }

    public void addScore(PlayerData player, int addAmount)
    {
        if (player == null || player.playerNum < 1 || player.playerNum > 3) return;
        player.score += addAmount;
        // Keep the same data when another game script supplies the player.
        if (players != null)
        {
            for (int i = 0; i < players.Length; i++)
                if (players[i] != null && players[i].playerNum == player.playerNum)
                    players[i] = player;
        }
        ShowScore(player);
    }

    public void RefreshScores()
    {
        if (players == null) return;
        foreach (PlayerData player in players) ShowScore(player);
    }

    void ShowScore(PlayerData player)
    {
        if (player == null) return;
        TextMeshProUGUI scoreText;

        switch (player.playerNum)
        {
            case 1: scoreText = p1ScoreText; break;
            case 2: scoreText = p2ScoreText; break;
            case 3: scoreText = p3ScoreText; break;
            default: return;
        }

        if (scoreText == null) return;
        string playerName = string.IsNullOrEmpty(player.name) ? "Player " + player.playerNum : player.name;
        scoreText.text = playerName + ": " + player.score;
    }
}
