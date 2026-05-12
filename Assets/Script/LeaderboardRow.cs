using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LeaderboardRow : MonoBehaviour
{
    public TMP_Text rankText;
    public TMP_Text nameText;
    public TMP_Text scoreText;
    public Image bg;

    public void Set(int rank, string name, int score, bool highlight)
    {
        rankText.text = rank.ToString();
        nameText.text = name;
        scoreText.text = score.ToString();

        if (highlight)
        {
            nameText.text = name + " (You)";
            bg.color = Color.yellow;

            rankText.color = Color.black;
            nameText.color = Color.black;
            scoreText.color = Color.black;
        }
    }
}