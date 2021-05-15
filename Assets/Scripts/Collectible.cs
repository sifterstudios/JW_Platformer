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
        GetComponent<AudioSource>().Play();
    }

    public event Action OnPickedUp;
}