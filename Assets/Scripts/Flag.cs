using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Flag : MonoBehaviour
{
    [SerializeField] string sceneName;

    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player == null)
            return;

        var animator = GetComponent<Animator>();
        animator.SetTrigger("Raise");
        StartCoroutine(LoadAfterDelay());
    }

    IEnumerator LoadAfterDelay()
    {
        PlayerPrefs.SetInt(sceneName + "Unlocked", 1);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(sceneName);
    }
}