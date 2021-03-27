using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField]
    Text Score, Move;
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
}