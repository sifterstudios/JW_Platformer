using UnityEngine;

public class Mushroom : MonoBehaviour
{
    [SerializeField] float bounceVelocity = 10f;

    void OnCollisionEnter2D(Collision2D other)
    {
        var player = other.collider.GetComponent<Player>();
        if (player != null)
        {
            var rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = new Vector2(rb.velocity.x, bounceVelocity);
            }
        }
    }
}