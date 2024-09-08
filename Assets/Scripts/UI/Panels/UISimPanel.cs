using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISimPanel : MonoBehaviour
{

    public GameObject playButton;
    public GameObject pauseButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlayBtnClicked()
    {
        PhysicsSimulatorManager.Instance.RunSimulation();

        EnableButtonOutline(playButton, true);
        EnableButtonOutline(pauseButton, false);
    }

    public void OnPauseBtnClicked()
    {
        PhysicsSimulatorManager.Instance.PauseSimulation();

        EnableButtonOutline(playButton, false);
        EnableButtonOutline(pauseButton, true);
    }

    public void EnableButtonOutline(GameObject button, bool enable)
    {
        button.transform.Find("outline").gameObject.SetActive(enable);
    }

}
