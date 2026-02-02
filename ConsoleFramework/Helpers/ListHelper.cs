using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleFramework.Helpers;

public static class ListHelper
{
    /// <summary>
    /// This method will compare the lists given to see if they represent the 
    /// same list of items regardless of the order they are in.  1, 1, 2, 3 
    /// is the same as 2, 1, 3, 1; but not the same as 1, 2, 2, 3.  This is 
    /// intended to be a value comparison, but relies on the implementation 
    /// of Equals for T.
    /// </summary>
    /// <typeparam name="T">Must be an IEquatable type</typeparam>
    /// <param name="thisList">the base list we're running this method on</param>
    /// <param name="compareList">the list we want to compare to</param>
    /// <returns>true if both lists have the same items, regardless of order</returns>
    public static bool EqualByValue<T>(this List<T> thisList, List<T> compareList) where T : IEquatable<T>
    {
        // quick exit if the lists aren't the same length
        if (thisList.Count != compareList.Count) { return false; }

        // build a list of this list's items to be sure we don't mess up the original list
        var workList = new List<T>();
        foreach (var item in thisList)
        {
            workList.Add(item);
        }

        // now remove items from the work list that equal the compare list.
        // if the compare list has an item that doesn't exist in the work list, 
        // we return false.
        foreach (var item in compareList)
        {
            if (!workList.Remove(item)) { return false; }
        }

        // if the worklist is now empty, it means we had a match to remove for every item
        return workList.Count == 0;
    }

    public static bool ContainsAll<T>(this List<T> thisList, params T[] findItems) where T : IEquatable<T>
    {
        // this is actually a bad operation.  Contains is already bad for being on the Order of N since it's 
        // not a sorted list for certain.  Adding in a list of things to look for means it's likely going to 
        // be an order of N times M operation which can approach N squared if the search item list is close 
        // to as long as the haystack list.  And there doesn't seem to be an efficient way to do it.

        if (findItems.Length == 0) { return false; }

        foreach (var item in findItems)
        {
            if (!thisList.Contains(item)) { return false; }
        }
        return true;
    }

    public static IEnumerable<T> GetOverlapValues<T>(this List<T> thisList, List<T> compareList) where T : IEquatable<T>
    {
        foreach (var item in thisList)
        {
            if (compareList.Contains(item)) { yield return item; }
        }
    }


    /// <summary>
    /// If the index desired exists, will append the text to the string in that index.  If 
    /// the index desired is higher than the highest existing index, it will add a new string 
    /// to the end of the list.  If the index is negative, it will try to append to the index 
    /// that far from the end.  If the negative index is more than the number of items, it 
    /// will insert a new string at index 0.
    /// </summary>
    /// <param name="text">The text you want to add</param>
    /// <param name="index">
    /// Pass an existing index to append to the string at that index.  Pass a number higher 
    /// than an existing index to add the text as a new item in the list.  Pass a negative 
    /// number to move that far back from the item count (-1 for the last item) and append 
    /// to that.  Pass a negative number that backtracks past the beginning of the list, 
    /// and it will insert the text as a new item at the beginning of the list.
    /// </param>
    /// <param name="wasAppended">
    /// This out parameter tells you if the method found an index to append to.  If not, 
    /// you know that the value was inserted at the index that gets returned.
    /// </param>
    /// <returns>
    /// The method returns the index where it put the text.
    /// </returns>
    public static int AppendOrCreate(this List<string> thisList, string text, int index, out bool wasAppended)
    {
        // first we figure out what the index should be
        var listLen = thisList.Count;

        // if it's a fresh list, the index given doesn't matter, and we have to add a new item
        if (listLen == 0)
        {
            thisList.Add(text);
            wasAppended = false;
            return 0;
        }

        // this means we have an append to an existing item
        if (index >= 0 && index < listLen)
        {
            thisList[index] += text;
            wasAppended = true;
            return index;
        }
        
        // this is the case where the index given refers to a new index, so we add the text as a new string
        if (index >= listLen)
        {
            thisList.Add(text);
            wasAppended = false;
            return thisList.Count - 1;
        }

        // at this point, the index given must be negative, so we will try to backtrack
        var calcIndex = listLen + index;

        // this would be if we went below zero, where we want to insert at zero.
        if (calcIndex < 0)
        {
            thisList.Insert(0, text);
            wasAppended = false;
            return 0;
        }

        // this is the last case, and should catch everything left.  We're being paranoid though.
        if (calcIndex >= 0 && calcIndex < listLen)
        {
            thisList[calcIndex] += text;
            wasAppended = true;
            return calcIndex;
        }

        wasAppended = false;
        return -1; // an error value since you can't put anything at index negative one.
    }

    /// <summary>
    /// Takes some text you'd like to add to an item in the list and tries to concatenate 
    /// the existing string with the string you provided.  If the list is empty, it ignores 
    /// the chosen index and just adds the first item as the text you provided.  If you do 
    /// not provide an index, it chooses the last item in the list by default.
    /// </summary>
    /// <param name="text">The text you want to add.</param>
    /// <param name="index">
    /// The index you'd like to add your text to.  If left null or is an index that doesn't 
    /// exist in the list, it will default to the last item in the list.
    /// </param>
    /// <returns>Returns the index that was appended to.</returns>
    public static int Append(this List<string> thisList, string text, int? index = null)
    {
        int itemCount = thisList.Count;

        if (itemCount == 0)
        {
            thisList.Add(text);
            return 0;
        }

        int targetIndex = itemCount - 1; // last index by default
        if (index is not null && index >= 0 && index < targetIndex) { targetIndex = index.Value; }

        thisList[targetIndex] += text;
        return targetIndex;
    }

    public static bool Remove(this List<string> thisList, string text, bool? ignoreCase = null)
    {
        int listLen = thisList.Count;
        StringComparison cmpType = ignoreCase ?? false ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        for (int i = 0; i < listLen; i++)
        {
            if (string.Equals(thisList[i], text, cmpType))
            {
                thisList.RemoveAt(i);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// This method allows you to get a new list of items pulled from this List, 
    /// but which do not exist in any of the other lists you provide.
    /// </summary>
    /// <typeparam name="T">Type must extend IEquatable so Equals can be reliably called.</typeparam>
    /// <param name="otherLists">The collection of lists with values you want to avoid.</param>
    /// <returns>A collection of items that do NOT exist in the avoidance collection.</returns>
    public static IEnumerable<T> GetItemsOnlyInThisList<T>(this List<T> thisList, params List<T>[] otherLists) where T : IEquatable<T>
    {
        bool foundItem;
        foreach (T srcItem in thisList)
        {
            foundItem = false;
            foreach (var compareList in otherLists)
            {
                foreach (T compareItem in compareList)
                {
                    if (srcItem.Equals(compareItem))
                    {
                        foundItem = true;
                        break;
                    }
                }
                if (foundItem) { break; }
            }

            if (!foundItem) { yield return srcItem; }
        }
    }

    /// <summary>
    /// This method returns only the items which appear exactly once in the list.  It has to 
    /// do order of N operations to figure it out, so only run this once if you can help it.
    /// </summary>
    public static IEnumerable<T> GetUniqueItems<T>(this List<T> thisList) where T : IEquatable<T>
    {
        var inventory = new Dictionary<T, int>();
        foreach (T item in thisList)
        {
            if (inventory.ContainsKey(item))
            {
                inventory[item]++;
            }
            else
            {
                inventory.Add(item, 1);
            }
        }

        foreach (var inv in inventory)
        {
            if (inv.Value == 1) { yield return inv.Key; }
        }
    }

    /// <summary>
    /// This method gets you only the items that appear in all the list provided.  If no compare 
    /// lists are given, it returns nothing, since there is no overlap.  Be careful for performance 
    /// dependent processes since this has to loop through the original list once, but each of the 
    /// other lists once per item in the original list... essentially an order of N squared operation.
    /// </summary>
    /// <param name="otherLists">The lists to find overlap with</param>
    public static IEnumerable<T> GetOverlap<T>(this List<T> thisList, params List<T>[] otherLists) where T : IEquatable<T>
    {
        if (otherLists.Length == 0) { yield break; }

        foreach (var checkItem in thisList)
        {
            bool otherHasIt = true;
            foreach (var otherList in otherLists)
            {
                if (!otherList.Contains(checkItem))
                {
                    otherHasIt = false; 
                    break;
                }
            }

            if (otherHasIt) { yield return checkItem; }
        }
    }

    public static IEnumerable<T> ReturnRandomizedList<T>(this List<T> thisList)
    {
        if (thisList.Count == 0) { yield break; }
        if (thisList.Count == 1) { yield return thisList[0]; }

        int listCount = thisList.Count;
        List<int> indexes = new();
        for (int i = 0; i < listCount; i++) { indexes.Add(i); }
        var rng = new Random();

        while (indexes.Count > 0)
        {
            int nextIndex = rng.Next(0, indexes.Count);
            yield return thisList[nextIndex];
            indexes.Remove(nextIndex);
        }
    }


    public static void RandomizeList<T>(this List<T> thisList, int? RandomizationPasses = null)
    {
        int rps = RandomizationPasses ?? 3;
        if (rps <= 0) { rps = 1; } else if (rps > 5) { rps = 5; }

        var rng = new Random();
        int listCount = thisList.Count;

        for (int p = 1; p <= rps; p++)
        {
            // this loop controls how many times we'll go over the complete list

            for (int i = 0; i < listCount; i++)
            {
                // this loop is the one that steps over the list and shuffles the items around
                int swapIndex = rng.Next(0, listCount);
                if (swapIndex != i)
                {
                    (thisList[i], thisList[swapIndex]) = (thisList[swapIndex], thisList[i]);
                }
            }
        }
    }


    public static int IndexOf<T>(this List<T> sourceList, Func<T, bool> customMatch) where T : IEquatable<T>
    {
        int itemCount = sourceList.Count;

        for (int i = 0; i < itemCount; i++)
        {
            if (customMatch(sourceList[i])) { return i; }
        }

        return -1;
    }


    public static List<int> IndexesOf<T>(this List<T> sourceList, T searchFor) where T : IEquatable<T>
    {
        List<int> found = new();

        for (int i = 0; i < sourceList.Count; i++)
        {
            if (sourceList[i].Equals(searchFor)) { found.Add(i); }
        }

        return found;
    }



}


/// <summary>
/// This is a bit of a niche use case where you might want a list of 
/// strings to have some of its items concatenated into one item.  The 
/// original use case that motivated this class was for the idea of an 
/// array of arguments that had something like [argname], [=], ["value 
/// with spaces in it"] to make it easy to smoosh them into one argument 
/// with the value [argname="value with spaces in it"].
/// </summary>
public class ConcatList
{
    public ConcatList() { }

    public ConcatList(List<string> sourceList)
    {
        _sourceList = sourceList;
    }

    public ConcatList(params string[] sourceList)
    {
        _sourceList.AddRange(sourceList);
    }

    private List<string> _sourceList = new();

    public IEnumerable<string> SourceItems
    {
        get
        {
            foreach (var item in _sourceList) { yield return item; }
        }
    }

    public int Count { get { return _sourceList.Count; } }

    public string this[int i]
    {
        get { return _sourceList[i]; }
        set { _sourceList[i] = value; }
    }

    public void Add(string item) { _sourceList.Add(item); }

    public bool Remove(string item, bool? ignoreCase = null)
    {
        StringComparison cmp = ignoreCase ?? false ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        int count = _sourceList.Count;

        for (int i = 0; i < count; i++)
        {
            if (_sourceList[i].Equals(item, cmp))
            {
                _sourceList.RemoveAt(i);
                return true;
            }
        }

        return false;
    }

    public bool Contains(string item, bool? ignoreCase = null)
    {
        StringComparison cmp = ignoreCase ?? false ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        foreach (string srcItem in _sourceList)
        {
            if (srcItem.Equals(item, cmp)) { return true; }
        }
        return false;
    }

    public int IndexOf(string item, bool? ignoreCase = null)
    {
        StringComparison cmp = ignoreCase ?? false ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        int count = _sourceList.Count;

        for (int i = 0; i < count; i++)
        {
            if (_sourceList[i].Equals(item, cmp)) { return i; }
        }

        return -1;
    }

    public bool TryFind(string item, out List<int> matches, bool? ignoreCase = null)
    {
        matches = new List<int>();

        StringComparison cmp = ignoreCase ?? false ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        int count = _sourceList.Count;

        for (int i = 0; i < count; i++)
        {
            if (_sourceList[i].Equals(item, cmp))
            {
                matches.Add(i);
            }
        }

        return matches.Count > 0;
    }

    /// <summary>
    /// This method lets you pick two indexes and combine them into the first index, 
    /// and remove the second index.  This is most likely useful when you're trying 
    /// to organize a set of strings like arguments from a command line that might 
    /// need to go together despite spaces.
    /// </summary>
    /// <param name="baseIndex">The first part of the combined result and the target index</param>
    /// <param name="takeIndex">The second part of the combined result and the index that will be removed</param>
    /// <param name="finalIndex">In case the taken index is before the base index, this let's you know what the final index is</param>
    /// <param name="conjunction">An optional character to put between the strings like if you want a space there</param>
    /// <returns>false if the indexes provided don't exist, true otherwise</returns>
    public bool TryCombine(int baseIndex, int takeIndex, [NotNullWhen(true)] out int? finalIndex, char? conjunction = null)
    {
        finalIndex = -1;

        // make sure the parameters are valid
        int count = _sourceList.Count;
        if (baseIndex < 0 || baseIndex >= count 
         || takeIndex < 0 || takeIndex >= count 
         || baseIndex == takeIndex)
        { return false; }

        // now build the concatenated string
        var worker = new StringBuilder();
        worker.Append(_sourceList[baseIndex]);
        if (conjunction is not null) { worker.Append(conjunction); }
        worker.Append(_sourceList[takeIndex]);

        // overwrite that one
        _sourceList[baseIndex] = worker.ToString();

        // and remove the other one
        _sourceList.RemoveAt(takeIndex);

        // now figure out the final index
        if (takeIndex < baseIndex) { finalIndex = baseIndex - 1; }
        else { finalIndex = baseIndex; }

        return true;
    }

    /// <summary>
    /// This is the real reason for this class.  If you have a list of strings and 
    /// got them to the point where [argName], [=], ["some test message"] are each 
    /// items in that list... and you want just one item that looks like: 
    /// [argName="some test message"], this method will adjust the list to no 
    /// longer have the separate items and to have just the one item that is the 
    /// combined parts.  It does require that the character it looks for is at the 
    /// end, on its own, or at the start of an item.  If it's in the middle of a 
    /// string, it won't do anything to it.  But be careful too, because you could 
    /// end up with something like [str1=str2=str3] if your source happens to be 
    /// organized that way.
    /// </summary>
    /// <param name="blackHoleChar">
    /// This is the character that draws nearby strings into it.
    /// </param>
    public void CollapseBy(char blackHoleChar)
    {
        int count = _sourceList.Count;
        List<string> resultList = new() { "" }; // make sure we have index 0;
        bool connectToPrevious = false;

        for (int i = 0; i < count; i++)
        {
            string current = _sourceList[i];
            int currentLen = current.Length;

            if (currentLen == 1 && current[0] == blackHoleChar)
            {
                // in this situation we need to tack this onto the previous value and tell the next value
                resultList[resultList.Count - 1] += current;
                connectToPrevious = true;
            }
            else if (current.StartsWith(blackHoleChar))
            {
                // in this case we tack this on to the end and make sure we don't attach the next one
                resultList[resultList.Count - 1] += current;
                connectToPrevious = false;
            }
            else if (current.EndsWith(blackHoleChar))
            {
                // in this case we know this string ends with the blackhole character so the next one needs to be connected
                if (connectToPrevious) { resultList[resultList.Count - 1] += current; }
                else { resultList.Add(current); }
                connectToPrevious = true;
            }
            else if (connectToPrevious)
            {
                // in this case we connect and mark it so the next one doesn't connect
                resultList[resultList.Count - 1] += current;
                connectToPrevious = false;
            }
            else
            {
                resultList.Add(current);
            }
        }

        if (resultList.Count != _sourceList.Count) { _sourceList = resultList; }
    }

}


public class UniqueList<T> : IEnumerable<T> where T : IEquatable<T>, IComparable<T>
{
    public UniqueList() { }

    public UniqueList(params T[] initialValues)
    {
        AddRange(initialValues);
    }


    private List<T> _items = new List<T>();

    public T this[int i] { get { return _items[i]; } }

    public IEnumerable<T> Items { get { foreach (var item in _items) { yield return item; } } }

    /// <summary>
    /// If the item given isn't already in the list, it 
    /// will be added and the method will return true.
    /// </summary>
    public bool Add(T item)
    {
        if (!_items.Contains(item))
        {
            _items.Add(item);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Attempts to add each item in turn and will 
    /// return the count of items added successfully.
    /// </summary>
    public int AddRange(params T[] items)
    {
        int added = 0;
        foreach (var item in items)
        {
            if (Add(item)) { added++; }
        }
        return added;
    }

    public void Clear() { _items.Clear(); }

    /// <summary>
    /// returns true if the item provided existed in the list 
    /// and was subsequently removed.
    /// </summary>
    public bool Remove(T item)
    {
        return _items.Remove(item);
    }

    /// <summary>
    /// Attempts to remove each item in turn and will 
    /// return the count of items removed successfully.
    /// </summary>
    public int RemoveRange(params T[] items)
    {
        int removed = 0;
        foreach (var item in items)
        {
            if (Remove(item)) { removed++; }
        }
        return removed;
    }

    /// <summary>
    /// Tells you if the item provided exists in the list.
    /// </summary>
    public bool Contains(T item)
    {
        return _items.Contains(item);
    }

    /// <summary>
    /// The total number of items existing in the list
    /// </summary>
    public int Count { get { return _items.Count; } }

    /// <summary>
    /// Uses the default comparer to sort the list
    /// </summary>
    public void Sort()
    {
        _items.Sort();
    }

    public IEnumerator<T> GetEnumerator()
    {
        foreach (var item in _items) { yield return item; }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

/// <summary>
/// Intended to avoid index out of range issues for high indexes that don't exist 
/// when reading the list.  Also allows setting by index and it will fill in new() 
/// items in the list if necessary.
/// </summary>
/// <typeparam name="T"></typeparam>
public class GhostList<T> : IEnumerable<T>, IEquatable<GhostList<T>> where T : IEquatable<T>, IComparable<T>, new()
{


    private List<T> _items = new();

    public int Count 
    { 
        get { return _items.Count; } 
        set
        {
            int currentCount = _items.Count;

            // easiest case
            if (currentCount == value) { return; }

            // if current count is high, we need to trim some off
            if (currentCount > value)
            {
                while (_items.Count > value) { _items.RemoveAt(_items.Count - 1); }
                return;
            }

            // if the current count is low, we need to tack on some empties
            while (_items.Count < value) { _items.Add(new()); }
        }
    }

    public int MaxIndex { get { return _items.Count - 1; } }

    public (int firstIndex, int lastIndex) IndexRange { get { return (0, MaxIndex); } }


    public T this[int i]
    {
        get
        {
            if (i < 0) { throw new ArgumentOutOfRangeException("negative index is not allowed"); }
            if (i >= _items.Count) { return new(); }

            return _items[i];
        }
        set
        {
            if (i < 0) { throw new ArgumentOutOfRangeException("negative index is not allowed"); }
            if (i < _items.Count) { _items[i] = value; }
            
            // now we know that they're trying to set at an index too high... we'll accomodate them anyway
            while (_items.Count < i) { _items.Add(new()); }
            _items.Add(value);
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        foreach (var item in _items)
        {
            yield return item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Clear() { _items.Clear(); }

    public void Add(T item) { _items.Add(item); }

    public void Insert(int targetIndex, T item)
    {
        if (targetIndex < 0) { throw new ArgumentOutOfRangeException("negative index is not allowed"); }

        if (targetIndex < _items.Count)
        {
            // this is the normal insert condition
            _items.Insert(targetIndex, item);
            return;
        }

        // now we know the insert is at an index above the existing stuff.  So, pad the list and then add
        while (_items.Count < targetIndex) { _items.Add(new()); }
        _items.Add(item);
    }
    
    public bool Remove(T item)
    {
        return _items.Remove(item);
    }

    public int RemoveAll(T item)
    {
        int numberRemoved = 0;

        for (int i = _items.Count - 1; i >= 0; i--)
        {
            if (_items[i].Equals(item))
            {
                _items.RemoveAt(i);
                numberRemoved++;
            }
        }

        return numberRemoved;
    }

    public void RemoveAt(int targetIndex)
    {
        if (targetIndex < 0) { throw new ArgumentOutOfRangeException("negative index is not allowed"); }

        if (targetIndex < _items.Count) { _items.RemoveAt(targetIndex); }
    }

    public bool Contains(T item)
    {
        return _items.Contains(item);
    }

    public void Sort() { _items.Sort(); }




    public bool Equals(GhostList<T>? other)
    {
        if (other is null) { return false; }

        return ListHelper.EqualByValue(_items, other._items);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) { return false; }

        if (obj is GhostList<T> glObj) { return Equals(glObj); }

        if (obj is List<T> lObj) { return ListHelper.EqualByValue(lObj, _items); }

        return false;
    }

    public override int GetHashCode()
    {
        return _items.GetHashCode();
    }

    public override string ToString()
    {
        return string.Join(", ", _items.Select(x => x.ToString()));
    }




}




