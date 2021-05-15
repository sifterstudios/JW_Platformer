using System;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    static int _score;
    static int _highScore;


    void Start()
    {
        _highScore = PlayerPrefs.GetInt("HighScore");
    }

    public static event Action<int> OnScoreChanged;

    public static void Add(int points)
    {
        _score += points;
        Debug.Log($"Score = {_score}");
        OnScoreChanged?.Invoke(_score);

        if (_score > _highScore)
        {
            _highScore = _score;
            Debug.Log("High Score" + _highScore);
            PlayerPrefs.SetInt("HighScore", _highScore);
        }
    }
}