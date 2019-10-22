#if UNITY_EDITOR && NANOECS_DEBUG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityObserver : MonoBehaviour
{
    public Entity Entity;
    public System.Action<Entity> OnEntityDestroy;
    public string CurrentComponentName = string.Empty;
    public bool DisplayDropDown;
    public Dictionary<string, int> ComponentsLookup;
    
    public void Initialize(Entity entity, System.Action<Entity> onEntityDestroy, Dictionary<string, int> componentsLookup)
    {
        Entity = entity;
        OnEntityDestroy = onEntityDestroy;
        ComponentsLookup = componentsLookup;
    }
}

#endif