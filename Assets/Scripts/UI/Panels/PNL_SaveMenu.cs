using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SandboxGame;

namespace SandboxGame
{
    public class PNL_SaveMenu : MonoBehaviour
    {
        [Header("Button References")]
        public GameObject btnNew;
        public GameObject btnLoad;
        public GameObject btnSave;

        public TMP_InputField projectInputField;

        public EditController editController;

        private void Awake()
        {
            //Set button references
            btnNew.GetComponent<Button>().onClick.AddListener(OnNewButtonClicked);
            btnLoad.GetComponent<Button>().onClick.AddListener(OnLoadButtonClicked);
            btnSave.GetComponent<Button>().onClick.AddListener(OnSaveButtonClicked);
        }


        //------------------------
        //Button callbacks
        //------------------------

        void OnNewButtonClicked()
        {

        }

        void OnLoadButtonClicked()
        {

        }

        void OnSaveButtonClicked()
        {

        }
    }
}