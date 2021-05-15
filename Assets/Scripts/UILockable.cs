using UnityEngine;

public class UILockable : MonoBehaviour
{
    void OnEnable()
    {
        var startButton = GetComponent<UIStartLevelButton>();
        var key = startButton.LevelName + "Unlocked";
        var unlocked = PlayerPrefs.GetInt(key, 0);

        if (unlocked == 0)
            gameObject.SetActive(false);
    }
}