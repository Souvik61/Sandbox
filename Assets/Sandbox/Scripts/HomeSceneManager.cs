using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SandboxGame
{
    public class HomeSceneManager : MonoBehaviour
    {
        public Button BuildButton;
        public Button SettingsButton;
        public Button HowtoButton;
        public Button CreditsButton;
        public PNL_Settings SettingsPanel;
        public GameObject HowToPanel;
        public GameObject CreditsPanel;


        private void Awake()
        {
            BuildButton.onClick.AddListener(OnBuildButtonClicked);
            SettingsButton.onClick.AddListener(OnSettingsButtonClicked);
            HowtoButton.onClick.AddListener(OnHowtoButtonClicked);
            CreditsButton.onClick.AddListener(OnCreditsButtonClicked);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnBuildButtonClicked()
        {

            GameManager.Instance.LoadGameScene();
        }

        void OnSettingsButtonClicked()
        {
            SettingsPanel.gameObject.SetActive(true);
        }

        void OnHowtoButtonClicked()
        {
            HowToPanel.SetActive(true);
        }

        void OnCreditsButtonClicked()
        {
            CreditsPanel.SetActive(true);
        }

    }
}
