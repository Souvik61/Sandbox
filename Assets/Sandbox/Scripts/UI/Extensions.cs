using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public interface IProbabilityItem { float Probability { get; } }

public static class Extensions
{
	private static DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
	public delegate void RecursiveFuntion(Transform self);
	public static void RecursiveOnChildren(Transform target, RecursiveFuntion function)
	{
		function(target);
		for (int i = 0; i < target.childCount; ++i)
			RecursiveOnChildren(target.GetChild(i), function);
	}
	public static void CallOnChildren(Transform target, RecursiveFuntion function)
	{
		for (int i = 0; i < target.childCount; ++i)
			function(target.GetChild(i));
	}

	internal static long EpochNow { get
		{
			return (long)(DateTime.UtcNow - epochStart).TotalSeconds;
		}
	}

	#region Value Parsing

	/// <summary>
	/// Parses String to Enum.
	/// </summary>
	/// <returns>The Enum.</returns>
	/// <param name="value">String Value.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static T ParseEnum<T>(this string value, bool ignoreFailedParse = false)
	{
		T convertedEnum;

		try { convertedEnum = (T)Enum.Parse(typeof(T), value, true); }
		catch (Exception) 
		{
			string err = "Could not parse '" + value + "' into " + typeof(T).ToString();
			if (!ignoreFailedParse)
				throw new UnityException(err);
			else
			{
				Debug.LogError(err);
				return default;
			}
		}

		return convertedEnum;
	}

	/// <summary>
	/// Parses String to Int.
	/// </summary>
	/// <returns>The Int.</returns>
	/// <param name="value">String Value.</param>
	public static int ParseInt(this string value)
	{
		int parsedValue;

		if (!int.TryParse(value, out parsedValue))
			throw new UnityException("Could not parse '" + value + "' into int.");

		return parsedValue;
	}

	/// <summary>
	/// Parses String to Float.
	/// </summary>
	/// <returns>The Float.</returns>
	/// <param name="value">String Value.</param>
	public static float ParseFloat(this string value)
	{
		float parsedValue;

		if (!float.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out parsedValue))
			throw new UnityException("Could not parse '" + value + "' into float.");

		return parsedValue;
	}

	/// <summary>
	/// Parses String to Double
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	public static double ParseDouble(this string value)
	{
		double parsedValue;

		if (!double.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out parsedValue))
			throw new UnityException("Could not parse '" + value + "' into double.");

		return parsedValue;
	}

	/// <summary>
	/// Parses String to Bool.
	/// </summary>
	/// <returns><c>true</c>, if bool was parsed, <c>false</c> otherwise.</returns>
	/// <param name="value">Value.</param>
	public static bool ParseBool(this string value)
	{
		bool parsedValue;

		if (!bool.TryParse(value, out parsedValue))
			throw new UnityException("Could not parse '" + value + "' into bool.");

		return parsedValue;
	}

	#endregion

	#region Time

	/// <summary>
	/// Converts from unix timestamp to DateTime.
	/// </summary>
	/// <returns>The from unix timestamp.</returns>
	/// <param name="timestamp">Timestamp.</param>
	public static DateTime ConvertFromUnixTimestamp(double timestamp)
	{
		DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		return origin.AddSeconds(timestamp);
	}

	/// <summary>
	/// Converts to unix timestamp.
	/// </summary>
	/// <returns>The to unix timestamp.</returns>
	/// <param name="date">Date.</param>
	public static double ConvertToUnixTimestamp(DateTime date)
	{
		//DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		TimeSpan diff = date.ToUniversalTime() - epochStart;
		return Math.Floor(diff.TotalSeconds);
	}

	/// <summary>
	/// Converts a Seconds Value you to a minutes seconds timer
	/// </summary>
	/// <param name="_seconds"></param>
	/// <returns></returns>
	public static string FormatSecondToTimer(float _seconds, string _additional = "")
	{
		int hoursNum = ((int)(_seconds / 3600f));
		string hours = hoursNum.ToString();
		if (hours.Length <= 1)
			hours = "0" + hours;

		string minutes = ((int)(_seconds % 3600) / 60).ToString();
		if (minutes.Length <= 1)
			minutes = "0" + minutes;

		string seconds = ((int)(_seconds % 3600) % 60).ToString();
		if (seconds.Length <= 1)
			seconds = "0" + seconds;

		if(hoursNum > 0)
			return $"{hours}:{minutes}:{seconds}";
		else
			return $"{minutes}:{seconds}";
	}

	/// <summary>
	/// Convert a 24hr time ( string ) to 12hr time
	/// </summary>
	/// <param name="_time"></param>
	/// <returns></returns>
	public static string Format24hrTimeToDigital(string _time)
	{
		int time = ParseInt(_time);
		string extension = "am";

		int remainder = time % 100;
		time = time / 100; // Divide by 100 to get into single number

		extension = time >= 12 ? "pm" : "am"; // Set the Extension on the Number

		time = time > 12 ? time - 12 : time; // let time convert to 12hr if over 12 oclock

		return string.Format("{0:00}:{1:00}" + extension, time, remainder);// time.ToString() + ":" + remainder.ToString(string.Format("{00}")) + extension;
	}

	#endregion

	/// <summary>
	/// Destroy all from a list
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="_items"></param>
	public static void DestroyAllGameObject<T>(this List<T> _items) where T : UnityEngine.Object
	{
		if (_items != null && _items.Count > 0)
		{
			for (int i = _items.Count - 1; i > -1; i--)
			{
				if (_items[i] == null)
				{
					continue;
				}

				GameObject.Destroy(_items[i]);
			}

			_items.Clear();
		}
	}

	/// <summary>
	/// Destroy all from a list
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="_items"></param>
	public static void DestroyAll<T>(this List<T> _items) where T : MonoBehaviour
	{
		if (_items != null && _items.Count > 0)
		{
			for (int i = _items.Count - 1; i > -1; i--)
			{
				if(_items[i] == null)
				{
#if UNITY_EDITOR
					Debug.Log("Null objects in list being destoryed!");
#endif
					continue;
				}

				GameObject.Destroy(_items[i].gameObject);
			}

			_items.Clear();
		}
	}

	/// <summary>
	/// Destroy all from an Array
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="_items"></param>
	public static void DestroyAll<T>(this T[] _items) where T : MonoBehaviour
	{
		if (_items != null && _items.Length > 0)
		{
			for (int i = _items.Length - 1; i > -1; i--)
			{
				if (_items[i] == null)
				{
#if UNITY_EDITOR
					Debug.Log("Null objects in array being destoryed!");
#endif
					continue;
				}

				GameObject.Destroy(_items[i].gameObject);
			}

			_items = null;
		}
	}

	/// <summary>
	/// Destory all Immidate
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="_items"></param>
	[ExecuteInEditMode]
	public static void DestroyAllImmediate<T>(this T[] _items) where T : MonoBehaviour
	{
		if (_items != null && _items.Length > 0)
		{
			for (int i = _items.Length - 1; i > -1; i--)
			{
				if (_items[i] == null)
				{
					Debug.Log("Null objects in array being destoryed!");
					continue;
				}

				GameObject.DestroyImmediate(_items[i].gameObject);
			}

			_items = null;
		}
	}

	/// <summary>
	/// Check if the Pointer is Over UI
	/// </summary>
	/// <returns></returns>
	public static bool IsPointerOverUI()
	{
		if (Application.isEditor)
		{
			return EventSystem.current.IsPointerOverGameObject();
		}
		else
		{
			return EventSystem.current.IsPointerOverGameObject(0) || EventSystem.current.IsPointerOverGameObject();
		}
	}

	/// <summary>
	/// Get a string via Enum Index from String Array
	/// </summary>
	/// <param name="_data"></param>
	/// <param name="_enum"></param>
	/// <returns></returns>
	public static string StringFromEnum(this string[] _data, System.Enum _enum)
	{
        string stringToReturn = "";
        try
        {
            stringToReturn = _data[(int)((object)_enum)];
        }
        catch (Exception)
        {
            throw;
        }
        return stringToReturn;
	}

	/// <summary>
	/// Clamp an Angle
	/// </summary>
	/// <param name="_angle"></param>
	/// <param name="_from"></param>
	/// <param name="_to"></param>
	/// <returns></returns>
	public static float ClampAngle(float _angle, float _from, float _to)
	{
		if (_angle < 0f)
			_angle = 360 + _angle;

		if (_angle > 180f)
			return Mathf.Max(_angle, 360 + _from);

		return Mathf.Min(_angle, _to);
	}

	/// <summary>
	/// Quick remove from a List
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="_list"></param>
	/// <param name="_object"></param>
	public static void QuickRemove<T>(this List<T> _list, T _object)
	{
		int index = _list.IndexOf(_object);
		int count = _list.Count - 1;

		_list[index] = _list[count];
		_list.RemoveAt(count);
	}

	/// <summary>
	/// Quick remove from a List
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="_list"></param>
	/// <param name="_object"></param>
	public static void QuickRemove<T>(this List<T> _list, int _index)
	{
		int count = _list.Count - 1;

		_list[_index] = _list[count];
		_list.RemoveAt(count);
	}

	/// <summary>
	/// Takes two dictionaries and merges their entries-
	/// for elements in both dictionaries, the ones from a_newer are used.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="U"></typeparam>
	/// <param name="a_older"></param>
	/// <param name="a_newer"></param>
	/// <returns></returns>
	public static Dictionary<T, U> MergeDictionaries<T, U>(Dictionary<T, U> a_older, Dictionary<T, U> a_newer) where T : System.IComparable<T>
	{
		foreach (KeyValuePair<T, U> pair in a_older)
		{
			bool newerHasKey = false;
			foreach (T key in a_newer.Keys)
			{
				if (EqualityComparer<T>.Default.Equals(key, pair.Key))
				{
					newerHasKey = true;
					break;
				}
			}
			if (!newerHasKey)
				a_newer.Add(pair.Key, pair.Value);
		}

		return a_newer;
	}

	/// <summary>
	/// Merge a list together
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="_oldList"></param>
	/// <param name="_newList"></param>
	/// <returns></returns>
	public static List<T> MergeList<T>(List<T> _oldList, List<T> _newList)
	{
		int count = _newList.Count;
		for (int i = 0; i < count; i++)
		{
			_oldList.Add(_newList[i]);
		}

		return _oldList;
	}

	/// <summary>
	/// Dequeue an item from a list
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list"></param>
	/// <returns></returns>
	public static T Dequeue<T>(this List<T> list)
	{
		T toReturn = list[0];
		list.RemoveAt(0);
		return toReturn;
	}

	#region Get Random Logic 

	/// <summary>
	/// Get the index of a weighted random object in an array
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="items"></param>
	/// <returns></returns>
	public static int GetWeightedRandomIndex<T>(this T[] items) where T : IProbabilityItem
	{
		float total = 0f, cumTotal = 0f;
		for (int i = 0; i < items.Length; i++)
			total += items[i].Probability;

		float r = UnityEngine.Random.Range(0f, total);

		for (int i = 0; i < items.Length; i++)
		{
			cumTotal += items[i].Probability;
			if (r < cumTotal)
				return i;
		}
		return items.Length - 1;
	}

	/// <summary>
	/// Get the index of a weighted random object in a list
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="items"></param>
	/// <returns></returns>
	public static int GetWeightedRandomIndex<T>(this List<T> items) where T : IProbabilityItem
	{
		float total = 0f, cumTotal = 0f;
		for (int i = 0; i < items.Count; i++)
			total += items[i].Probability;

		float r = UnityEngine.Random.Range(0f, total);

		for (int i = 0; i < items.Count; i++)
		{
			cumTotal += items[i].Probability;
			if (r < cumTotal)
				return i;
		}
		return items.Count - 1;
	}

	public static T GetWeightedRandomItem<T>(this T[] items) where T : IProbabilityItem { return items[items.GetWeightedRandomIndex()]; }
	public static T GetWeightedRandomItem<T>(this List<T> items) where T : IProbabilityItem { return items[items.GetWeightedRandomIndex()]; }

	#endregion

	#region Enum Flags

	/// <summary>
	/// Add a flag to an enum
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="_enumToAdjust"></param>
	/// <param name="_valueToAdd"></param>
	/// <returns></returns>
	public static T AddFlag<T>(T _enumToAdjust, T _valueToAdd) where T : Enum
	{
		int toAdjust = (int)(object)_enumToAdjust;
		int valueToAdd = (int)(object)_valueToAdd;
		
		return (T)(object)(toAdjust | valueToAdd);
	}

	/// <summary>
	/// Add a flag to an enum
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="_enumToAdjust"></param>
	/// <param name="_valueToRemove"></param>
	/// <returns></returns>
	public static T RemoveFlag<T>(T _enumToAdjust, T _valueToRemove) where T : Enum
	{
		int toAdjust = (int)(object)_enumToAdjust;
		int valueToRemove = (int)(object)_valueToRemove;

		return (T)(object)(toAdjust & (~valueToRemove));
	}

	/// <summary>
	/// Check if an Enum contains a flag
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="_enumToCheck"></param>
	/// <param name="_valueToCheck"></param>
	/// <returns></returns>
	public static bool ContainsSpecificFlag<T>(T _enumToCheck, T _valueToCheck) where T : Enum
	{
		int toAdjust = (int)(object)_enumToCheck;
		int valueToCheck = (int)(object)_valueToCheck;

		return (toAdjust & valueToCheck) == valueToCheck;
	}

	/// <summary>
	/// Check if two bit shifted Enums (ints) contain any of the same flags
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="_compareOne"></param>
	/// <param name="_compareTwo"></param>
	/// <returns></returns>
	public static bool ContainsAnyFlag<T>(T _compareOne, T _compareTwo) where T : Enum
	{
		int toAdjust = (int)(object)_compareOne;
		int valueToCheck = (int)(object)_compareTwo;

		return (toAdjust & valueToCheck) != 0;
	}

	#endregion

	#region Serialization

	/// <summary>
	/// Serialize a Dictionary
	/// </summary>
	/// <param name="_data"></param>
	/// <returns></returns>
	internal static string SerializeDictionary<T>(Dictionary<T, object> _data)
	{
		int count = _data.Count;
		int counter = 0;

		string customJson = "{";
		foreach (T key in _data.Keys)
		{
			Type currentType = _data[key].GetType();
			bool noQuotation = currentType == typeof(bool) || currentType == typeof(int) || currentType == typeof(float);

			string quotationMarks = (noQuotation ? "" : "\"");
			string dataValue = (noQuotation ? _data[key].ToString().ToLower() : _data[key].ToString());

			customJson += "\"" + key + "\":" + quotationMarks + dataValue + quotationMarks;
			if (++counter < count)
				customJson += ",";
		}
		customJson += "}";
		return System.Text.RegularExpressions.Regex.Unescape(customJson);
	}

	#endregion

	#region UI

	/// <summary>
	/// Calculates a rect of the screen positions of the sprite's corners
	/// </summary>
	/// <returns></returns>
	internal static Rect SpriteToScreenRect(RectTransform _transform, Camera _camera)
	{
		Vector3[] corners = new Vector3[4];
		_transform.GetWorldCorners(corners);

		Rect screenRect = new Rect();
		screenRect.xMin = _camera.WorldToScreenPoint(corners[0]).x;
		screenRect.xMax = _camera.WorldToScreenPoint(corners[2]).x;
		screenRect.yMin = _camera.WorldToScreenPoint(corners[0]).y;
		screenRect.yMax = _camera.WorldToScreenPoint(corners[2]).y;

		return screenRect;
	}

	#endregion

	#region Transforms

	public static Vector3 SetLocalX(this Transform v, float x) { return v.localPosition = new Vector3(x, v.localPosition.y, v.localPosition.z); }
	public static Vector3 SetLocalY(this Transform v, float y) { return v.localPosition = new Vector3(v.localPosition.x, y, v.localPosition.z); }
	public static Vector3 SetLocalZ(this Transform v, float z) { return v.localPosition = new Vector3(v.localPosition.x, v.localPosition.y, z); }

	public static Vector3 AddLocalX(this Transform v, float x) { return v.localPosition = new Vector3(v.localPosition.x + x, v.localPosition.y, v.localPosition.z); }
	public static Vector3 AddLocalY(this Transform v, float y) { return v.localPosition = new Vector3(v.localPosition.x, v.localPosition.y + y, v.localPosition.z); }
	public static Vector3 AddLocalZ(this Transform v, float z) { return v.localPosition = new Vector3(v.localPosition.x, v.localPosition.y, v.localPosition.z + z); }

	public static Vector3 SetLocalScale(this Transform v, Vector3 scale) { return v.localScale = new Vector3(scale.x, scale.y, scale.z); }

	public static Vector3 InvertX(this Vector3 v) { return new Vector3(-v.x, v.y, v.z); }
	public static Vector3 InvertY(this Vector3 v) { return new Vector3(v.x, -v.y, v.z); }
	public static Vector3 InvertZ(this Vector3 v) { return new Vector3(v.x, v.y, -v.z); }

	#endregion
}