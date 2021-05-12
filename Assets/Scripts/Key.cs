using UnityEngine;

public class Key : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player != null)
        {
            transform.SetParent(player.transform);
            transform.localPosition = Vector3.up;
        }
    }
}