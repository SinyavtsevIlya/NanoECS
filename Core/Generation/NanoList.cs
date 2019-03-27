using System.Collections;
using System.Collections.Generic;

public interface ICount
{
    int Count { get; }
}

public class NanoList<T> : IEnumerable<T>, ICount // TODO implement IList
{
    List<T> values = new System.Collections.Generic.List<T>();

    System.Action<ushort> InternalValueChange;

    public List<T> Values { get { return values; } set { values = value; } }

    public System.Action<T> OnItemAdd;
    public System.Action<T> OnItemRemove;
    public System.Action<int> OnItemsCountChange;

    ushort fieldId;

    public T this[int i]
    {
        get { return values[i]; }
        set { values[i] = value; }
    }

    public NanoList(params T[] range)
    {
        AddRange(range);
    }

    public NanoList(T value)
    {
        Add(value);
    }

    public NanoList()
    {
    }

    public void Initialize(System.Action<ushort> onValueChange, ushort fieldId)
    {
        this.fieldId = fieldId;
        InternalValueChange = onValueChange;
    }

    public T First
    {
        get
        {
            return values[0];
        }
        set
        {
            if (values.Count > 0)
                values[0] = value;
            else
                values.Add(value);

            InternalTrigger();
        }
    }

    private void InternalTrigger()
    {
        if (InternalValueChange != null)
        {
            InternalValueChange(fieldId);
        }
    }

    public T Last()
    {
        if (values.Count == 0) return default (T);
        return values[values.Count - 1];
    }

    public bool Contains(T value)
    {
        return values.Contains(value);
    }

    public int Count
    {
        get
        {
            return values.Count;
        }
    }

    public void AddRange(NanoList<T> range)
    {
        AddRange(range.values);
    }

    public void AddRange(IEnumerable<T> range)
    {
        foreach (var item in range)
        {
            Add(item);
        }
        InternalTrigger();
    }

    public void Clear(bool silantly = false)
    {
        if (silantly)
        {
            values.Clear();
            return;
        }

        var pool = new List<T>();
        foreach (var item in values)
        {
            pool.Add(item);
        }

        foreach (var item in pool)
        {
            values.Remove(item);
            if (OnItemRemove != null) OnItemRemove(item);
        }

        InternalTrigger();
        if (OnItemsCountChange != null) OnItemsCountChange(0);
    }

    public void Add(T value, bool silently = false)
    {
        values.Add(value);
        if (silently) return;
        if (OnItemAdd != null) OnItemAdd(value);
        if (OnItemsCountChange != null) OnItemsCountChange(values.Count);
        InternalTrigger();
    }

    public void Remove(T value)
    {
        values.Remove(value);
        if (OnItemRemove != null) OnItemRemove(value);
        if (OnItemsCountChange != null) OnItemsCountChange(values.Count);
        InternalTrigger();
    }

    public IEnumerator<T> GetEnumerator()
    {
        return values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}