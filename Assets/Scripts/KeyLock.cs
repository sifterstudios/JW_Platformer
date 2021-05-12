using UnityEngine;
using UnityEngine.Events;

public class KeyLock : MonoBehaviour
{
    [SerializeField] UnityEvent _onUnlocked;

    public void Unlock()
    {
        print("Unlocked");
        _onUnlocked.Invoke();
    }
}