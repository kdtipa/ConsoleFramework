using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleFramework.Helpers;

/// <summary>
/// This list type gives you an enumerator that includes the index within the list 
/// along with each item.  This way, when you searching for an item in the list, 
/// you can access it again at the given index without having to do a separate search 
/// for the index.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ExposedIndexList<T> : IEnumerable<ExposedIndexItem<T>> where T : IEquatable<T>
{
    public ExposedIndexList() { }

    public ExposedIndexList(params T[] initialItems)
    {
        _items = initialItems.ToList();
    }

    private List<T> _items = new List<T>();

    public List<T> Items { get { return _items; } set { _items = value; } }

    public IEnumerator<ExposedIndexItem<T>> GetEnumerator()
    {
        for (int i = 0; i < _items.Count; i++)
        {
            yield return new ExposedIndexItem<T>(_items[i], i);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public T this[int i]
    {
        get { return _items[i]; }
        set { _items[i] = value; }
    }


    public void Add(T item)
    {
        _items.Add(item); 
    }

    public bool Remove(T item)
    {
        return _items.Remove(item); 
    }

    public void RemoveAt(int index)
    {
        _items.RemoveAt(index); 
    }


    public bool Contains(T item)
    {
        return _items.Contains(item); 
    }

    public int IndexOf(T item)
    {
        return _items.IndexOf(item); 
    }

    public int LastIndexOf(T item)
    {
        return _items.LastIndexOf(item); 
    }




}


public struct ExposedIndexItem<T>
{
    public ExposedIndexItem(T item, int index) 
    { 
        Item = item;
        Index = index;
    }

    public T Item { get; set; }
    public int Index { get; set; }
}
