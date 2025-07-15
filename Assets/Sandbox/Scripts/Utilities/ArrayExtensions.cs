using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayExtensions
{
    public static bool IsInRange(Array _Array, int _Index)
    {
        if (_Index >= 0 && _Index <= _Array.Length - 1)
            return true;
        return false;
    }

    public static bool IsInRange<T>(List<T> _List, int _Index)
    {
        if (_Index >= 0 && _Index <= _List.Count - 1)
            return true;
        return false;
    }
}
