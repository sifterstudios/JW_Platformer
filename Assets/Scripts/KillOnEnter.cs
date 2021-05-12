using UnityEngine;

public class KillOnEnter : MonoBehaviour
{
    void OnParticleCollision(GameObject other)
    {
        var player = other.GetComponent<Player>();
        if (player != null) player.ResetToStart();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player != null) player.ResetToStart();
    }
}