using System.Collections.Generic;

public static class ExtensionMethods
{
    public static T RemoveElementAt<T>(this List<T> list, int index)
    {
        var result = list[index];
        list.RemoveAt(index);
        return result;
    }

    public static List<T> RemoveElements<T>(this List<T> list, int amount)
    {
        var result = list.GetRange(0, amount);
        list.RemoveRange(0, amount);
        return result;
    }
    
    public static void Randomize<T>(this List<T> list)
    {
        var aux = new List<T>(list);
        list.Clear();
        aux.RandomizeListTo(list);
    }

    public static void RandomizeListTo<T>(this List<T> origin, List<T> destination)
    {
        for (var i = origin.Count; i > 0; --i)
        {
            var index = UnityEngine.Random.Range(0, i);
            destination.Add(origin.RemoveElementAt(index));
        }
    }
}