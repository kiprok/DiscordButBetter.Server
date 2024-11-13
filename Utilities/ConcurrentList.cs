namespace DiscordButBetter.Server.Utilities;

public class ConcurrentList<T>
{
    private readonly List<T> _list = new();
    private readonly Lock _lock = new();


    public ConcurrentList(params T[] items)
    {
        _list.AddRange(items);
    }

    public int Count
    {
        get
        {
            lock (_lock)
            {
                return _list.Count;
            }
        }
    }

    public void Add(T item)
    {
        lock (_lock)
        {
            _list.Add(item);
        }
    }

    public void Remove(T item)
    {
        lock (_lock)
        {
            _list.Remove(item);
        }
    }

    public List<T> ToList()
    {
        lock (_lock)
        {
            return _list.ToList();
        }
    }

    public void ForEach(Action<T> action)
    {
        lock (_lock)
        {
            foreach (var item in _list) action(item);
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            _list.Clear();
        }
    }

    public bool Contains(T item)
    {
        lock (_lock)
        {
            return _list.Contains(item);
        }
    }
}