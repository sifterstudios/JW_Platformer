using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    public bool PlayerInside;
    Vector3 _initialPosition;
    readonly HashSet<Player> _playersInTrigger = new HashSet<Player>();

    void Start()
    {
        _initialPosition = transform.position;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player == null)
            return;

        _playersInTrigger.Add(player);

        PlayerInside = true;
        if (_playersInTrigger.Count == 1)
            StartCoroutine(WiggleAndFall());
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player == null)
            return;
        _playersInTrigger.Remove(player);
        if (_playersInTrigger.Count == 0)
        {
            PlayerInside = false;
            StopCoroutine(WiggleAndFall());
        }
    }

    IEnumerator WiggleAndFall()
    {
        Debug.Log("Waiting to wiggle");
        yield return new WaitForSeconds(0.25f);
        Debug.Log("Wiggling");
        float wiggleTimer = 0;

        while (wiggleTimer < 1f)
        {
            var randomX = Random.Range(-0.05f, 0.05f);
            var randomY = Random.Range(-0.05f, 0.05f);
            transform.position = _initialPosition + new Vector3(randomX, randomY);
            var randomDelay = Random.Range(0.005f, 0.01f);
            yield return null;
            wiggleTimer += randomDelay;
        }

        Debug.Log("Falling");
        yield return new WaitForSeconds(3f);
    }
}