using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] float fallSpeed;

    public bool PlayerInside;
    readonly HashSet<Player> _playersInTrigger = new HashSet<Player>();
    bool _falling;

    Vector3 _initialPosition;

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
        if (_falling)
            return;
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
            yield return new WaitForSeconds(randomDelay);
            wiggleTimer += randomDelay;
        }

        Debug.Log("Falling");
        _falling = true;
        foreach (var col in GetComponents<Collider2D>()) col.enabled = false;

        float fallTimer = 0;
        while (fallTimer < 3f)
        {
            transform.position += Vector3.down * Time.deltaTime * fallSpeed;
            fallTimer += Time.deltaTime;
            Debug.Log(fallTimer);
            yield return null;
        }

        Destroy(gameObject);
    }
}