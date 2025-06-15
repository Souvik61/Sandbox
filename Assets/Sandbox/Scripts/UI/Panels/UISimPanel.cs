using UnityEngine;

namespace SandboxGame
{

    public class UISimPanel : MonoBehaviour
    {

        public EditController EditControllerInstance;

        public GameObject playButton;
        public GameObject pauseButton;
        public GameObject resetButton;

        // Start is called before the first frame update
        void Start()
        {

        }

        public void OnPlayBtnClicked()
        {
            //PhysicsSimulatorManager.Instance.RunSimulation();
            EditControllerInstance.OnPlayButtonClicked();

            //EnableButtonOutline(playButton, true);
            //EnableButtonOutline(pauseButton, false);
            //EnableButtonOutline(resetButton, false);
        }

        public void OnPauseBtnClicked()
        {
            //PhysicsSimulatorManager.Instance.PauseSimulation();
            EditControllerInstance.OnPauseButtonClicked();

            //EnableButtonOutline(playButton, false);
            //EnableButtonOutline(pauseButton, true);
            //EnableButtonOutline(resetButton, false);
        }

        public void OnResetBtnClicked()
        {
            EditControllerInstance.OnResetButtonClicked();

            //EnableButtonOutline(playButton, false);
            //EnableButtonOutline(pauseButton, false);
            //EnableButtonOutline(resetButton, true);
        }

        public void EnableButtonOutline(GameObject button, bool enable)
        {
            button.transform.Find("outline").gameObject.SetActive(enable);
        }

        public void EnableButtonOutlineOnly(string btnName, bool enable)
        {
            EnableButtonOutline(playButton, false);
            EnableButtonOutline(pauseButton, false);
            EnableButtonOutline(resetButton, false);

            if (btnName == "PLAY")
            {
                EnableButtonOutline(playButton, enable);
            }
            else if (btnName == "RESET")
            {
                EnableButtonOutline(resetButton, enable);
            }
        }

    }
}