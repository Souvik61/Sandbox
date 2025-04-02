using UnityEngine;

namespace SandboxGame
{

    public class UISimPanel : MonoBehaviour
    {

        public EditController EditControllerInstance;

        public GameObject playButton;
        public GameObject pauseButton;

        // Start is called before the first frame update
        void Start()
        {

        }

        public void OnPlayBtnClicked()
        {
            //PhysicsSimulatorManager.Instance.RunSimulation();
            EditControllerInstance.OnPlayButtonClicked();

            EnableButtonOutline(playButton, true);
            EnableButtonOutline(pauseButton, false);
        }

        public void OnPauseBtnClicked()
        {
            //PhysicsSimulatorManager.Instance.PauseSimulation();
            EditControllerInstance.OnPauseButtonClicked();

            EnableButtonOutline(playButton, false);
            EnableButtonOutline(pauseButton, true);
        }

        public void EnableButtonOutline(GameObject button, bool enable)
        {
            button.transform.Find("outline").gameObject.SetActive(enable);
        }

    }
}