using System.Collections;
using UnityEngine;
using SimpleFileBrowser;
using static SimpleFileBrowser.FileBrowser;
using System.Collections.Generic;
using System.IO;


namespace SandboxGame
{

    /// <summary>
    /// Responsible for loading and saving files and dialog window
    /// </summary>
    public class FilesystemManager : Singleton<FilesystemManager>, IManager
    {
        //Public 
        public enum State { NONE, CLOSED, OPENFORSAVE, OPENFORLOAD }

        public State state;

        public bool IsAnyDialogOpen { get { return state != State.CLOSED; } }

        public OnSuccess OnSuccess;
        public OnCancel OnCancel;

        public List<OnSuccess> onSuccesses;
        public List<OnCancel> onCancels;



        protected override void Awake()
        {
            base.Awake();

            state = State.CLOSED;

            OnSuccess += OnSuccessCallback;
            OnCancel += OnCancelCallback;
        }

        #region ModalEvents

        public void OnSuccessCallback(string[] paths)
        {
            state = State.CLOSED;
            onSuccesses.ForEach(i => i.Invoke(paths));
        }

        public void OnCancelCallback()
        {
            state = State.CLOSED;
            onCancels.ForEach(i => i.Invoke());
        }

        #endregion

        public IEnumerator Init()
        {
            yield return null;
        }

        public void OpenNewDialog()
        {
            if (state == State.CLOSED)
            {
                ShowSaveDialog(OnSuccessCallback, OnCancelCallback, PickMode.Files);
                state = State.OPENFORSAVE;
            }
        }

        /// <summary>
        /// Coroutine version of OpenNewDialog
        /// </summary>
        /// <returns></returns>
        public IEnumerator OpenNewDialogRoutine()
        {
            state = State.OPENFORSAVE;
            yield return StartCoroutine(WaitForSaveDialog(PickMode.Files));
            state = State.CLOSED;
        }

        /// <summary>
        /// Open load file modal
        /// </summary>
        /// <returns></returns>
        public IEnumerator OpenLoadDialogRoutine()
        {
            state = State.OPENFORLOAD;
            yield return StartCoroutine(WaitForLoadDialog(PickMode.Files));
            state = State.CLOSED;
        }

        /// <summary>
        /// Save this json string to filename with fullpath
        /// </summary>
        /// <param name="jsonObject"></param>
        /// <param name="filename"></param>
        public bool SaveToFile(string json, string fullPath)
        {
            bool success = false;
            try
            {
                //Create directory if it doesnt exists
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                //Write serialized data to file
                using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(json);
                        success = true;
                    }
                }

            }
            catch (System.Exception e)
            {
                Debug.LogError("Error while storing data to file: " + fullPath + "\n" + e);
            }

            return success;
        }

        /// <summary>
        /// Read this json object from filename with path
        /// </summary>
        /// <param name="jsonObject"></param>
        /// <param name="filename"></param>
        public string LoadFromFile(string fileFullPath)
        {
            string dataToload = "";
            if (File.Exists(fileFullPath))
            {

                try
                {
                    using (FileStream stream = new FileStream(fileFullPath, FileMode.Open))
                    {
                        using (StreamReader reader=new StreamReader(stream))
                        {
                            dataToload = reader.ReadToEnd();
                        }
                    
                    }
                }
                catch (System.Exception e)
                {

                    Debug.LogError("Error occured when trying to load data from file: " + "\n" + e);
                }
            }

            return dataToload;
        }
    }
}