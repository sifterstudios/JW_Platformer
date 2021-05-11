using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    public bool PlayerInside;
    readonly HashSet<Player> _playersInTrigger = new HashSet<Player>();

    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player == null)
            return;

        _playersInTrigger.Add(player);

        PlayerInside = true;

        StartCoroutine(WiggleAndFall());
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player == null)
            return;
        _playersInTrigger.Remove(player);
        if (_playersInTrigger.Count == 0) PlayerInside = false;
    }

    IEnumerator WiggleAndFall()
    {
        Debug.Log("Waiting to wiggle");
        yield return new WaitForSeconds(0.25f);
        Debug.Log("Wiggling");
        yield return new WaitForSeconds(1f);
        Debug.Log("Falling");
        yield return new WaitForSeconds(3f);
    }
}