using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float movementSpeed = 5f;

    void Update()
    {
        var horizontal = Input.GetAxis("Horizontal") * movementSpeed;
        var rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(horizontal, rb.velocity.y);
    }
}