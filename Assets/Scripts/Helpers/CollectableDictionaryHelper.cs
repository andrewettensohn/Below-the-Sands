using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public static class CollectableDictionaryHelper
{
    public static Dictionary<T, bool> GetCollectableDictionaryForEnum<T>()
    {
        Dictionary<T, bool> dictionary = new Dictionary<T, bool>();

        List<T> enumValues = Enum.GetValues(typeof(T)).Cast<T>().ToList();
        enumValues.ForEach(x => dictionary.Add(x, true));

        return dictionary;
    }
}
