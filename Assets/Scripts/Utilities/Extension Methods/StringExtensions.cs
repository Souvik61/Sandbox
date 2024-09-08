using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class StringExtensions
{
    public static TEnum ConvertToEnum<TEnum>(this string enumValue, TEnum defaultValue) where TEnum : struct, System.IConvertible
    {
        System.Type EnumType = typeof(TEnum);

        Debug.Assert(EnumType.IsEnum, string.Format("Failed to convert to enum: Type {0} is not an enum", EnumType.Name));

        if (!System.Enum.IsDefined(EnumType, enumValue))
        {
            //Debug.Assert(true, string.Format("{0} is not an enum"));
            return defaultValue;
        }

        return (TEnum)System.Enum.Parse(EnumType, enumValue);
    }

    public static int ToHash(this string text, bool toLower = true)
    {
        if (toLower)
            text = text.ToLower();

        return Animator.StringToHash(text);
    }

    public static Hash128 ToHash128(this string text, bool toLower = true)
    {
        if (toLower)
            text = text.ToLower();

        return Hash128.Parse(text);
    }

    public static bool Contains(this string str, string substring, System.StringComparison comp)
    {
        return str.IndexOf(substring, comp) >= 0;
    }
}