using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{
    [SerializeField] Transform leftSensor;
    [SerializeField] Transform rightSensor;
    Rigidbody2D _rb;
    float _direction = -1;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        _rb.velocity = new Vector2(_direction, _rb.velocity.y);

        if (_direction < 0)
        {
            Debug.DrawRay(leftSensor.position, Vector2.down * 0.1f, Color.red);

            var result = Physics2D.Raycast(leftSensor.position, Vector2.down, 0.1f);
            if (result.collider == null)
            {
                TurnAround();
            }
        }
        else
        {
            Debug.DrawRay(rightSensor.position, Vector2.down * 0.1f, Color.red);

            var result = Physics2D.Raycast(rightSensor.position, Vector2.down, 0.1f);
            if (result.collider == null)
            {
                TurnAround();
            }
        }
    }

    void TurnAround()
    {
        _direction *= -1;
        var spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.flipX = _direction > 0;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        var player = other.collider.GetComponent<Player>();
        if (player == null)
            return;

        player.ResetToStart();
    }
}