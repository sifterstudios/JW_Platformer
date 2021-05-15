using System;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    static int _highScore;
    public static int Score { get; private set; }


    void Start()
    {
        _highScore = PlayerPrefs.GetInt("HighScore");
        Score = 0;
    }

    public static event Action<int> OnScoreChanged;

    public static void Add(int points)
    {
        Score += points;
        Debug.Log($"Score = {Score}");
        OnScoreChanged?.Invoke(Score);

        if (Score > _highScore)
        {
            _highScore = Score;
            Debug.Log("High Score" + _highScore);
            PlayerPrefs.SetInt("HighScore", _highScore);
        }
    }
}