using TMPro;
using UnityEngine;

public class ScoringUI : MonoBehaviour
{
    public TextMeshProUGUI p1ScoreText;
    public TextMeshProUGUI p2ScoreText;
    public TextMeshProUGUI p3ScoreText;

    public bool active = false;

    // simple method for changing a given player's score, can be modified easily in the future to account for more players
    public void addScore(PlayerData player, int addAmount)
    {
        switch (player.playerNum)
        {
            case 1:
                p1ScoreText.text = player.name + "'s Score: " + (player.score + addAmount).ToString();
                break;
            case 2:
                p2ScoreText.text = player.name + "'s Score: " + (player.score + addAmount).ToString();
                break;
            case 3:
                p3ScoreText.text = player.name + "'s Score: " + (player.score + addAmount).ToString();
                break;
        }
    }
}
