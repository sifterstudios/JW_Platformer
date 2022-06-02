using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class Collector : MonoBehaviour
{
    [SerializeField] private List<Collectible> _collectibles;
    [SerializeField] private UnityEvent _onCollectionComplete;
    private AudioSource _audioSource;

    private int _countCollected;
    private TMP_Text _remainingText;

    private void Start()
    {
        _remainingText = GetComponentInChildren<TMP_Text>();

        foreach (var collectible in _collectibles) collectible.OnPickedUp += ItemPickedUp;

        var countRemaining = _collectibles.Count - _countCollected;
        _remainingText?.SetText(countRemaining.ToString());
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        foreach (var collectible in _collectibles)
        {
            if (Selection.activeGameObject == gameObject)
                Gizmos.color = Color.red;
            else
                Gizmos.color = Color.gray;
            Gizmos.DrawLine(transform.position, collectible.transform.position);
        }
    }

    private void OnValidate()
    {
        _collectibles = _collectibles.Distinct().ToList();
    }


    public void ItemPickedUp()
    {
        _countCollected++;
        var countRemaining = _collectibles.Count - _countCollected;

        _remainingText?.SetText(countRemaining.ToString());
        if (countRemaining > 0)
            return;
// Ja her tester vi mere!
        _onCollectionComplete.Invoke();
        _audioSource.Play();
    }
}