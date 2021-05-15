using System;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player == null)
            return;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;

        OnPickedUp?.Invoke();
        GetComponent<AudioSource>().Play();
    }

    public event Action OnPickedUp;
}