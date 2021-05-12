using UnityEngine;

public class Collector : MonoBehaviour
{
    [SerializeField] Collectible[] _collectibles;

    void Update()
    {
        foreach (var collectible in _collectibles)
            if (collectible.isActiveAndEnabled)
                return;
        print("Got All Gems");
    }
}