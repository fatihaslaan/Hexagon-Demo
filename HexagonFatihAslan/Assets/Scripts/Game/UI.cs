using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField]
    Text Score, Move, HintButton;
    [SerializeField]
    GameObject GameOverPanel;

    void Update()
    {
        Score.text = " Score: " + Global.GameScore;
        Move.text = "Move: " + Global.Move;
        if (Global.GameOver)
            GameOverPanel.SetActive(true);
    }

    public void Restart()
    {
        Global.GameOver = false;
        Global.GameScore = 0;
        Global.Move = 0;
        Global.BombSpawned = false;
        Global.TotalBombsSpawned = 0;
        SceneManager.LoadScene(0);
    }

    public void Hints()
    {
        HintButton.text = Global.Hints ? "Enable Hints" : "Disable Hints";
        Global.Hints = !Global.Hints;
    }
}