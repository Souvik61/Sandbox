using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;
using static SimpleFileBrowser.FileBrowser;

public class FilesystemTest : MonoBehaviour
{

    public OnSuccess OnSuccess;
    public OnCancel OnCancel;

    private void Awake()
    {
        OnSuccess += OnSuccessCallback;
        OnCancel += OnCancelCallback;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {

            ShowLoadDialog(OnSuccessCallback, OnCancelCallback, PickMode.Files);

        }
    }

    public void OnSuccessCallback(string[] paths)
    {

        Debug.Log(paths[0]);
    }

    public void OnCancelCallback()
    {
        Debug.Log("On Cancel");

    }
}
