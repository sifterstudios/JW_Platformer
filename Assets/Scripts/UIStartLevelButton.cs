using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIStartLevelButton : MonoBehaviour
{
    [SerializeField] string _levelName;
    public string LevelName => _levelName;

    void OnValidate()
    {
        GetComponentInChildren<TMP_Text>()?.SetText(_levelName);
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene(_levelName);
    }
}