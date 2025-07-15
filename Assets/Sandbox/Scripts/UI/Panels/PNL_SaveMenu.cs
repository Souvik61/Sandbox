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
        public GameObject btnMenu;

        public TMP_InputField projectInputField;

        public EditController editController;

        private void Awake()
        {
            //Set button references
            btnNew.GetComponent<Button>().onClick.AddListener(OnNewButtonClicked);
            btnLoad.GetComponent<Button>().onClick.AddListener(OnLoadButtonClicked);
            btnSave.GetComponent<Button>().onClick.AddListener(OnSaveButtonClicked);
            btnMenu.GetComponent<Button>().onClick.AddListener(OnMenuButtonClicked);
        }

        //------------------------
        //Button callbacks
        //------------------------

        void OnNewButtonClicked()
        {
            editController.OnNewButtonClicked();
        }

        void OnLoadButtonClicked()
        {
            editController.OnLoadButtonClicked();
        }

        void OnSaveButtonClicked()
        {
            editController.OnSaveButtonClicked();
        }

        void OnMenuButtonClicked()
        {
            editController.OnMenuButtonClicked();
        }
    }
}