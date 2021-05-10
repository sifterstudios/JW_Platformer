using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PushButtonSwitch : MonoBehaviour
{
    [SerializeField] Sprite _downSprite;
    [SerializeField] UnityEvent onEnter;

    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player == null)
            return;

        var spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = _downSprite;

        onEnter?.Invoke();
    }
}