using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Responsible for loading and saving files
/// </summary>
public class FilesystemManager : Singleton<FilesystemManager>, IManager
{

    //Public 

    #region Events



    #endregion

    public IEnumerator Init()
    {
        yield return null;
    }

    /// <summary>
    /// Save this json object to filename with path
    /// </summary>
    /// <param name="jsonObject"></param>
    /// <param name="filename"></param>
    public void SaveToFile(object jsonObject, string filename)
    {
       
    }

    /// <summary>
    /// Read this json object from filename with path
    /// </summary>
    /// <param name="jsonObject"></param>
    /// <param name="filename"></param>
    public object LoadFromFile(string filename)
    {
        return null;
    }
}
