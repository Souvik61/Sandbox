using System.Collections;
using UnityEngine;
using SimpleFileBrowser;
using static SimpleFileBrowser.FileBrowser;
using System.Collections.Generic;


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

        public IEnumerator OpenNewDialogRoutine()
        {
            state = State.OPENFORSAVE;
            yield return StartCoroutine(WaitForSaveDialog(PickMode.Files));
            state = State.CLOSED;
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
}