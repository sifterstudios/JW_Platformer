using System;
using UnityEngine;

public class Fly : MonoBehaviour
{
    Vector2 _startPosition;
    [SerializeField] Vector2 direction = Vector2.up;
    [SerializeField] float maxDistance = 2;
    [SerializeField] float speed = 2;

    void Start()
    {
        _startPosition = transform.position;
    }

    void Update()
    {
        transform.Translate(direction.normalized * Time.deltaTime * speed);
        var distance = Vector2.Distance(_startPosition, transform.position);
        if (distance >= maxDistance)
        {
            transform.position = _startPosition + (direction.normalized * maxDistance);
            direction *= -1;
        }
    }


}