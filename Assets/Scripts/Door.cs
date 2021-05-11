using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] Sprite openMid;
    [SerializeField] Sprite openTop;

    [SerializeField] SpriteRenderer rendererMid;
    [SerializeField] SpriteRenderer rendererTop;


    [ContextMenu("Open Door")]
    void Open()
    {
        rendererMid.sprite = openMid;
        rendererTop.sprite = openTop;
    }

    void Start()
    {
    }

    void Update()
    {
    }
}