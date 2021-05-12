using TMPro;
using UnityEngine;

public class Collector : MonoBehaviour
{
    [SerializeField] Collectible[] _collectibles;
    TMP_Text _remainingText;

    void Start()
    {
        _remainingText = GetComponentInChildren<TMP_Text>();
    }

    void Update()
    {
        var countRemaining = 0;
        foreach (var collectible in _collectibles)
            if (collectible.isActiveAndEnabled)
                countRemaining++;
        _remainingText?.SetText(countRemaining.ToString());
        if (countRemaining > 0)
            return;

        print("Got All Gems");
    }
}