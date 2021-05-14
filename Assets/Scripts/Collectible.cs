using System;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player == null)
            return;
        gameObject.SetActive(false);

        OnPickedUp?.Invoke();
    }

    public event Action OnPickedUp;
}