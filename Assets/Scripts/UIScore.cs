using TMPro;
using UnityEngine;

public class UIScore : MonoBehaviour
{
    TMP_Text _text;

    void Start()
    {
        _text = GetComponent<TMP_Text>();
        ScoreSystem.OnScoreChanged += UpdateScoreText;
    }

    void UpdateScoreText(int score)
    {
        _text.SetText(score.ToString());
    }
}