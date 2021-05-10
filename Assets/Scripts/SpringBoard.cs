using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringBoard : MonoBehaviour
{
    [SerializeField] float bounceVelocity = 10f;
    [SerializeField] Sprite downSprite;

    SpriteRenderer _spriteRenderer;
    Sprite _upSprite;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _upSprite = _spriteRenderer.sprite;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        var player = other.collider.GetComponent<Player>();
        if (player != null)
        {
            var rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = new Vector2(rb.velocity.x, bounceVelocity);
                _spriteRenderer.sprite = downSprite;
            }
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        var player = other.collider.GetComponent<Player>();
        if (player != null)
        {
            _spriteRenderer.sprite = _upSprite;
        }
    }
}