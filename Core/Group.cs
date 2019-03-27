using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CollectorNode<TEntity> where TEntity : Entity
{
    public Collector<TEntity>[] collectors;
    public bool[] hasCollector;

    public CollectorNode(Collector<TEntity>[] collectors)
    {
        this.collectors = collectors;
        hasCollector = new bool[collectors.Length];
    }
}

public class Collector<TEntity> : IEnumerable<TEntity> where TEntity : Entity 
{
    public HashSet<TEntity> Values = new HashSet<TEntity>(EntityEqualityComparer<TEntity>.comparer);

    public int Count { get { return Values.Count; } }

    public bool IsNotEmpty
    {
        get
        {
            return Values.Count > 0;
        }
    }

    //Refactor with resizable array
    public List<int> FieldIDs = new List<int>();

    public bool Add(TEntity entity)
    {
        return Values.Add(entity);
    }

    public void Remove(TEntity entity)
    {
        Values.Remove(entity);
    }

    public void Clear()
    {
        Values.Clear();
    }

    public IEnumerator<TEntity> GetEnumerator()
    {
        return Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}

[System.Serializable]
public partial class Group<TEntity> : IEnumerable<TEntity> where TEntity : Entity
{
    public string Name = typeof(TEntity).ToString() + "-Group";

    public int Count { get { return Entities.Count; } }

    /// <summary>
    /// Don't use every frame, it's slow enough
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public TEntity GetEntityAt(int index)
    {
        return Entities.ElementAt(index);
    }

    HashSet<TEntity> Entities = new HashSet<TEntity>(EntityEqualityComparer<TEntity>.comparer);
    TEntity[] existingEntities;

    public Dictionary<int, CollectorNode<TEntity>> CollectorNodes;

    public bool[] HasCollectorNode = new bool[500];

    protected void InternalOnAdd(Collector<TEntity> collector)
    {
        if (!onAddCreated)
        {
            onAdd = collector;
            onAddCreated = true;
        }
    }

    protected void InternalOnRemove(Collector<TEntity> collector)
    {
        if (!onRemoveCreated)
        {
            onRemove = collector;
            onRemoveCreated = true;
        }
    }

    protected void InternalOnDestroy(Collector<TEntity> collector)
    {
        if (!onDestroyCreated)
        {
            onDestroy = collector;
            onDestroyCreated = true;
        }
    }

    Collector<TEntity> onAdd; bool onAddCreated;
    Collector<TEntity> onRemove; bool onRemoveCreated;
    Collector<TEntity> onDestroy; bool onDestroyCreated;

    protected virtual void RemoveFromCollectors(TEntity entity)
    {
        foreach (var collectorNode in CollectorNodes)
        {
            foreach (var collector in collectorNode.Value.collectors)
            {
                collector.Values.Remove(entity);
            }
        }
    }

    public bool IsNotEmpty
    {
        get
        {
            return Entities.Count > 0;
        }
    }

    public TEntity SingleEntity
    {
        get
        {
            if (Entities.Count != 1)
            {
                UnityEngine.Debug.LogException(new Exception("entities count is not equal to one. Count  is :" + Entities.Count));
            }
            return Entities.ToList().FirstOrDefault();
        }
    }

    public TEntity[] GetEntities()
    {
        if (existingEntities == null)
        {
            existingEntities = new TEntity[Entities.Count];
            Entities.CopyTo(existingEntities);
        }

        return existingEntities;
    }

    public bool Contains(TEntity entity)
    {
        return Entities.Contains(entity);
    }

    public bool Add(TEntity entity)
    {
        var mayAdd = Entities.Add(entity);
        if (mayAdd)
        {
            existingEntities = null;
            if (onAddCreated)
                onAdd.Add(entity);
        }

#if NANOECS_VERBOSE_DEBUG
        var res = mayAdd ? " succesfully added to the " + Name : " not added to the " + Name + ". It already contains this";
        UnityEngine.Debug.Log("entity" + entity.ID + res); 
#endif
        return mayAdd;
    }

    public bool Remove(TEntity entity)
    {
        existingEntities = null;

        if (onAddCreated)
            onAdd.Remove(entity);

        if (!entity.IsReserved)
        {
            if (onRemoveCreated)
                onRemove.Add(entity);
        }
        else
        {
            if (onRemoveCreated)
                onRemove.Remove(entity);
        }

        var mayRemove = Entities.Remove(entity);
        RemoveFromCollectors(entity);

#if NANOECS_VERBOSE_DEBUG
        var res = mayRemove ? " succesfully removed from the " + Name : " not removed from the " + Name + ". It doesn't contain this";
        UnityEngine.Debug.Log("entity" + entity.ID + res); 
#endif
        return mayRemove;
    }


    public void InternalTryAddToOnDestroy(TEntity entity)
    {
        if (!onDestroyCreated) return;

        onDestroy.Add(entity);
    }

    List<int> withTypes;
    List<int> withoutTypes;
    List<int> anyofTypes;
   
    public List<int> WithTypes
    {
        get
        {
            if (withTypes == null) withTypes = new List<int>(); 
            return withTypes;
        }
    }

    public List<int> WithoutTypes
    {
        get
        {
            if (withoutTypes == null) withoutTypes = new List<int>();
            return withoutTypes;
        }
    }

    public List<int> AnyofTypes
    {
        get
        {
            if (anyofTypes == null) anyofTypes = new List<int>();
            return anyofTypes;
        }
    }

    public bool IsMatching(Entity entity)
    {
        if (withTypes != null)
        {
            foreach (var type in WithTypes)
            {
                if (!entity.Has(type)) return false;
            }
        }

        if (withoutTypes != null)
        {
            foreach (var type in WithoutTypes)
            {
                if (entity.Has(type)) return false;
            }
        }

        if (anyofTypes != null)
        {
            foreach (var type in AnyofTypes)
            {
                if (entity.Has(type)) return true;
            }
            return false;
        }

        return true;

    }

    protected WithBuilder<TEntity> withBuilder;
    protected WithoutBuilder<TEntity> withoutBuilder;
    protected AnyofBuilder<TEntity> anyofBuilder;

    public Group()
    {
        CollectorNodes = new Dictionary<int, CollectorNode<TEntity>>(new IntEqualityComparer()); 
    }

    public IEnumerator<TEntity> GetEnumerator()
    {
        return Entities.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}

public class WithBuilder<TEntity> where TEntity : Entity 
{
    protected Group<TEntity> group;

    public WithBuilder(Group<TEntity> group)
    {
        this.group = group;
    }
}

public class WithoutBuilder<TEntity> where TEntity : Entity
{
    protected Group<TEntity> group;

    public WithoutBuilder(Group<TEntity> group)
    {
        this.group = group;
    }
}

public class AnyofBuilder<TEntity> where TEntity : Entity
{
    protected Group<TEntity> group;

    public AnyofBuilder(Group<TEntity> group)
    {
        this.group = group;
    }
}

