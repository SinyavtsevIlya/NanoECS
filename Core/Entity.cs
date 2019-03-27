using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[System.Serializable]
public partial class Entity
{
    public int ID { get; set; }

    public bool IsReserved;

    public ComponentLink[] ComponentLinks = new ComponentLink[8];
    public int ComponentsCount;

    public int[] GroupsIDs = new int[8];
    public int GroupsCount;

    /// <summary>
    /// called before removing all components, and before removing from groups
    /// </summary>
    public Action OnDestroy;

    Action<int, int, int> OnComponentValueChange;
    Action<int> OnComponentAdd;
    Action<int> OnComponentRemove;

#if UNITY_EDITOR && NANOECS_DEBUG
    public List<ComponentObserver> ComponentObservers = new List<ComponentObserver>();
    public Dictionary<string, int> componentsLookup = new Dictionary<string, int>();
#endif

    Storage[] storages;

    public void InternalInitialize(
        int id,
        Storage[] storages,
        Action<int, int, int> onComponentValueChange,
        Action<int> onComponentAdd,
        Action<int> onComponentRemove)
    {
        ID = id;
        this.storages = storages;
        OnComponentAdd = onComponentAdd;
        OnComponentRemove = onComponentRemove;
        OnComponentValueChange = onComponentValueChange;
    }

    public void InternalReset()
    {
    }

    public T Get<T>(int componentIndex) where T : ComponentEcs, new()
    {
        return (T) storages[componentIndex].GetAtIndex(ID);
    } 

    public T Add<T>(int componentIndex) where T : ComponentEcs, new()
    {
        if (IsReserved) throw new Exception(string.Format("Unable to add {0} to reserved entity", componentIndex));

        if (Has(componentIndex))
        {
            Debug.LogException(new Exception(string.Format("Component {0} is Already Added to the Entity (id: {1}) ", componentIndex, ID)));
            return null;
        }

        var component = (T) storages[componentIndex].ActivateAtIndex(ID);

        component._InternalOnValueChange = (fieldID) => { OnComponentValueChange(ID, fieldID, componentIndex); };

        ComponentLinks[ComponentsCount++] = new ComponentLink(componentIndex);

        OnComponentAdd(ID);

        if (ComponentLinks.Length == ComponentsCount)
        {
            Array.Resize(ref ComponentLinks, ComponentsCount << 1);
        }

#if UNITY_EDITOR && NANOECS_DEBUG
        ComponentObservers.Add(new ComponentObserver() {Component = component, IsFoldout = true});
#endif

        return component;
    }

    public void RemoveComponentOfIndex(int componentIndex)
    {
#if UNITY_EDITOR && NANOECS_DEBUG
        var c = ComponentObservers.Find(x => componentsLookup[x.Component.GetType().ToString()] == componentIndex);
        if (c != null)
        {
            ComponentObservers.Remove(c);
        }
#endif
        // TODO : Check maybe need to null component._InternalOnValueChange

        storages[componentIndex].DeactivateAtIndex(ID);

        int index = 0;
        for (int i = 0; i < ComponentsCount; i++)
        {
            if (ComponentLinks[i].ComponentIndex == componentIndex) index = i;
        }

        ComponentsCount--;
        Array.Copy(ComponentLinks, index + 1, ComponentLinks, index, ComponentsCount - index);

        OnComponentRemove(ID);
    }

    public void RemoveAllComponents()
    {
        var indexes = ComponentLinks.Take(ComponentsCount).Select(x => x.ComponentIndex).ToArray();

        foreach (var index in indexes)
        {
            RemoveComponentOfIndex(index);
        }
    }

    public bool Has(int componentIndex)
    {
        return storages[componentIndex].HasComponentAtIndex(ID);
    }

    // accessing by component index

    //public T Get<T>(int componentIndex) where T : ComponentEcs, new()
    //{
    //    return (T)storages.Values[componentIndex].Get(ID);
    //}

}

[System.Serializable]
public struct ComponentLink
{
    public int ComponentIndex;

    public ComponentLink(int ComponentIndex)
    {
        this.ComponentIndex = ComponentIndex;
    }
}

public class EntityEqualityComparer<TEntity> : IEqualityComparer<TEntity> where TEntity :  Entity
{

    public static readonly IEqualityComparer<TEntity> comparer = new EntityEqualityComparer<TEntity>();

    public bool Equals(TEntity x, TEntity y)
    {
        return x == y;
    }

    public int GetHashCode(TEntity obj)
    {
        return obj.ID;
    }
}


#if UNITY_EDITOR && NANOECS_DEBUG
public class ComponentObserver
{
    public ComponentEcs Component;
    public bool IsFoldout;
} 
#endif