using UnityEngine;

public static class EntityLinkingLookup
{
    const int MaxEntitiesCount = 2048;

    public static GameObject[] EntityLinks = new GameObject[MaxEntitiesCount];
    public static bool[] HasEntityLink = new bool[MaxEntitiesCount];
}